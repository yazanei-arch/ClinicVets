
namespace ClinicVets.UI
{
    partial class PetManagementForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnAddPet = new ClinicVets.UI.PetActionCard();
            this.btnSearchPet = new ClinicVets.UI.PetActionCard();
            this.btnViewAllPets = new ClinicVets.UI.PetActionCard();
            this.btnAnimalTypes = new ClinicVets.UI.PetActionCard();
            this.btnBack = new ClinicVets.UI.PetBackButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1100, 700);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // btnAddPet
            // 
            this.btnAddPet.FlatAppearance.BorderSize = 0;
            this.btnAddPet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddPet.IconKind = ClinicVets.UI.PetCardIcon.Add;
            this.btnAddPet.Location = new System.Drawing.Point(74, 300);
            this.btnAddPet.Name = "btnAddPet";
            this.btnAddPet.Size = new System.Drawing.Size(220, 168);
            this.btnAddPet.TabIndex = 1;
            this.btnAddPet.Text = "Add Pet";
            this.btnAddPet.UseVisualStyleBackColor = false;
            this.btnAddPet.Click += new System.EventHandler(this.btnAddPet_Click);
            // 
            // btnSearchPet
            // 
            this.btnSearchPet.FlatAppearance.BorderSize = 0;
            this.btnSearchPet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearchPet.IconKind = ClinicVets.UI.PetCardIcon.Search;
            this.btnSearchPet.Location = new System.Drawing.Point(314, 300);
            this.btnSearchPet.Name = "btnSearchPet";
            this.btnSearchPet.Size = new System.Drawing.Size(220, 168);
            this.btnSearchPet.TabIndex = 2;
            this.btnSearchPet.Text = "Search Pet";
            this.btnSearchPet.UseVisualStyleBackColor = false;
            this.btnSearchPet.Click += new System.EventHandler(this.btnSearchPet_Click);
            // 
            // btnViewAllPets
            // 
            this.btnViewAllPets.FlatAppearance.BorderSize = 0;
            this.btnViewAllPets.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewAllPets.IconKind = ClinicVets.UI.PetCardIcon.ViewAll;
            this.btnViewAllPets.Location = new System.Drawing.Point(554, 300);
            this.btnViewAllPets.Name = "btnViewAllPets";
            this.btnViewAllPets.Size = new System.Drawing.Size(220, 168);
            this.btnViewAllPets.TabIndex = 4;
            this.btnViewAllPets.Text = "View All Pets";
            this.btnViewAllPets.UseVisualStyleBackColor = false;
            this.btnViewAllPets.Click += new System.EventHandler(this.btnViewAllPets_Click);
            // 
            // btnAnimalTypes
            // 
            this.btnAnimalTypes.FlatAppearance.BorderSize = 0;
            this.btnAnimalTypes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAnimalTypes.IconKind = ClinicVets.UI.PetCardIcon.AnimalTypes;
            this.btnAnimalTypes.Location = new System.Drawing.Point(794, 300);
            this.btnAnimalTypes.Name = "btnAnimalTypes";
            this.btnAnimalTypes.Size = new System.Drawing.Size(220, 168);
            this.btnAnimalTypes.TabIndex = 5;
            this.btnAnimalTypes.Text = "Animal Types";
            this.btnAnimalTypes.UseVisualStyleBackColor = false;
            this.btnAnimalTypes.Click += new System.EventHandler(this.btnAnimalTypes_Click);
            // 
            // btnBack
            // 
            this.btnBack.FlatAppearance.BorderSize = 0;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Location = new System.Drawing.Point(28, 24);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(112, 40);
            this.btnBack.TabIndex = 3;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // PetManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Controls.Add(this.btnAnimalTypes);
            this.Controls.Add(this.btnViewAllPets);
            this.Controls.Add(this.btnSearchPet);
            this.Controls.Add(this.btnAddPet);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "PetManagementForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pet Management";
            this.Load += new System.EventHandler(this.PetManagementForm_Load);
            this.Resize += new System.EventHandler(this.PetManagementForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private PetActionCard btnAddPet;
        private PetActionCard btnSearchPet;
        private PetActionCard btnViewAllPets;
        private PetActionCard btnAnimalTypes;
        private PetBackButton btnBack;
    }
}
