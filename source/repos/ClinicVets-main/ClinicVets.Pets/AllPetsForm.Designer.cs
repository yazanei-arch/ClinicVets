namespace ClinicVets.UI
{
    partial class AllPetsForm
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
            this.dgvPets = new System.Windows.Forms.DataGridView();
            this.colCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBirth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOwner = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVaccine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnDeletePet = new System.Windows.Forms.Button();
            this.btnUpdatePet = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPets
            // 
            this.dgvPets.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            this.dgvPets.AllowUserToAddRows = false;
            this.dgvPets.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPets.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvPets.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dgvPets.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPets.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCode,
            this.colName,
            this.colType,
            this.colWeight,
            this.colBirth,
            this.colOwner,
            this.colChip,
            this.colVaccine});
            this.dgvPets.Location = new System.Drawing.Point(92, 140);
            this.dgvPets.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvPets.Name = "dgvPets";
            this.dgvPets.RowHeadersWidth = 62;
            this.dgvPets.RowTemplate.Height = 28;
            this.dgvPets.Size = new System.Drawing.Size(906, 311);
            this.dgvPets.TabIndex = 1;
            // 
            // colCode
            // 
            this.colCode.HeaderText = "Pet Code";
            this.colCode.MinimumWidth = 8;
            this.colCode.Name = "colCode";
            // 
            // colName
            // 
            this.colName.HeaderText = "Pet Name";
            this.colName.MinimumWidth = 8;
            this.colName.Name = "colName";
            // 
            // colType
            // 
            this.colType.HeaderText = "Animal Type";
            this.colType.MinimumWidth = 8;
            this.colType.Name = "colType";
            // 
            // colWeight
            // 
            this.colWeight.HeaderText = "Weight";
            this.colWeight.MinimumWidth = 8;
            this.colWeight.Name = "colWeight";
            // 
            // colBirth
            // 
            this.colBirth.HeaderText = "Birth Date";
            this.colBirth.MinimumWidth = 8;
            this.colBirth.Name = "colBirth";
            // 
            // colOwner
            // 
            this.colOwner.HeaderText = "Owner";
            this.colOwner.MinimumWidth = 8;
            this.colOwner.Name = "colOwner";
            // 
            // colChip
            // 
            this.colChip.HeaderText = "Chip Number";
            this.colChip.MinimumWidth = 8;
            this.colChip.Name = "colChip";
            // 
            // colVaccine
            // 
            this.colVaccine.HeaderText = "Last Vaccine";
            this.colVaccine.MinimumWidth = 8;
            this.colVaccine.Name = "colVaccine";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.label2.Location = new System.Drawing.Point(390, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(376, 25);
            this.label2.TabIndex = 23;
            this.label2.Text = "View all registered pets in the clinic system.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.label1.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.label1.Location = new System.Drawing.Point(346, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(443, 45);
            this.label1.TabIndex = 24;
            this.label1.Text = "Animal Types Management";
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(184)))), ((int)(((byte)(213)))));
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnBack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.btnBack.Location = new System.Drawing.Point(45, 31);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(50, 50);
            this.btnBack.TabIndex = 25;
            this.btnBack.Text = "→";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click_1);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ClinicVets.Pets.Properties.Resources.background;
            this.pictureBox1.Location = new System.Drawing.Point(0, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1185, 652);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 26;
            this.pictureBox1.TabStop = false;
            // 
            // btnDeletePet
            // 
            this.btnDeletePet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(184)))), ((int)(((byte)(213)))));
            this.btnDeletePet.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnDeletePet.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.btnDeletePet.Location = new System.Drawing.Point(629, 475);
            this.btnDeletePet.Name = "btnDeletePet";
            this.btnDeletePet.Size = new System.Drawing.Size(160, 45);
            this.btnDeletePet.TabIndex = 27;
            this.btnDeletePet.Text = "Delete Pet";
            this.btnDeletePet.UseVisualStyleBackColor = false;
            this.btnDeletePet.Click += new System.EventHandler(this.btnDeletePet_Click);
            // 
            // btnUpdatePet
            // 
            this.btnUpdatePet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(184)))), ((int)(((byte)(213)))));
            this.btnUpdatePet.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnUpdatePet.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.btnUpdatePet.Location = new System.Drawing.Point(354, 475);
            this.btnUpdatePet.Name = "btnUpdatePet";
            this.btnUpdatePet.Size = new System.Drawing.Size(160, 45);
            this.btnUpdatePet.TabIndex = 28;
            this.btnUpdatePet.Text = "Update Pet";
            this.btnUpdatePet.UseVisualStyleBackColor = false;
            this.btnUpdatePet.Click += new System.EventHandler(this.btnUpdatePet_Click);
            // 
            // AllPetsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::ClinicVets.Pets.Properties.Resources.background;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1182, 653);
            this.Controls.Add(this.btnUpdatePet);
            this.Controls.Add(this.btnDeletePet);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dgvPets);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimizeBox = false;
            this.Name = "AllPetsForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AllPetsForm";
            this.Load += new System.EventHandler(this.AllPetsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvPets;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWeight;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBirth;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOwner;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChip;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVaccine;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnDeletePet;
        private System.Windows.Forms.Button btnUpdatePet;
    }
}