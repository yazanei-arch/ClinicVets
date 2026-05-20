using System.IO;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ClinicVets.UI;

namespace ClinicVets.UI
{
    public partial class SearchPetForm : Form
    {
        public SearchPetForm()
        {
            InitializeComponent();
            SetupDataGridView();
        }

        private void SetupDataGridView()
        {
            dgvPets.Columns.Clear();

            dgvPets.ColumnCount = 7;
            dgvPets.Columns[0].Name = "Pet Name";
            dgvPets.Columns[1].Name = "Animal Type";
            dgvPets.Columns[2].Name = "Weight";
            dgvPets.Columns[3].Name = "Birth Date";
            dgvPets.Columns[4].Name = "Owner";
            dgvPets.Columns[5].Name = "Chip Number";
            dgvPets.Columns[6].Name = "Last Vaccine Date";

            dgvPets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPets.ReadOnly = true;
            dgvPets.AllowUserToAddRows = false;
            dgvPets.AllowUserToDeleteRows = false;
            dgvPets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPets.MultiSelect = false;
            dgvPets.RowHeadersVisible = false;

            dgvPets.EnableHeadersVisualStyles = false;
            dgvPets.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgvPets.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPets.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvPets.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgvPets.DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue;
            dgvPets.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        private void btnClear2_Click(object sender, EventArgs e)
        {
            txtPetName2.Clear();
            txtChipNumber2.Clear();
            dgvPets.Rows.Clear();
        }

        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            string petName = txtPetName2.Text.Trim();
            string chipNumber = txtChipNumber2.Text.Trim();

            if (string.IsNullOrWhiteSpace(petName) && string.IsNullOrWhiteSpace(chipNumber))
            {
                MessageBox.Show("Please enter Pet Name or Chip Number.");
                return;
            }

            dgvPets.Rows.Clear();

            string filePath = ExcelFileManager.FilePath;

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Excel file not found");
                return;
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                if (!workbook.Worksheets.Contains("Pets"))
                {
                    MessageBox.Show("Pets sheet not found.");
                    return;
                }

                var sheet = workbook.Worksheet("Pets");
                var range = sheet.RangeUsed();

                if (range == null)
                {
                    MessageBox.Show("No pets found.");
                    return;
                }

                bool found = false;

                foreach (var row in range.RowsUsed())
                {
                    if (row.RowNumber() == 1)
                        continue;

                    string excelPetName = row.Cell(2).GetValue<string>().Trim();
                    string excelAnimalType = row.Cell(3).GetValue<string>().Trim();
                    string excelWeight = row.Cell(4).GetValue<string>().Trim();
                    string excelBirthDate = row.Cell(5).GetValue<string>().Trim();
                    string excelOwner = row.Cell(6).GetValue<string>().Trim();
                    string excelChipNumber = row.Cell(7).GetValue<string>().Trim();
                    string excelLastVaccineDate = row.Cell(8).GetValue<string>().Trim();

                    bool matchByName =
                        !string.IsNullOrWhiteSpace(petName) &&
                        excelPetName.ToLower().Contains(petName.ToLower());

                    bool matchByChip =
                        !string.IsNullOrWhiteSpace(chipNumber) &&
                        excelChipNumber.ToLower().Contains(chipNumber.ToLower());

                    if (matchByName || matchByChip)
                    {
                        dgvPets.Rows.Add(
                            excelPetName,
                            excelAnimalType,
                            excelWeight,
                            excelBirthDate,
                            excelOwner,
                            excelChipNumber,
                            excelLastVaccineDate
                        );

                        found = true;
                    }
                }

                if (!found)
                {
                    MessageBox.Show("No pets found.");
                }
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