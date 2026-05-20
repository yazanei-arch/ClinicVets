using System;
using System.IO;
using ClosedXML.Excel;
using System.Drawing;
using System.Windows.Forms;

namespace ClinicVets.UI
{
    public partial class AddPetForm : Form
    {
        private readonly string filePath = ExcelFileManager.FilePath;

        private Label lblPetNameError = new Label();
        private Label lblAnimalTypeError = new Label();
        private Label lblWeightError = new Label();
        private Label lblBirthDateError = new Label();
        private Label lblOwnerError = new Label();
        private Label lblChipNumberError = new Label();
        private Label lblLastVaccineError = new Label();

        public AddPetForm()
        {
            InitializeComponent();
            EnsureErrorLabels();
            LoadAnimalTypes();
        }

        private void EnsureErrorLabels()
        {
            SetupErrorLabel(lblPetNameError, txtPetName);
            SetupErrorLabel(lblAnimalTypeError, cmbAnimalType);
            SetupErrorLabel(lblWeightError, txtWeight);
            SetupErrorLabel(lblBirthDateError, dtpBirthDate);
            SetupErrorLabel(lblOwnerError, txtOwner);
            SetupErrorLabel(lblChipNumberError, txtChipNumber);
            SetupErrorLabel(lblLastVaccineError, dtpLastVaccineDate);
        }

        private void SetupErrorLabel(Label label, Control control)
        {
            label.Text = "";
            label.ForeColor = Color.Red;
            label.BackColor = Color.Transparent;
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            label.Location = new Point(control.Left, control.Bottom + 3);

            if (!this.Controls.Contains(label))
                this.Controls.Add(label);

            label.BringToFront();
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
                        cmbAnimalType.Items.Add(animalType);
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
            txtOwner.BackColor = Color.White;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtPetName.Clear();
            cmbAnimalType.SelectedIndex = -1;
            txtWeight.Clear();
            txtChipNumber.Clear();
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
            string owner = txtOwner.Text.Trim();

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

            if (string.IsNullOrWhiteSpace(animalType))
            {
                lblAnimalTypeError.Text = "Select animal type.";
                cmbAnimalType.BackColor = Color.MistyRose;
                isValid = false;
            }

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

            if (string.IsNullOrWhiteSpace(owner))
            {
                lblOwnerError.Text = "Owner is required.";
                txtOwner.BackColor = Color.MistyRose;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(chipNumber))
            {
                lblChipNumberError.Text = "Chip number is required.";
                txtChipNumber.BackColor = Color.MistyRose;
                isValid = false;
            }

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
            SavePetToExcel(pet);

            MessageBox.Show("Pet saved successfully.");
            btnClear_Click(sender, e);
        }

        private void SavePetToExcel(Pet pet)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Excel file not found");
                return;
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                IXLWorksheet sheet;

                if (workbook.Worksheets.Contains("Pets"))
                {
                    sheet = workbook.Worksheet("Pets");
                }
                else
                {
                    sheet = workbook.Worksheets.Add("Pets");

                    sheet.Cell(1, 1).Value = "PetID";
                    sheet.Cell(1, 2).Value = "PetName";
                    sheet.Cell(1, 3).Value = "AnimalType";
                    sheet.Cell(1, 4).Value = "Weight";
                    sheet.Cell(1, 5).Value = "BirthDate";
                    sheet.Cell(1, 6).Value = "Owner";
                    sheet.Cell(1, 7).Value = "ChipNumber";
                    sheet.Cell(1, 8).Value = "LastVaccineDate";
                }

                int lastRow;

                if (sheet.LastRowUsed() == null)
                    lastRow = 2;
                else
                    lastRow = sheet.LastRowUsed().RowNumber() + 1;

                sheet.Cell(lastRow, 1).Value = pet.PetID;
                sheet.Cell(lastRow, 2).Value = pet.PetName;
                sheet.Cell(lastRow, 3).Value = pet.AnimalType;
                sheet.Cell(lastRow, 4).Value = pet.Weight;
                sheet.Cell(lastRow, 5).Value = pet.BirthDate;
                sheet.Cell(lastRow, 6).Value = pet.Owner;
                sheet.Cell(lastRow, 7).Value = pet.ChipNumber;
                sheet.Cell(lastRow, 8).Value = pet.LastVaccineDate;

                workbook.Save();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            PetManagementForm form = new PetManagementForm();
            form.ShowDialog();
            this.Hide();
        }

        
    }
}