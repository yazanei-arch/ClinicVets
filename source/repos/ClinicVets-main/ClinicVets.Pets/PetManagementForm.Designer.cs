
namespace ClinicVets.UI
{
    partial class PetManagementForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.btnAddPet = new System.Windows.Forms.Button();
            this.btnSearchPet = new System.Windows.Forms.Button();
            this.btnShowAllPets = new System.Windows.Forms.Button();
            this.btnAnimalTypes = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(404, 209);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(495, 54);
            this.label1.TabIndex = 1;
            this.label1.Text = "Pet Management System";
            // 
            // btnAddPet
            // 
            this.btnAddPet.BackColor = System.Drawing.Color.Transparent;
            this.btnAddPet.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnAddPet.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btnAddPet.Location = new System.Drawing.Point(985, 345);
            this.btnAddPet.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAddPet.Name = "btnAddPet";
            this.btnAddPet.Size = new System.Drawing.Size(248, 62);
            this.btnAddPet.TabIndex = 2;
            this.btnAddPet.Text = "Add New Pet";
            this.btnAddPet.UseVisualStyleBackColor = false;
            this.btnAddPet.Click += new System.EventHandler(this.btnAddPet_Click);
            // 
            // btnSearchPet
            // 
            this.btnSearchPet.BackColor = System.Drawing.Color.Transparent;
            this.btnSearchPet.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnSearchPet.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btnSearchPet.Location = new System.Drawing.Point(678, 345);
            this.btnSearchPet.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSearchPet.Name = "btnSearchPet";
            this.btnSearchPet.Size = new System.Drawing.Size(248, 62);
            this.btnSearchPet.TabIndex = 3;
            this.btnSearchPet.Text = "Search Pet";
            this.btnSearchPet.UseVisualStyleBackColor = false;
            this.btnSearchPet.Click += new System.EventHandler(this.btnSearchPet_Click);
            // 
            // btnShowAllPets
            // 
            this.btnShowAllPets.BackColor = System.Drawing.Color.Transparent;
            this.btnShowAllPets.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnShowAllPets.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btnShowAllPets.Location = new System.Drawing.Point(359, 345);
            this.btnShowAllPets.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnShowAllPets.Name = "btnShowAllPets";
            this.btnShowAllPets.Size = new System.Drawing.Size(248, 62);
            this.btnShowAllPets.TabIndex = 4;
            this.btnShowAllPets.Text = "Show All Pets";
            this.btnShowAllPets.UseVisualStyleBackColor = false;
            this.btnShowAllPets.Click += new System.EventHandler(this.btnShowAllPets_Click);
            // 
            // btnAnimalTypes
            // 
            this.btnAnimalTypes.BackColor = System.Drawing.Color.Transparent;
            this.btnAnimalTypes.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnAnimalTypes.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btnAnimalTypes.Location = new System.Drawing.Point(47, 345);
            this.btnAnimalTypes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAnimalTypes.Name = "btnAnimalTypes";
            this.btnAnimalTypes.Size = new System.Drawing.Size(248, 62);
            this.btnAnimalTypes.TabIndex = 5;
            this.btnAnimalTypes.Text = "Animal Types Catalog";
            this.btnAnimalTypes.UseVisualStyleBackColor = false;
            this.btnAnimalTypes.Click += new System.EventHandler(this.btnAnimalTypes_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ClinicVets.Pets.Properties.Resources.background;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1332, 816);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(167, 273);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(932, 30);
            this.label2.TabIndex = 24;
            this.label2.Text = "Manage pets, search records, view registered pets, and control animal types in th" +
    "e clinic system";
            // 
            // PetManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1330, 816);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnAnimalTypes);
            this.Controls.Add(this.btnShowAllPets);
            this.Controls.Add(this.btnSearchPet);
            this.Controls.Add(this.btnAddPet);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "PetManagementForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pet Management";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAddPet;
        private System.Windows.Forms.Button btnSearchPet;
        private System.Windows.Forms.Button btnShowAllPets;
        private System.Windows.Forms.Button btnAnimalTypes;
        private System.Windows.Forms.Label label2;
    }
}

