
namespace ClinicVets
{
    public partial class VeterinarianMenuForm
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
            this.pnlCard = new ClinicVets.VetMenuCardPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.btnManagePets = new ClinicVets.VetDashboardNavButton();
            this.btnVisits = new ClinicVets.VetDashboardNavButton();
            this.btnMedicines = new ClinicVets.VetDashboardNavButton();
            this.btnLogout = new ClinicVets.VetDashboardNavButton();
            this.pnlCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.pnlCard.Controls.Add(this.lblTitle);
            this.pnlCard.Controls.Add(this.lblSubtitle);
            this.pnlCard.Controls.Add(this.btnManagePets);
            this.pnlCard.Controls.Add(this.btnVisits);
            this.pnlCard.Controls.Add(this.btnMedicines);
            this.pnlCard.Controls.Add(this.btnLogout);
            this.pnlCard.Location = new System.Drawing.Point(310, 195);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(520, 450);
            this.pnlCard.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblTitle.Location = new System.Drawing.Point(40, 32);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(440, 42);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Veterinarian Menu";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(95)))), ((int)(((byte)(120)))));
            this.lblSubtitle.Location = new System.Drawing.Point(40, 78);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(440, 32);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Pets, visits, and medicines.";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnManagePets
            // 
            this.btnManagePets.FlatAppearance.BorderSize = 0;
            this.btnManagePets.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManagePets.Location = new System.Drawing.Point(40, 128);
            this.btnManagePets.MenuIcon = ClinicVets.VetMenuIcon.Pets;
            this.btnManagePets.Name = "btnManagePets";
            this.btnManagePets.Size = new System.Drawing.Size(440, 52);
            this.btnManagePets.TabIndex = 2;
            this.btnManagePets.Text = "Pet Management";
            this.btnManagePets.UseVisualStyleBackColor = false;
            this.btnManagePets.Click += new System.EventHandler(this.btnManagePets_Click);
            // 
            // btnVisits
            // 
            this.btnVisits.FlatAppearance.BorderSize = 0;
            this.btnVisits.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVisits.Location = new System.Drawing.Point(40, 194);
            this.btnVisits.MenuIcon = ClinicVets.VetMenuIcon.Visits;
            this.btnVisits.Name = "btnVisits";
            this.btnVisits.Size = new System.Drawing.Size(440, 52);
            this.btnVisits.TabIndex = 3;
            this.btnVisits.Text = "Visits";
            this.btnVisits.UseVisualStyleBackColor = false;
            this.btnVisits.Click += new System.EventHandler(this.btnVisits_Click);
            // 
            // btnMedicines
            // 
            this.btnMedicines.FlatAppearance.BorderSize = 0;
            this.btnMedicines.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMedicines.Location = new System.Drawing.Point(40, 260);
            this.btnMedicines.MenuIcon = ClinicVets.VetMenuIcon.Medicines;
            this.btnMedicines.Name = "btnMedicines";
            this.btnMedicines.Size = new System.Drawing.Size(440, 52);
            this.btnMedicines.TabIndex = 4;
            this.btnMedicines.Text = "Medicines";
            this.btnMedicines.UseVisualStyleBackColor = false;
            this.btnMedicines.Click += new System.EventHandler(this.btnMedicines_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Location = new System.Drawing.Point(40, 368);
            this.btnLogout.MenuIcon = ClinicVets.VetMenuIcon.Logout;
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(440, 52);
            this.btnLogout.TabIndex = 5;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // VeterinarianMenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(252)))));
            this.CancelButton = this.btnLogout;
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Controls.Add(this.pnlCard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "VeterinarianMenuForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ClinicVets — Veterinarian Menu";
            this.Load += new System.EventHandler(this.VeterinarianMenuForm_Load);
            this.Resize += new System.EventHandler(this.VeterinarianMenuForm_Resize);
            this.pnlCard.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private VetMenuCardPanel pnlCard;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private VetDashboardNavButton btnManagePets;
        private VetDashboardNavButton btnVisits;
        private VetDashboardNavButton btnMedicines;
        private VetDashboardNavButton btnLogout;
    }
}
