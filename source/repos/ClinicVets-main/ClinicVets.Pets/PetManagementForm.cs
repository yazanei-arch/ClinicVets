using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClinicVets.UI
{
    public partial class PetManagementForm : Form
    {
        public PetManagementForm()
        {
            InitializeComponent();
        }

        private void btnAddPet_Click(object sender, EventArgs e)
        {
            AddPetForm form = new AddPetForm();
            form.ShowDialog();
            this.Hide();
        }

        private void btnSearchPet_Click(object sender, EventArgs e)
        {
            SearchPetForm form = new SearchPetForm();
            form.ShowDialog();
            this.Hide();
        }

        private void btnShowAllPets_Click(object sender, EventArgs e)
        {
            
        }

        private void btnAnimalTypes_Click(object sender, EventArgs e)
        {
            
        }
    }
}