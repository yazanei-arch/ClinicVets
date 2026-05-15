using System;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace ClinicVets.UI
{
    public partial class AnimalTypesForm : Form
    {
        string path =
        @"C:\Users\ENTER\source\repos\ClinicVets\ClinicVetsData.xlsx";

        public AnimalTypesForm()
        {
            InitializeComponent();
            LoadTypes();
        }

        private void LoadTypes()
        {
            lstTypes.Items.Clear();

            using (var workbook = new XLWorkbook(path))
            {
                var worksheet =
                    workbook.Worksheet("AnimalTypes");

                var rows =
                    worksheet.RangeUsed()
                    .RowsUsed()
                    .Skip(1);

                foreach (var row in rows)
                {
                    lstTypes.Items.Add(
                        row.Cell(1).GetValue<string>()
                    );
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string type = txtType.Text.Trim();

            if (type == "")
            {
                MessageBox.Show(
                    "Please enter animal type"
                );
                return;
            }

            bool exists = false;

            foreach (var item in lstTypes.Items)
            {
                if (item.ToString().ToLower() ==
                    type.ToLower())
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
            {
                MessageBox.Show(
                    "This type already exists"
                );
                return;
            }

            using (var workbook = new XLWorkbook(path))
            {
                var worksheet =
                    workbook.Worksheet("AnimalTypes");

                int lastRow =
                    worksheet.LastRowUsed()
                    .RowNumber() + 1;

                worksheet.Cell(lastRow, 1).Value =
                    type;

                workbook.Save();
            }

            LoadTypes();

            txtType.Clear();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (lstTypes.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Please select animal type"
                );
                return;
            }

            string newType =
                txtType.Text.Trim();

            if (newType == "")
            {
                MessageBox.Show(
                    "Please enter new animal type"
                );
                return;
            }

            bool exists = false;

            foreach (var item in lstTypes.Items)
            {
                if (
                    item.ToString().ToLower()
                    == newType.ToLower()

                    &&

                    item.ToString().ToLower()
                    != lstTypes.SelectedItem
                    .ToString().ToLower()
                   )
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
            {
                MessageBox.Show(
                    "This type already exists"
                );
                return;
            }

            using (var workbook = new XLWorkbook(path))
            {
                var worksheet =
                    workbook.Worksheet("AnimalTypes");

                int row =
                    lstTypes.SelectedIndex + 2;

                worksheet.Cell(row, 1).Value =
                    newType;

                workbook.Save();
            }

            LoadTypes();

            txtType.Clear();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstTypes.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Please select animal type"
                );
                return;
            }

            using (var workbook = new XLWorkbook(path))
            {
                var worksheet =
                    workbook.Worksheet("AnimalTypes");

                int row =
                    lstTypes.SelectedIndex + 2;

                worksheet.Row(row).Delete();

                workbook.Save();
            }

            LoadTypes();

            txtType.Clear();
        }

        private void lstTypes_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if (lstTypes.SelectedIndex != -1)
            {
                txtType.Text =
                    lstTypes.SelectedItem
                    .ToString();
            }
        }

        private void btnBack_Click(
            object sender,
            EventArgs e)
        {
            this.Close();
        }

        private void AnimalTypesForm_Load(
            object sender,
            EventArgs e)
        {
        }

        private void label2_Click(
            object sender,
            EventArgs e)
        {
        }
    }
}