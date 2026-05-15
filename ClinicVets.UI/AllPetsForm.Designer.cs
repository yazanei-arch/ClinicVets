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
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.colPetCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPetName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBirthDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOwner = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVaccine = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnBack = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.Location = new System.Drawing.Point(587, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(208, 59);
            this.label1.TabIndex = 0;
            this.label1.Text = "All Pets";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPetCode,
            this.colPetName,
            this.colType,
            this.colWeight,
            this.colBirthDate,
            this.colOwner,
            this.colChip,
            this.colVaccine});
            this.dataGridView1.Location = new System.Drawing.Point(68, 231);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.Size = new System.Drawing.Size(1049, 173);
            this.dataGridView1.TabIndex = 1;
            // 
            // colPetCode
            // 
            this.colPetCode.HeaderText = "Pet Code";
            this.colPetCode.MinimumWidth = 8;
            this.colPetCode.Name = "colPetCode";
            this.colPetCode.ReadOnly = true;
            // 
            // colPetName
            // 
            this.colPetName.HeaderText = "Pet Name";
            this.colPetName.MinimumWidth = 8;
            this.colPetName.Name = "colPetName";
            this.colPetName.ReadOnly = true;
            // 
            // colType
            // 
            this.colType.HeaderText = "Animal Type";
            this.colType.MinimumWidth = 8;
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            // 
            // colWeight
            // 
            this.colWeight.HeaderText = "Weight";
            this.colWeight.MinimumWidth = 8;
            this.colWeight.Name = "colWeight";
            this.colWeight.ReadOnly = true;
            // 
            // colBirthDate
            // 
            this.colBirthDate.HeaderText = "Birth Date";
            this.colBirthDate.MinimumWidth = 8;
            this.colBirthDate.Name = "colBirthDate";
            this.colBirthDate.ReadOnly = true;
            // 
            // colOwner
            // 
            this.colOwner.HeaderText = "Owner";
            this.colOwner.MinimumWidth = 8;
            this.colOwner.Name = "colOwner";
            this.colOwner.ReadOnly = true;
            // 
            // colChip
            // 
            this.colChip.HeaderText = "Chip Number";
            this.colChip.MinimumWidth = 8;
            this.colChip.Name = "colChip";
            this.colChip.ReadOnly = true;
            // 
            // colVaccine
            // 
            this.colVaccine.HeaderText = "Last Vaccine";
            this.colVaccine.MinimumWidth = 8;
            this.colVaccine.Name = "colVaccine";
            this.colVaccine.ReadOnly = true;
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnBack.Location = new System.Drawing.Point(1046, 572);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(120, 40);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "Back\r\n";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // AllPetsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::ClinicVets.UI.Properties.Resources.WhatsApp_Image_2026_05_14_at_16_47_35;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1178, 644);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AllPetsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pet Management";
            this.Load += new System.EventHandler(this.AllPetsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void AllPetsForm_Load(object sender, System.EventArgs e)
        {

        }

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPetCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPetName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWeight;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBirthDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOwner;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChip;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVaccine;
        private System.Windows.Forms.Button btnBack;
    }
}