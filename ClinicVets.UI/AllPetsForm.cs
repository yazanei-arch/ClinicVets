using System;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace ClinicVets.UI
{
    public partial class AllPetsForm : Form
    {
        public AllPetsForm()
        {
            InitializeComponent();

            SetupTable();
            LoadPets();

            dataGridView1.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView1.Height =
                (dataGridView1.Rows.Count *
                 dataGridView1.RowTemplate.Height)
                 + dataGridView1.ColumnHeadersHeight + 20;
        }

        private void SetupTable()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("PetID", "Pet ID");
            dataGridView1.Columns.Add("PetName", "Pet Name");
            dataGridView1.Columns.Add("AnimalType", "Animal Type");
            dataGridView1.Columns.Add("Weight", "Weight");
            dataGridView1.Columns.Add("BirthDate", "Birth Date");
            dataGridView1.Columns.Add("ChipNumber", "Chip Number");
            dataGridView1.Columns.Add("OwnerID", "Owner ID");
            dataGridView1.Columns.Add("LastVaccineDate", "Last Vaccine Date");
        }

        private void LoadPets()
        {
            dataGridView1.Rows.Clear();

            string path =
           
             @"C:\Users\ENTER\source\repos\ClinicVets\ClinicVetsData.xlsx";

            using (var workbook = new XLWorkbook(path))
            {
                var worksheet = workbook.Worksheet("Pets");

                var rows = worksheet
                    .RangeUsed()
                    .RowsUsed()
                    .Skip(1);

                foreach (var row in rows)
                {
                    dataGridView1.Rows.Add(
                        row.Cell(1).GetValue<string>(),
                        row.Cell(2).GetValue<string>(),
                        row.Cell(3).GetValue<string>(),
                        row.Cell(4).GetValue<string>(),
                        row.Cell(5).GetValue<string>(),
                        row.Cell(6).GetValue<string>(),
                        row.Cell(7).GetValue<string>(),
                        row.Cell(8).GetValue<string>()
                    );
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}