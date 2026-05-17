using System;
using System.IO;
using ClosedXML.Excel;
using System.Drawing;
using System.Windows.Forms;
using ClinicVets.UI;

namespace ClinicVets.UI
{
    public partial class AddPetForm : Form
    {
        private readonly string filePath = ExcelFileManager.FilePath;
        public AddPetForm()
        {
            InitializeComponent();
            LoadAnimalTypes();
        }

        private void LoadAnimalTypes()
        {
            cmbAnimalType.Items.Clear();

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Excel file not found");
                return;
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                if (!workbook.Worksheets.Contains("AnimalTypes"))
                {
                    MessageBox.Show("AnimalTypes sheet not found");
                    return;
                }

                var sheet = workbook.Worksheet("AnimalTypes");
                var range = sheet.RangeUsed();

                if (range == null)
                    return;

                foreach (var row in range.RowsUsed())
                {
                    if (row.RowNumber() == 1)
                        continue;

                    string animalType = row.Cell(1).GetValue<string>().Trim();

                    if (!string.IsNullOrWhiteSpace(animalType))
                    {
                        cmbAnimalType.Items.Add(animalType);
                    }
                }
            }

            cmbAnimalType.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void ClearErrorLabels()
        {
            lblPetNameError.Text = "";
            lblAnimalTypeError.Text = "";
            lblWeightError.Text = "";
            lblBirthDateError.Text = "";
            lblOwnerError.Text = "";
            lblChipNumberError.Text = "";
            lblLastVaccineError.Text = "";

            txtPetName.BackColor = Color.White;
            cmbAnimalType.BackColor = Color.White;
            txtWeight.BackColor = Color.White;
            txtChipNumber.BackColor = Color.White;

            if (this.Controls.Find("txtOwner", true).Length > 0)
                txtOwner.BackColor = Color.White;

            if (this.Controls.Find("txtOwner", true).Length > 0)
                txtOwner.BackColor = Color.White;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtPetName.Clear();
            cmbAnimalType.SelectedIndex = -1;
            txtWeight.Clear();
            txtChipNumber.Clear();

            if (this.Controls.Find("txtOwner", true).Length > 0)
                txtOwner.Clear();

            if (this.Controls.Find("cmbOwner", true).Length > 0)
                txtOwner.Clear();

            dtpBirthDate.Value = DateTime.Now;
            dtpLastVaccineDate.Value = DateTime.Now;

            ClearErrorLabels();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ClearErrorLabels();

            bool isValid = true;

            string petName = txtPetName.Text.Trim();
            string animalType = cmbAnimalType.Text.Trim();
            string weightText = txtWeight.Text.Trim();
            string chipNumber = txtChipNumber.Text.Trim();

            string owner = "";
            if (this.Controls.Find("txtOwner", true).Length > 0)
                owner = txtOwner.Text.Trim();

            if (this.Controls.Find("cmbOwner", true).Length > 0)
                owner = txtOwner.Text.Trim();

            // Pet Name
            if (string.IsNullOrWhiteSpace(petName))
            {
                lblPetNameError.Text = "Pet name is required.";
                txtPetName.BackColor = Color.MistyRose;
                isValid = false;
            }
            else
            {
                foreach (char c in petName)
                {
                    if (!char.IsLetter(c) && c != ' ')
                    {
                        lblPetNameError.Text = "Only letters are allowed.";
                        txtPetName.BackColor = Color.MistyRose;
                        isValid = false;
                        break;
                    }
                }
            }

            // Animal Type
            if (string.IsNullOrWhiteSpace(animalType))
            {
                lblAnimalTypeError.Text = "Select animal type.";
                cmbAnimalType.BackColor = Color.MistyRose;
                isValid = false;
            }

            // Weight
            double weight = 0;
            if (string.IsNullOrWhiteSpace(weightText))
            {
                lblWeightError.Text = "Weight is required.";
                txtWeight.BackColor = Color.MistyRose;
                isValid = false;
            }
            else if (!double.TryParse(weightText, out weight))
            {
                lblWeightError.Text = "Weight must be a number.";
                txtWeight.BackColor = Color.MistyRose;
                isValid = false;
            }
            else if (weight < 0.1 || weight > 100)
            {
                lblWeightError.Text = "Weight must be 0.1 - 100.";
                txtWeight.BackColor = Color.MistyRose;
                isValid = false;
            }

            // Birth Date
            if (dtpBirthDate.Value.Date > DateTime.Now.Date)
            {
                lblBirthDateError.Text = "Birth date cannot be future.";
                isValid = false;
            }
            else if (dtpBirthDate.Value.Year < 2000)
            {
                lblBirthDateError.Text = "Birth date cannot be before 2000.";
                isValid = false;
            }

            // Owner
            if (string.IsNullOrWhiteSpace(owner))
            {
                lblOwnerError.Text = "Owner is required.";

                if (this.Controls.Find("txtOwner", true).Length > 0)
                    txtOwner.BackColor = Color.MistyRose;

                if (this.Controls.Find("cmbOwner", true).Length > 0)
                    txtOwner.BackColor = Color.MistyRose;

                isValid = false;
            }

            // Chip Number
            if (string.IsNullOrWhiteSpace(chipNumber))
            {
                lblChipNumberError.Text = "Chip number is required.";
                txtChipNumber.BackColor = Color.MistyRose;
                isValid = false;
            }

            // Last Vaccine Date
            if (dtpLastVaccineDate.Value.Date > DateTime.Now.Date)
            {
                lblLastVaccineError.Text = "Vaccine date cannot be future.";
                isValid = false;
            }

            if (!isValid)
                return;

            PetRepository repo = new PetRepository();

            Pet pet = new Pet
            {
                PetID = repo.GeneratePetID(),
                PetName = petName,
                AnimalType = animalType,
                Weight = weight,
                BirthDate = dtpBirthDate.Value.Date,
                Owner = owner,
                ChipNumber = chipNumber,
                LastVaccineDate = dtpLastVaccineDate.Value.Date
            };

            repo.AddPet(pet);

            MessageBox.Show("Pet saved successfully.");
            btnClear_Click(sender, e);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            PetManagementForm form = new PetManagementForm();
            form.ShowDialog();
            this.Hide();
        }
    }
}