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
    public partial class AllPetsForm : Form
    {
        string filePath = @"C:\Users\ENTER\OneDrive - ac.sce.ac.il\שולחן העבודה\ClinicVets\ClinicVetsData.xlsx";

        public AllPetsForm()
        {
            InitializeComponent();
            LoadPetsFromExcel();
        }

        private void LoadPetsFromExcel()
        {
            dgvPets.Rows.Clear();

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Excel file not found");
                return;
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                var sheet = workbook.Worksheet("Pets");

                foreach (var row in sheet.RangeUsed().RowsUsed())
                {
                    if (row.RowNumber() == 1)
                        continue;

                    dgvPets.Rows.Add(
                        row.Cell(1).GetValue<string>(),
                        row.Cell(2).GetValue<string>(),
                        row.Cell(3).GetValue<string>(),
                        row.Cell(4).GetValue<string>(),
                        row.Cell(5).GetDateTime(),
                        row.Cell(6).GetValue<string>(),
                        row.Cell(7).GetValue<string>(),
                        row.Cell(8).GetDateTime()
                    );
                }
            }

            dgvPets.Columns[4].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvPets.Columns[7].DefaultCellStyle.Format = "dd/MM/yyyy";

            dgvPets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}