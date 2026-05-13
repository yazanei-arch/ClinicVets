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

            PetRepository repo = new PetRepository();
            List<Pet> results = repo.SearchPets(petName, chipNumber);

            if (results.Count == 0)
            {
                MessageBox.Show("No pets found.");
                return;
            }

            foreach (Pet pet in results)
            {
                dgvPets.Rows.Add(
                    pet.PetName,
                    pet.AnimalType,
                    pet.Weight,
                    pet.BirthDate.ToString("yyyy-MM-dd"),
                    pet.Owner,
                    pet.ChipNumber,
                    pet.LastVaccineDate.ToString("yyyy-MM-dd")
                );
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