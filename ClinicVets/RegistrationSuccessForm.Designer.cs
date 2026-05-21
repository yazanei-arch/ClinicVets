
namespace ClinicVets
{
    public partial class RegistrationSuccessForm
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
            this.pnlCard = new ClinicVets.CardPanel();
            this.lblSuccessIcon = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.btnBackToLogin = new ClinicVets.RoundedActionButton();
            this.btnRegisterAnother = new ClinicVets.RoundedActionButton();
            this.pnlCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.pnlCard.Controls.Add(this.lblSuccessIcon);
            this.pnlCard.Controls.Add(this.lblTitle);
            this.pnlCard.Controls.Add(this.lblSubtitle);
            this.pnlCard.Controls.Add(this.btnBackToLogin);
            this.pnlCard.Controls.Add(this.btnRegisterAnother);
            this.pnlCard.Location = new System.Drawing.Point(630, 55);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Padding = new System.Windows.Forms.Padding(0);
            this.pnlCard.Size = new System.Drawing.Size(560, 760);
            this.pnlCard.TabIndex = 0;
            // 
            // lblSuccessIcon
            // 
            this.lblSuccessIcon.AutoSize = false;
            this.lblSuccessIcon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.lblSuccessIcon.Font = new System.Drawing.Font("Segoe UI", 64F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSuccessIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.lblSuccessIcon.Location = new System.Drawing.Point(220, 150);
            this.lblSuccessIcon.Name = "lblSuccessIcon";
            this.lblSuccessIcon.Size = new System.Drawing.Size(120, 120);
            this.lblSuccessIcon.TabIndex = 0;
            this.lblSuccessIcon.Text = "\u2713";
            this.lblSuccessIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = false;
            this.lblTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(192)))));
            this.lblTitle.Location = new System.Drawing.Point(45, 300);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(470, 48);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Registration Successful";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(95)))), ((int)(((byte)(120)))));
            this.lblSubtitle.Location = new System.Drawing.Point(45, 360);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(470, 56);
            this.lblSubtitle.TabIndex = 2;
            this.lblSubtitle.Text = "Your account has been created successfully.";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnBackToLogin
            // 
            this.btnBackToLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.btnBackToLogin.CornerRadius = 8;
            this.btnBackToLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBackToLogin.FlatAppearance.BorderSize = 0;
            this.btnBackToLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackToLogin.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBackToLogin.ForeColor = System.Drawing.Color.White;
            this.btnBackToLogin.IsOutlineStyle = false;
            this.btnBackToLogin.Location = new System.Drawing.Point(45, 470);
            this.btnBackToLogin.Name = "btnBackToLogin";
            this.btnBackToLogin.Size = new System.Drawing.Size(470, 48);
            this.btnBackToLogin.TabIndex = 3;
            this.btnBackToLogin.Text = "Back to Login";
            this.btnBackToLogin.UseVisualStyleBackColor = false;
            this.btnBackToLogin.Click += new System.EventHandler(this.btnBackToLogin_Click);
            // 
            // btnRegisterAnother
            // 
            this.btnRegisterAnother.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.btnRegisterAnother.CornerRadius = 8;
            this.btnRegisterAnother.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegisterAnother.FlatAppearance.BorderSize = 0;
            this.btnRegisterAnother.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegisterAnother.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRegisterAnother.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnRegisterAnother.IsOutlineStyle = true;
            this.btnRegisterAnother.Location = new System.Drawing.Point(45, 535);
            this.btnRegisterAnother.Name = "btnRegisterAnother";
            this.btnRegisterAnother.Size = new System.Drawing.Size(470, 48);
            this.btnRegisterAnother.TabIndex = 4;
            this.btnRegisterAnother.Text = "Register Another Employee";
            this.btnRegisterAnother.UseVisualStyleBackColor = false;
            this.btnRegisterAnother.Click += new System.EventHandler(this.btnRegisterAnother_Click);
            // 
            // RegistrationSuccessForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1240, 880);
            this.Controls.Add(this.pnlCard);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "RegistrationSuccessForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ClinicVets — Registration Successful";
            this.Load += new System.EventHandler(this.RegistrationSuccessForm_Load);
            this.pnlCard.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private CardPanel pnlCard;
        private System.Windows.Forms.Label lblSuccessIcon;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private RoundedActionButton btnBackToLogin;
        private RoundedActionButton btnRegisterAnother;
    }
}
