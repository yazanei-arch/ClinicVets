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
    public partial class AnimalTypesForm : Form
    {
        private readonly string filePath = ExcelFileManager.FilePath;

        public AnimalTypesForm()
        {
            InitializeComponent();
            LoadTypesFromExcel();
        }

        private void LoadTypesFromExcel()
        {
            lstTypes.Items.Clear();

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

                    string type = row.Cell(1).GetValue<string>();

                    if (!string.IsNullOrWhiteSpace(type))
                        lstTypes.Items.Add(type);
                }
            }
        }

        private bool TypeExists(string type)
        {
            foreach (var item in lstTypes.Items)
            {
                if (item.ToString().Trim().ToLower() == type.Trim().ToLower())
                    return true;
            }

            return false;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (lstTypes.SelectedItem == null)
            {
                MessageBox.Show("Please select animal type to update.");
                return;
            }

            string oldType = lstTypes.SelectedItem.ToString();
            string newType = txtType.Text.Trim();

            if (string.IsNullOrWhiteSpace(newType))
            {
                MessageBox.Show("Please enter new animal type.");
                return;
            }

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Excel file not found");
                return;
            }

            try
            {
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

                        string currentType = row.Cell(1).GetValue<string>().Trim();

                        if (currentType.Equals(oldType, StringComparison.OrdinalIgnoreCase))
                        {
                            row.Cell(1).Value = newType;
                            break;
                        }
                    }

                    workbook.Save();
                }

                // כאן העדכון החשוב:
                UpdatePetsAnimalTypeInExcel(oldType, newType);

                txtType.Clear();
                LoadTypesFromExcel();

                MessageBox.Show("Animal type updated successfully.");
            }
            catch (IOException)
            {
                MessageBox.Show("Please close the Excel file before updating.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while updating animal type: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstTypes.SelectedIndex == -1)
            {
                MessageBox.Show("Select type first");
                return;
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                var sheet = workbook.Worksheet("AnimalTypes");
                int rowNumber = lstTypes.SelectedIndex + 2;

                sheet.Row(rowNumber).Delete();
                workbook.Save();
            }

            txtType.Clear();
            LoadTypesFromExcel();

            MessageBox.Show("Animal type deleted");
        }

        private void lstTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTypes.SelectedItem != null)
                txtType.Text = lstTypes.SelectedItem.ToString();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            PetManagementForm form = new PetManagementForm();
            form.ShowDialog();
            this.Hide();
        }

        private void btnDelete_Click_2(object sender, EventArgs e)
        {
            btnDelete_Click(sender, e);
        }

        private void btnUpdate_Click_2(object sender, EventArgs e)
        {
            btnUpdate_Click(sender, e);
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            string type = txtType.Text.Trim();

            if (type == "")
            {
                MessageBox.Show("Enter animal type");
                return;
            }

            if (TypeExists(type))
            {
                MessageBox.Show("Animal type already exists");
                return;
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                var sheet = workbook.Worksheet("AnimalTypes");
                int lastRow = sheet.LastRowUsed().RowNumber() + 1;

                sheet.Cell(lastRow, 1).Value = type;
                workbook.Save();
            }

            txtType.Clear();
            LoadTypesFromExcel();

            MessageBox.Show("Animal type added");
        }

        private void UpdatePetsAnimalTypeInExcel(string oldType, string newType)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Excel file not found");
                return;
            }

            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    if (!workbook.Worksheets.Contains("Pets"))
                        return;

                    var petsSheet = workbook.Worksheet("Pets");
                    var range = petsSheet.RangeUsed();

                    if (range == null)
                        return;

                    foreach (var row in range.RowsUsed())
                    {
                        if (row.RowNumber() == 1)
                            continue;

                        // לפי הסדר שלנו:
                        // A = PetID
                        // B = PetName
                        // C = AnimalType
                        string currentAnimalType = row.Cell(3).GetValue<string>().Trim();

                        if (currentAnimalType.Equals(oldType, StringComparison.OrdinalIgnoreCase))
                        {
                            row.Cell(3).Value = newType;
                        }
                    }

                    workbook.Save();
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Please close the Excel file before updating.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while updating pets animal type: " + ex.Message);
            }
        }
    }
}