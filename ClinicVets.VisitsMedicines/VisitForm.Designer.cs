namespace ClinicVets.VisitsMedicines
{
    partial class VisitForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VisitForm));
            cmbPets = new System.Windows.Forms.ComboBox();
            rtbSummary = new System.Windows.Forms.RichTextBox();
            dtpVisitDate = new System.Windows.Forms.DateTimePicker();
            lblVaccineAlert = new System.Windows.Forms.Label();
            clbMedicines = new System.Windows.Forms.CheckedListBox();
            lblTotalCost = new System.Windows.Forms.Label();
            btnSaveVisit = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            txtReason = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            txtVetName = new System.Windows.Forms.TextBox();
            dtpVisitTime = new System.Windows.Forms.DateTimePicker();
            lblPetError = new System.Windows.Forms.Label();
            lblVetEmpty = new System.Windows.Forms.Label();
            lblReasonError = new System.Windows.Forms.Label();
            lblTimeError = new System.Windows.Forms.Label();
            lblVetInvalid = new System.Windows.Forms.Label();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // cmbPets
            // 
            cmbPets.FormattingEnabled = true;
            cmbPets.Location = new System.Drawing.Point(518, 171);
            cmbPets.Name = "cmbPets";
            cmbPets.Size = new System.Drawing.Size(151, 28);
            cmbPets.TabIndex = 1;
            cmbPets.SelectedIndexChanged += cmbPets_SelectedIndexChanged;
            // 
            // rtbSummary
            // 
            rtbSummary.Location = new System.Drawing.Point(519, 365);
            rtbSummary.Name = "rtbSummary";
            rtbSummary.Size = new System.Drawing.Size(431, 67);
            rtbSummary.TabIndex = 2;
            rtbSummary.Text = "";
            // 
            // dtpVisitDate
            // 
            dtpVisitDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            dtpVisitDate.Location = new System.Drawing.Point(519, 235);
            dtpVisitDate.Name = "dtpVisitDate";
            dtpVisitDate.Size = new System.Drawing.Size(106, 27);
            dtpVisitDate.TabIndex = 3;
            // 
            // lblVaccineAlert
            // 
            lblVaccineAlert.AutoSize = true;
            lblVaccineAlert.BackColor = System.Drawing.Color.White;
            lblVaccineAlert.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblVaccineAlert.ForeColor = System.Drawing.Color.Red;
            lblVaccineAlert.Location = new System.Drawing.Point(710, 174);
            lblVaccineAlert.Name = "lblVaccineAlert";
            lblVaccineAlert.Size = new System.Drawing.Size(187, 20);
            lblVaccineAlert.TabIndex = 4;
            lblVaccineAlert.Text = "Annual Vaccine Required!";
            lblVaccineAlert.Visible = false;
            // 
            // clbMedicines
            // 
            clbMedicines.FormattingEnabled = true;
            clbMedicines.Location = new System.Drawing.Point(518, 463);
            clbMedicines.Name = "clbMedicines";
            clbMedicines.Size = new System.Drawing.Size(150, 70);
            clbMedicines.TabIndex = 5;
            clbMedicines.ItemCheck += clbMedicines_ItemCheck;
            // 
            // lblTotalCost
            // 
            lblTotalCost.AutoSize = true;
            lblTotalCost.BackColor = System.Drawing.Color.FromArgb(243, 247, 250);
            lblTotalCost.Location = new System.Drawing.Point(540, 626);
            lblTotalCost.Name = "lblTotalCost";
            lblTotalCost.Size = new System.Drawing.Size(133, 20);
            lblTotalCost.TabIndex = 6;
            lblTotalCost.Text = "Total Cost: 100 NIS";
            // 
            // btnSaveVisit
            // 
            btnSaveVisit.BackColor = System.Drawing.Color.FromArgb(225, 234, 241);
            btnSaveVisit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(158, 197, 218);
            btnSaveVisit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btnSaveVisit.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnSaveVisit.ForeColor = System.Drawing.Color.FromArgb(53, 93, 136);
            btnSaveVisit.Location = new System.Drawing.Point(308, 617);
            btnSaveVisit.Name = "btnSaveVisit";
            btnSaveVisit.Size = new System.Drawing.Size(185, 36);
            btnSaveVisit.TabIndex = 7;
            btnSaveVisit.Text = "Finish and Save Visit";
            btnSaveVisit.UseVisualStyleBackColor = false;
            btnSaveVisit.Click += btnSaveVisit_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = System.Drawing.Color.FromArgb(243, 247, 250);
            label2.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label2.ForeColor = System.Drawing.Color.FromArgb(55, 97, 139);
            label2.Location = new System.Drawing.Point(308, 171);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(93, 23);
            label2.TabIndex = 9;
            label2.Text = "Select Pet:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = System.Drawing.Color.FromArgb(243, 247, 250);
            label3.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label3.ForeColor = System.Drawing.Color.FromArgb(55, 97, 139);
            label3.Location = new System.Drawing.Point(308, 235);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(186, 23);
            label3.TabIndex = 10;
            label3.Text = "Select Date and Time:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = System.Drawing.Color.FromArgb(243, 247, 250);
            label4.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label4.ForeColor = System.Drawing.Color.FromArgb(55, 97, 139);
            label4.Location = new System.Drawing.Point(308, 366);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(133, 23);
            label4.TabIndex = 11;
            label4.Text = "Visit Summary:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = System.Drawing.Color.FromArgb(243, 247, 250);
            label5.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label5.ForeColor = System.Drawing.Color.FromArgb(55, 97, 139);
            label5.Location = new System.Drawing.Point(308, 463);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(150, 23);
            label5.TabIndex = 12;
            label5.Text = "Choose Medicine:";
            // 
            // txtReason
            // 
            txtReason.Location = new System.Drawing.Point(519, 297);
            txtReason.Name = "txtReason";
            txtReason.Size = new System.Drawing.Size(150, 27);
            txtReason.TabIndex = 13;
            txtReason.Leave += txtReason_Leave;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = System.Drawing.Color.FromArgb(243, 247, 250);
            label6.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label6.ForeColor = System.Drawing.Color.FromArgb(55, 97, 139);
            label6.Location = new System.Drawing.Point(308, 297);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(111, 23);
            label6.TabIndex = 14;
            label6.Text = "Visit Reason:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BackColor = System.Drawing.Color.Transparent;
            label7.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label7.ForeColor = System.Drawing.Color.FromArgb(55, 97, 139);
            label7.Location = new System.Drawing.Point(308, 555);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(93, 23);
            label7.TabIndex = 15;
            label7.Text = "Vet Name:";
            // 
            // txtVetName
            // 
            txtVetName.Location = new System.Drawing.Point(518, 555);
            txtVetName.Name = "txtVetName";
            txtVetName.Size = new System.Drawing.Size(150, 27);
            txtVetName.TabIndex = 16;
            txtVetName.Leave += txtVetName_Leave;
            // 
            // dtpVisitTime
            // 
            dtpVisitTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            dtpVisitTime.Location = new System.Drawing.Point(648, 235);
            dtpVisitTime.Name = "dtpVisitTime";
            dtpVisitTime.ShowUpDown = true;
            dtpVisitTime.Size = new System.Drawing.Size(91, 27);
            dtpVisitTime.TabIndex = 17;
            dtpVisitTime.Leave += dtpVisitTime_Leave;
            // 
            // lblPetError
            // 
            lblPetError.AutoSize = true;
            lblPetError.BackColor = System.Drawing.Color.White;
            lblPetError.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblPetError.ForeColor = System.Drawing.Color.Red;
            lblPetError.Location = new System.Drawing.Point(518, 202);
            lblPetError.Name = "lblPetError";
            lblPetError.Size = new System.Drawing.Size(121, 17);
            lblPetError.TabIndex = 18;
            lblPetError.Text = "Please select a pet";
            lblPetError.Visible = false;
            // 
            // lblVetEmpty
            // 
            lblVetEmpty.AutoSize = true;
            lblVetEmpty.BackColor = System.Drawing.Color.White;
            lblVetEmpty.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblVetEmpty.ForeColor = System.Drawing.Color.Red;
            lblVetEmpty.Location = new System.Drawing.Point(519, 585);
            lblVetEmpty.Name = "lblVetEmpty";
            lblVetEmpty.Size = new System.Drawing.Size(174, 17);
            lblVetEmpty.TabIndex = 19;
            lblVetEmpty.Text = "Vet name cannot be empty";
            lblVetEmpty.Visible = false;
            // 
            // lblReasonError
            // 
            lblReasonError.AutoSize = true;
            lblReasonError.BackColor = System.Drawing.Color.White;
            lblReasonError.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblReasonError.ForeColor = System.Drawing.Color.Red;
            lblReasonError.Location = new System.Drawing.Point(519, 327);
            lblReasonError.Name = "lblReasonError";
            lblReasonError.Size = new System.Drawing.Size(215, 17);
            lblReasonError.TabIndex = 20;
            lblReasonError.Text = "Please enter a reason for the visit";
            lblReasonError.Visible = false;
            // 
            // lblTimeError
            // 
            lblTimeError.AutoSize = true;
            lblTimeError.BackColor = System.Drawing.Color.White;
            lblTimeError.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblTimeError.ForeColor = System.Drawing.Color.Red;
            lblTimeError.Location = new System.Drawing.Point(648, 265);
            lblTimeError.Name = "lblTimeError";
            lblTimeError.Size = new System.Drawing.Size(176, 17);
            lblTimeError.TabIndex = 21;
            lblTimeError.Text = "Cannot select a future time";
            lblTimeError.Visible = false;
            // 
            // lblVetInvalid
            // 
            lblVetInvalid.AutoSize = true;
            lblVetInvalid.BackColor = System.Drawing.Color.White;
            lblVetInvalid.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            lblVetInvalid.ForeColor = System.Drawing.Color.Red;
            lblVetInvalid.Location = new System.Drawing.Point(519, 585);
            lblVetInvalid.Name = "lblVetInvalid";
            lblVetInvalid.Size = new System.Drawing.Size(264, 17);
            lblVetInvalid.TabIndex = 22;
            lblVetInvalid.Text = "Name can only contain letters and spaces";
            lblVetInvalid.Visible = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (System.Drawing.Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new System.Drawing.Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(1209, 727);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // VisitForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(1209, 726);
            Controls.Add(lblVetInvalid);
            Controls.Add(lblTimeError);
            Controls.Add(lblReasonError);
            Controls.Add(lblVetEmpty);
            Controls.Add(lblPetError);
            Controls.Add(dtpVisitTime);
            Controls.Add(txtVetName);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(txtReason);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(btnSaveVisit);
            Controls.Add(lblTotalCost);
            Controls.Add(clbMedicines);
            Controls.Add(lblVaccineAlert);
            Controls.Add(dtpVisitDate);
            Controls.Add(rtbSummary);
            Controls.Add(cmbPets);
            Controls.Add(pictureBox1);
            MaximizeBox = false;
            Name = "VisitForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Visit Management";
            Load += VisitForm_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.ComboBox cmbPets;
        private System.Windows.Forms.RichTextBox rtbSummary;
        private System.Windows.Forms.DateTimePicker dtpVisitDate;
        private System.Windows.Forms.Label lblVaccineAlert;
        private System.Windows.Forms.CheckedListBox clbMedicines;
        private System.Windows.Forms.Label lblTotalCost;
        private System.Windows.Forms.Button btnSaveVisit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtReason;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtVetName;
        private System.Windows.Forms.DateTimePicker dtpVisitTime;
        private System.Windows.Forms.Label lblPetError;
        private System.Windows.Forms.Label lblVetEmpty;
        private System.Windows.Forms.Label lblReasonError;
        private System.Windows.Forms.Label lblTimeError;
        private System.Windows.Forms.Label lblVetInvalid;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}