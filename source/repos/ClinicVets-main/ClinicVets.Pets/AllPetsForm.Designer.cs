namespace ClinicVets.Pets
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
            this.label1 = new System.Windows.Forms.Label();
            this.dgvPets = new System.Windows.Forms.DataGridView();
            this.colCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBirth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOwner = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVaccine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnBack = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPets)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.Location = new System.Drawing.Point(465, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 46);
            this.label1.TabIndex = 0;
            this.label1.Text = "All Pets ";
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
            this.dgvPets.Location = new System.Drawing.Point(28, 177);
            this.dgvPets.Name = "dgvPets";
            this.dgvPets.RowHeadersWidth = 62;
            this.dgvPets.RowTemplate.Height = 28;
            this.dgvPets.Size = new System.Drawing.Size(1019, 389);
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
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnBack.Location = new System.Drawing.Point(941, 45);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(92, 41);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // AllPetsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::ClinicVets.Pets.Properties.Resources.WhatsApp_Image_2026_05_14_at_16_47_35;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1178, 644);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.dgvPets);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.Name = "AllPetsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AllPetsForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvPets)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvPets;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWeight;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBirth;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOwner;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChip;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVaccine;
        private System.Windows.Forms.Button btnBack;
    }
}