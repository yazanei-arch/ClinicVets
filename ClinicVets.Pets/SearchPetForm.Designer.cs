
namespace ClinicVets.UI
{
    partial class SearchPetForm
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
            this.txtPetName2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtChipNumber2 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.dgvPets = new System.Windows.Forms.DataGridView();
            this.btnClear2 = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPets)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ClinicVets.Pets.Properties.Resources.background;
            this.pictureBox1.Location = new System.Drawing.Point(-1, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1330, 819);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.label1.Location = new System.Drawing.Point(613, 183);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 28);
            this.label1.TabIndex = 3;
            this.label1.Text = "Search Pet";
            // 
            // txtPetName2
            // 
            this.txtPetName2.BackColor = System.Drawing.Color.White;
            this.txtPetName2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPetName2.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.txtPetName2.ForeColor = System.Drawing.Color.Black;
            this.txtPetName2.Location = new System.Drawing.Point(696, 258);
            this.txtPetName2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtPetName2.Name = "txtPetName2";
            this.txtPetName2.Size = new System.Drawing.Size(225, 36);
            this.txtPetName2.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label2.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label2.Location = new System.Drawing.Point(927, 260);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 30);
            this.label2.TabIndex = 0;
            this.label2.Text = "Pet Name";
            // 
            // txtChipNumber2
            // 
            this.txtChipNumber2.BackColor = System.Drawing.Color.White;
            this.txtChipNumber2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtChipNumber2.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.txtChipNumber2.ForeColor = System.Drawing.Color.Black;
            this.txtChipNumber2.Location = new System.Drawing.Point(248, 254);
            this.txtChipNumber2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtChipNumber2.Name = "txtChipNumber2";
            this.txtChipNumber2.Size = new System.Drawing.Size(225, 36);
            this.txtChipNumber2.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label7.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.label7.Location = new System.Drawing.Point(479, 258);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(151, 30);
            this.label7.TabIndex = 0;
            this.label7.Text = "Chip Number";
            // 
            // dgvPets
            // 
            this.dgvPets.AllowUserToAddRows = false;
            this.dgvPets.AllowUserToDeleteRows = false;
            this.dgvPets.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPets.BackgroundColor = System.Drawing.Color.White;
            this.dgvPets.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPets.Location = new System.Drawing.Point(248, 302);
            this.dgvPets.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dgvPets.MultiSelect = false;
            this.dgvPets.Name = "dgvPets";
            this.dgvPets.ReadOnly = true;
            this.dgvPets.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dgvPets.RowHeadersVisible = false;
            this.dgvPets.RowHeadersWidth = 51;
            this.dgvPets.RowTemplate.Height = 24;
            this.dgvPets.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPets.Size = new System.Drawing.Size(753, 323);
            this.dgvPets.TabIndex = 10;
            this.dgvPets.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPets_CellContentClick);
            // 
            // btnClear2
            // 
            this.btnClear2.BackColor = System.Drawing.Color.Transparent;
            this.btnClear2.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnClear2.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btnClear2.Location = new System.Drawing.Point(640, 645);
            this.btnClear2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnClear2.Name = "btnClear2";
            this.btnClear2.Size = new System.Drawing.Size(180, 56);
            this.btnClear2.TabIndex = 16;
            this.btnClear2.Text = "Clear";
            this.btnClear2.UseVisualStyleBackColor = false;
            this.btnClear2.Click += new System.EventHandler(this.btnClear2_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.Transparent;
            this.btnSearch.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnSearch.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btnSearch.Location = new System.Drawing.Point(403, 645);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(180, 56);
            this.btnSearch.TabIndex = 17;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click_1);
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.Transparent;
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.btnBack.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btnBack.Location = new System.Drawing.Point(52, 35);
            this.btnBack.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(56, 62);
            this.btnBack.TabIndex = 18;
            this.btnBack.Text = "→";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.label3.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(93)))), ((int)(((byte)(136)))));
            this.label3.Location = new System.Drawing.Point(389, 211);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(532, 30);
            this.label3.TabIndex = 25;
            this.label3.Text = "Search and view pet records by name or chip number";
            // 
            // SearchPetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1330, 816);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPetName2);
            this.Controls.Add(this.txtChipNumber2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnClear2);
            this.Controls.Add(this.dgvPets);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "SearchPetForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Search Pet";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPets)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPetName2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtChipNumber2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridView dgvPets;
        private System.Windows.Forms.Button btnClear2;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label label3;
    }
}