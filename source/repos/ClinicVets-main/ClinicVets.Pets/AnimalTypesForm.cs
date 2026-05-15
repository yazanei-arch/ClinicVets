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


namespace ClinicVets.Pets
{
    public partial class AnimalTypesForm : Form
    {
        string filePath = @"C:\Users\ENTER\OneDrive - ac.sce.ac.il\שולחן העבודה\ClinicVets\ClinicVetsData.xlsx";

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
                var sheet = workbook.Worksheet("AnimalTypes");

                foreach (var row in sheet.RangeUsed().RowsUsed())
                {
                    if (row.RowNumber() == 1)
                        continue;

                    lstTypes.Items.Add(row.Cell(1).GetValue<string>());
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

        private void btnAdd_Click(object sender, EventArgs e)
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (lstTypes.SelectedIndex == -1)
            {
                MessageBox.Show("Select type first");
                return;
            }

            string newType = txtType.Text.Trim();

            if (newType == "")
            {
                MessageBox.Show("Enter new type");
                return;
            }

            string oldType = lstTypes.SelectedItem.ToString();

            if (!oldType.Equals(newType, StringComparison.OrdinalIgnoreCase) && TypeExists(newType))
            {
                MessageBox.Show("Animal type already exists");
                return;
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                var sheet = workbook.Worksheet("AnimalTypes");
                int rowNumber = lstTypes.SelectedIndex + 2;

                sheet.Cell(rowNumber, 1).Value = newType;
                workbook.Save();
            }

            txtType.Clear();
            LoadTypesFromExcel();

            MessageBox.Show("Animal type updated");
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

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            btnUpdate_Click(sender, e);
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {
            btnDelete_Click(sender, e);
        }

        private void btnBack_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}