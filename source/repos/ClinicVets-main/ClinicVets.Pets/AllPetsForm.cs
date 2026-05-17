using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ClosedXML.Excel;


namespace ClinicVets.UI
{
    public partial class AllPetsForm : Form
    {
        private readonly string filePath = ExcelFileManager.FilePath;
        private bool isUpdateMode = false;
        private int editingRowIndex = -1;
        private string editingPetCode = "";

        public AllPetsForm()
        {
            InitializeComponent();
            dgvPets.EnableHeadersVisualStyles = false;

            dgvPets.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgvPets.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPets.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvPets.ColumnHeadersHeight = 45;

            dgvPets.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvPets.DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue;
            dgvPets.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvPets.BackgroundColor = Color.White;
            dgvPets.GridColor = Color.LightGray;
            dgvPets.BorderStyle = BorderStyle.FixedSingle;

            dgvPets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPets.ReadOnly = true;
            dgvPets.AllowUserToAddRows = false;
            dgvPets.AllowUserToDeleteRows = false;
            dgvPets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPets.RowHeadersVisible = false;

        }

        private void AllPetsForm_Load(object sender, EventArgs e)
        {
            LoadPetsFromExcel();
        }

        private void LoadPetsFromExcel()
        {
            dgvPets.Rows.Clear();

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Excel file not found:\n" + filePath);
                return;
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                var sheet = workbook.Worksheet("Pets");

                if (sheet == null)
                {
                    MessageBox.Show("Sheet Pets not found");
                    return;
                }

                var usedRange = sheet.RangeUsed();

                if (usedRange == null)
                {
                    MessageBox.Show("No data found in Pets sheet");
                    return;
                }

                foreach (var row in usedRange.RowsUsed())
                {
                    if (row.RowNumber() == 1)
                        continue;

                    dgvPets.Rows.Add(
                        row.Cell(1).GetValue<string>(), // PetID
                        row.Cell(2).GetValue<string>(), // PetName
                        row.Cell(3).GetValue<string>(), // AnimalType
                        row.Cell(4).GetValue<string>(), // Weight
                        row.Cell(5).GetValue<string>(), // BirthDate
                        row.Cell(6).GetValue<string>(), // Owner
                        row.Cell(7).GetValue<string>(), // ChipNumber
                        row.Cell(8).GetValue<string>()  // LastVaccineDate
                    );
                }
            }

            dgvPets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnBack_Click_1(object sender, EventArgs e)
        {
            PetManagementForm form = new PetManagementForm();
            form.ShowDialog();
            this.Hide();
        }

        private void btnDeletePet_Click(object sender, EventArgs e)
        {
            if (dgvPets.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a pet to delete");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this pet?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            int selectedIndex = dgvPets.SelectedRows[0].Index;

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Excel file not found:\n" + filePath);
                return;
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                var sheet = workbook.Worksheet("Pets");

                if (sheet == null)
                {
                    MessageBox.Show("Sheet Pets not found");
                    return;
                }

                int excelRowNumber = selectedIndex + 2;

                sheet.Row(excelRowNumber).Delete();

                workbook.Save();
            }

            LoadPetsFromExcel();

            MessageBox.Show("Pet deleted successfully");
        }

        private void btnUpdatePet_Click(object sender, EventArgs e)
        {
            if (!isUpdateMode)
            {
                if (dgvPets.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Select a pet to update");
                    return;
                }

                editingRowIndex = dgvPets.SelectedRows[0].Index;
                editingPetCode = dgvPets.Rows[editingRowIndex].Cells[0].Value.ToString();

                dgvPets.ReadOnly = false;

                foreach (DataGridViewRow row in dgvPets.Rows)
                {
                    row.ReadOnly = true;
                }

                dgvPets.Rows[editingRowIndex].ReadOnly = false;

                // Pet Code should not be changed
                dgvPets.Rows[editingRowIndex].Cells[0].ReadOnly = true;

                isUpdateMode = true;
                btnUpdatePet.Text = "Save Update";

                MessageBox.Show("You can now edit the selected pet row");
            }
            else
            {
                SaveUpdatedPetToExcel();

                dgvPets.ReadOnly = true;
                isUpdateMode = false;
                editingRowIndex = -1;
                editingPetCode = "";

                btnUpdatePet.Text = "Update Pet";

                LoadPetsFromExcel();

                MessageBox.Show("Pet updated successfully");
            }
        }
        private void SaveUpdatedPetToExcel()
        {
            if (editingRowIndex == -1)
            {
                MessageBox.Show("No pet selected for update");
                return;
            }

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Excel file not found:\n" + filePath);
                return;
            }

            string petCode = dgvPets.Rows[editingRowIndex].Cells[0].Value.ToString();
            string petName = dgvPets.Rows[editingRowIndex].Cells[1].Value.ToString();
            string animalType = dgvPets.Rows[editingRowIndex].Cells[2].Value.ToString();
            string weight = dgvPets.Rows[editingRowIndex].Cells[3].Value.ToString();
            string birthDate = dgvPets.Rows[editingRowIndex].Cells[4].Value.ToString();
            string owner = dgvPets.Rows[editingRowIndex].Cells[5].Value.ToString();
            string chipNumber = dgvPets.Rows[editingRowIndex].Cells[6].Value.ToString();
            string lastVaccineDate = dgvPets.Rows[editingRowIndex].Cells[7].Value.ToString();

            using (var workbook = new XLWorkbook(filePath))
            {
                var sheet = workbook.Worksheet("Pets");

                if (sheet == null)
                {
                    MessageBox.Show("Sheet Pets not found");
                    return;
                }

                var usedRange = sheet.RangeUsed();

                if (usedRange == null)
                {
                    MessageBox.Show("No data found in Pets sheet");
                    return;
                }

                foreach (var row in usedRange.RowsUsed())
                {
                    if (row.RowNumber() == 1)
                        continue;

                    string currentPetCode = row.Cell(1).GetValue<string>();

                    if (currentPetCode == editingPetCode)
                    {
                        row.Cell(1).Value = petCode;
                        row.Cell(2).Value = petName;
                        row.Cell(3).Value = animalType;
                        row.Cell(4).Value = weight;
                        row.Cell(5).Value = birthDate;
                        row.Cell(6).Value = owner;
                        row.Cell(7).Value = chipNumber;
                        row.Cell(8).Value = lastVaccineDate;

                        workbook.Save();
                        return;
                    }
                }
            }

            MessageBox.Show("Pet not found in Excel file");
        }
    }
}