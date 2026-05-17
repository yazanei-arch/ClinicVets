
namespace ClinicVets.UI
{
    partial class AddPetForm
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblPetNameError = new System.Windows.Forms.Label();
            this.txtPetName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblAnimalTypeError = new System.Windows.Forms.Label();
            this.cmbAnimalType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblWeightError = new System.Windows.Forms.Label();
            this.txtWeight = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblBirthDateError = new System.Windows.Forms.Label();
            this.dtpBirthDate = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.lblOwnerError = new System.Windows.Forms.Label();
            this.txtOwner = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.lblChipNumberError = new System.Windows.Forms.Label();
            this.txtChipNumber = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.lblLastVaccineError = new System.Windows.Forms.Label();
            this.dtpLastVaccineDate = new System.Windows.Forms.DateTimePicker();
            this.label8 = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel7.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::ClinicVets.Pets.Properties.Resources.background;
            this.pictureBox1.Location = new System.Drawing.Point(-2, -2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1185, 657);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.label1.Location = new System.Drawing.Point(462, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(221, 45);
            this.label1.TabIndex = 2;
            this.label1.Text = "Add New Pet";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.panel1.Controls.Add(this.lblPetNameError);
            this.panel1.Controls.Add(this.txtPetName);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(599, 167);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(359, 96);
            this.panel1.TabIndex = 3;
            // 
            // lblPetNameError
            // 
            this.lblPetNameError.AutoSize = true;
            this.lblPetNameError.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblPetNameError.ForeColor = System.Drawing.Color.Red;
            this.lblPetNameError.Location = new System.Drawing.Point(139, 62);
            this.lblPetNameError.Name = "lblPetNameError";
            this.lblPetNameError.Size = new System.Drawing.Size(0, 19);
            this.lblPetNameError.TabIndex = 2;
            // 
            // txtPetName
            // 
            this.txtPetName.BackColor = System.Drawing.Color.White;
            this.txtPetName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPetName.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.txtPetName.ForeColor = System.Drawing.Color.Black;
            this.txtPetName.Location = new System.Drawing.Point(139, 17);
            this.txtPetName.Name = "txtPetName";
            this.txtPetName.Size = new System.Drawing.Size(200, 31);
            this.txtPetName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.label2.Location = new System.Drawing.Point(12, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 25);
            this.label2.TabIndex = 0;
            this.label2.Text = "Pet Name";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.panel2.Controls.Add(this.lblAnimalTypeError);
            this.panel2.Controls.Add(this.cmbAnimalType);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(599, 269);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(359, 91);
            this.panel2.TabIndex = 4;
            // 
            // lblAnimalTypeError
            // 
            this.lblAnimalTypeError.AutoSize = true;
            this.lblAnimalTypeError.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblAnimalTypeError.ForeColor = System.Drawing.Color.Red;
            this.lblAnimalTypeError.Location = new System.Drawing.Point(139, 59);
            this.lblAnimalTypeError.Name = "lblAnimalTypeError";
            this.lblAnimalTypeError.Size = new System.Drawing.Size(0, 19);
            this.lblAnimalTypeError.TabIndex = 3;
            // 
            // cmbAnimalType
            // 
            this.cmbAnimalType.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.cmbAnimalType.FormattingEnabled = true;
            this.cmbAnimalType.Location = new System.Drawing.Point(139, 17);
            this.cmbAnimalType.Name = "cmbAnimalType";
            this.cmbAnimalType.Size = new System.Drawing.Size(200, 33);
            this.cmbAnimalType.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.label3.Location = new System.Drawing.Point(12, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 25);
            this.label3.TabIndex = 0;
            this.label3.Text = "Animal Type";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.panel3.Controls.Add(this.lblWeightError);
            this.panel3.Controls.Add(this.txtWeight);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Location = new System.Drawing.Point(599, 374);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(359, 96);
            this.panel3.TabIndex = 5;
            // 
            // lblWeightError
            // 
            this.lblWeightError.AutoSize = true;
            this.lblWeightError.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblWeightError.ForeColor = System.Drawing.Color.Red;
            this.lblWeightError.Location = new System.Drawing.Point(139, 64);
            this.lblWeightError.Name = "lblWeightError";
            this.lblWeightError.Size = new System.Drawing.Size(0, 19);
            this.lblWeightError.TabIndex = 4;
            // 
            // txtWeight
            // 
            this.txtWeight.BackColor = System.Drawing.Color.White;
            this.txtWeight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWeight.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.txtWeight.ForeColor = System.Drawing.Color.Black;
            this.txtWeight.Location = new System.Drawing.Point(139, 17);
            this.txtWeight.Name = "txtWeight";
            this.txtWeight.Size = new System.Drawing.Size(200, 31);
            this.txtWeight.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.label4.Location = new System.Drawing.Point(12, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 25);
            this.label4.TabIndex = 0;
            this.label4.Text = "Weight (kg)";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.panel4.Controls.Add(this.lblBirthDateError);
            this.panel4.Controls.Add(this.dtpBirthDate);
            this.panel4.Controls.Add(this.label5);
            this.panel4.Location = new System.Drawing.Point(29, 276);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(503, 97);
            this.panel4.TabIndex = 6;
            // 
            // lblBirthDateError
            // 
            this.lblBirthDateError.AutoSize = true;
            this.lblBirthDateError.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblBirthDateError.ForeColor = System.Drawing.Color.Red;
            this.lblBirthDateError.Location = new System.Drawing.Point(278, 65);
            this.lblBirthDateError.Name = "lblBirthDateError";
            this.lblBirthDateError.Size = new System.Drawing.Size(0, 19);
            this.lblBirthDateError.TabIndex = 5;
            // 
            // dtpBirthDate
            // 
            this.dtpBirthDate.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.dtpBirthDate.Location = new System.Drawing.Point(176, 15);
            this.dtpBirthDate.Name = "dtpBirthDate";
            this.dtpBirthDate.Size = new System.Drawing.Size(311, 31);
            this.dtpBirthDate.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.label5.Location = new System.Drawing.Point(12, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 25);
            this.label5.TabIndex = 0;
            this.label5.Text = "Birth Date";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.panel5.Controls.Add(this.lblOwnerError);
            this.panel5.Controls.Add(this.txtOwner);
            this.panel5.Controls.Add(this.label6);
            this.panel5.Location = new System.Drawing.Point(29, 391);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(503, 94);
            this.panel5.TabIndex = 7;
            // 
            // lblOwnerError
            // 
            this.lblOwnerError.AutoSize = true;
            this.lblOwnerError.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblOwnerError.ForeColor = System.Drawing.Color.Red;
            this.lblOwnerError.Location = new System.Drawing.Point(278, 60);
            this.lblOwnerError.Name = "lblOwnerError";
            this.lblOwnerError.Size = new System.Drawing.Size(0, 19);
            this.lblOwnerError.TabIndex = 8;
            // 
            // txtOwner
            // 
            this.txtOwner.BackColor = System.Drawing.Color.White;
            this.txtOwner.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtOwner.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.txtOwner.ForeColor = System.Drawing.Color.Black;
            this.txtOwner.Location = new System.Drawing.Point(176, 18);
            this.txtOwner.Name = "txtOwner";
            this.txtOwner.Size = new System.Drawing.Size(311, 31);
            this.txtOwner.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.label6.Location = new System.Drawing.Point(12, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 25);
            this.label6.TabIndex = 0;
            this.label6.Text = "Owner";
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.panel6.Controls.Add(this.lblChipNumberError);
            this.panel6.Controls.Add(this.txtChipNumber);
            this.panel6.Controls.Add(this.label7);
            this.panel6.Location = new System.Drawing.Point(583, 489);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(375, 95);
            this.panel6.TabIndex = 8;
            // 
            // lblChipNumberError
            // 
            this.lblChipNumberError.AutoSize = true;
            this.lblChipNumberError.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblChipNumberError.ForeColor = System.Drawing.Color.Red;
            this.lblChipNumberError.Location = new System.Drawing.Point(139, 60);
            this.lblChipNumberError.Name = "lblChipNumberError";
            this.lblChipNumberError.Size = new System.Drawing.Size(0, 19);
            this.lblChipNumberError.TabIndex = 7;
            // 
            // txtChipNumber
            // 
            this.txtChipNumber.BackColor = System.Drawing.Color.White;
            this.txtChipNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtChipNumber.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.txtChipNumber.ForeColor = System.Drawing.Color.Black;
            this.txtChipNumber.Location = new System.Drawing.Point(159, 17);
            this.txtChipNumber.Name = "txtChipNumber";
            this.txtChipNumber.Size = new System.Drawing.Size(200, 31);
            this.txtChipNumber.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.label7.Location = new System.Drawing.Point(12, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(131, 25);
            this.label7.TabIndex = 0;
            this.label7.Text = "Chip Number";
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(246)))), ((int)(((byte)(249)))));
            this.panel7.Controls.Add(this.lblLastVaccineError);
            this.panel7.Controls.Add(this.dtpLastVaccineDate);
            this.panel7.Controls.Add(this.label8);
            this.panel7.Location = new System.Drawing.Point(29, 167);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(503, 96);
            this.panel7.TabIndex = 9;
            // 
            // lblLastVaccineError
            // 
            this.lblLastVaccineError.AutoSize = true;
            this.lblLastVaccineError.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.lblLastVaccineError.ForeColor = System.Drawing.Color.Red;
            this.lblLastVaccineError.Location = new System.Drawing.Point(278, 62);
            this.lblLastVaccineError.Name = "lblLastVaccineError";
            this.lblLastVaccineError.Size = new System.Drawing.Size(0, 19);
            this.lblLastVaccineError.TabIndex = 6;
            // 
            // dtpLastVaccineDate
            // 
            this.dtpLastVaccineDate.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.dtpLastVaccineDate.Location = new System.Drawing.Point(183, 20);
            this.dtpLastVaccineDate.Name = "dtpLastVaccineDate";
            this.dtpLastVaccineDate.Size = new System.Drawing.Size(305, 31);
            this.dtpLastVaccineDate.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.label8.Location = new System.Drawing.Point(12, 20);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(165, 25);
            this.label8.TabIndex = 0;
            this.label8.Text = "Last Vaccine Date";
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(184)))), ((int)(((byte)(213)))));
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnBack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.btnBack.Location = new System.Drawing.Point(29, 25);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(50, 50);
            this.btnBack.TabIndex = 13;
            this.btnBack.Text = "→";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(184)))), ((int)(((byte)(213)))));
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.btnSave.Location = new System.Drawing.Point(170, 509);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(160, 45);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(184)))), ((int)(((byte)(213)))));
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.btnClear.Location = new System.Drawing.Point(372, 509);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(160, 45);
            this.btnClear.TabIndex = 15;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.label9.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.label9.Location = new System.Drawing.Point(313, 86);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(506, 25);
            this.label9.TabIndex = 25;
            this.label9.Text = "Enter pet details and save a new record in the clinic system";
            // 
            // AddPetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 653);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AddPetForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add New Pet";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtPetName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox cmbAnimalType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox txtWeight;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DateTimePicker dtpBirthDate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TextBox txtOwner;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TextBox txtChipNumber;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.DateTimePicker dtpLastVaccineDate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblPetNameError;
        private System.Windows.Forms.Label lblAnimalTypeError;
        private System.Windows.Forms.Label lblWeightError;
        private System.Windows.Forms.Label lblBirthDateError;
        private System.Windows.Forms.Label lblOwnerError;
        private System.Windows.Forms.Label lblChipNumberError;
        private System.Windows.Forms.Label lblLastVaccineError;
        private System.Windows.Forms.Label label9;
    }
}