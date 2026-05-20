
namespace ClinicVets
{
    public partial class SecretaryMenuForm
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
            this.pnlCard = new ClinicVets.LoginCardPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.btnAddCustomer = new ClinicVets.RoundedActionButton();
            this.btnSearchCustomer = new ClinicVets.RoundedActionButton();
            this.btnViewCustomers = new ClinicVets.RoundedActionButton();
            this.btnLogout = new ClinicVets.RoundedActionButton();
            this.pnlCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.pnlCard.Controls.Add(this.lblTitle);
            this.pnlCard.Controls.Add(this.lblSubtitle);
            this.pnlCard.Controls.Add(this.btnAddCustomer);
            this.pnlCard.Controls.Add(this.btnSearchCustomer);
            this.pnlCard.Controls.Add(this.btnViewCustomers);
            this.pnlCard.Controls.Add(this.btnLogout);
            this.pnlCard.Location = new System.Drawing.Point(390, 255);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Padding = new System.Windows.Forms.Padding(0);
            this.pnlCard.Size = new System.Drawing.Size(560, 430);
            this.pnlCard.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = false;
            this.lblTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(192)))));
            this.lblTitle.Location = new System.Drawing.Point(50, 35);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(460, 42);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Secretary Menu";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(95)))), ((int)(((byte)(120)))));
            this.lblSubtitle.Location = new System.Drawing.Point(50, 85);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(460, 36);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Choose a customer task below.";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnAddCustomer
            // 
            this.btnAddCustomer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.btnAddCustomer.CornerRadius = 8;
            this.btnAddCustomer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddCustomer.FlatAppearance.BorderSize = 0;
            this.btnAddCustomer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddCustomer.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddCustomer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnAddCustomer.IsOutlineStyle = true;
            this.btnAddCustomer.Location = new System.Drawing.Point(50, 135);
            this.btnAddCustomer.Name = "btnAddCustomer";
            this.btnAddCustomer.Size = new System.Drawing.Size(460, 48);
            this.btnAddCustomer.TabIndex = 2;
            this.btnAddCustomer.Text = "Add Customer";
            this.btnAddCustomer.UseVisualStyleBackColor = false;
            this.btnAddCustomer.Click += new System.EventHandler(this.btnAddCustomer_Click);
            // 
            // btnSearchCustomer
            // 
            this.btnSearchCustomer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.btnSearchCustomer.CornerRadius = 8;
            this.btnSearchCustomer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSearchCustomer.FlatAppearance.BorderSize = 0;
            this.btnSearchCustomer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearchCustomer.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearchCustomer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnSearchCustomer.IsOutlineStyle = true;
            this.btnSearchCustomer.Location = new System.Drawing.Point(50, 200);
            this.btnSearchCustomer.Name = "btnSearchCustomer";
            this.btnSearchCustomer.Size = new System.Drawing.Size(460, 48);
            this.btnSearchCustomer.TabIndex = 3;
            this.btnSearchCustomer.Text = "Search Customer";
            this.btnSearchCustomer.UseVisualStyleBackColor = false;
            this.btnSearchCustomer.Click += new System.EventHandler(this.btnSearchCustomer_Click);
            // 
            // btnViewCustomers
            // 
            this.btnViewCustomers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.btnViewCustomers.CornerRadius = 8;
            this.btnViewCustomers.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnViewCustomers.FlatAppearance.BorderSize = 0;
            this.btnViewCustomers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewCustomers.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnViewCustomers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnViewCustomers.IsOutlineStyle = true;
            this.btnViewCustomers.Location = new System.Drawing.Point(50, 265);
            this.btnViewCustomers.Name = "btnViewCustomers";
            this.btnViewCustomers.Size = new System.Drawing.Size(460, 48);
            this.btnViewCustomers.TabIndex = 4;
            this.btnViewCustomers.Text = "View Customers";
            this.btnViewCustomers.UseVisualStyleBackColor = false;
            this.btnViewCustomers.Click += new System.EventHandler(this.btnViewCustomers_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(255)))));
            this.btnLogout.CornerRadius = 8;
            this.btnLogout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogout.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnLogout.IsOutlineStyle = true;
            this.btnLogout.Location = new System.Drawing.Point(50, 345);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(460, 48);
            this.btnLogout.TabIndex = 5;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // SecretaryMenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(252)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.btnLogout;
            this.ClientSize = new System.Drawing.Size(1240, 880);
            this.Controls.Add(this.pnlCard);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SecretaryMenuForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ClinicVets — Secretary Menu";
            this.Load += new System.EventHandler(this.SecretaryMenuForm_Load);
            this.pnlCard.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private LoginCardPanel pnlCard;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private RoundedActionButton btnAddCustomer;
        private RoundedActionButton btnSearchCustomer;
        private RoundedActionButton btnViewCustomers;
        private RoundedActionButton btnLogout;
    }
}
