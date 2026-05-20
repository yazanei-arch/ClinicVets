
namespace ClinicVets
{
    public partial class ForgotPasswordForm
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.pnlDivider = new System.Windows.Forms.Panel();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblVerificationCode = new System.Windows.Forms.Label();
            this.txtVerificationCode = new System.Windows.Forms.TextBox();
            this.lblNewPassword = new System.Windows.Forms.Label();
            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.lblConfirmPassword = new System.Windows.Forms.Label();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.btnSendCode = new ClinicVets.RoundedActionButton();
            this.btnResetPassword = new ClinicVets.RoundedActionButton();
            this.btnBack = new ClinicVets.RoundedActionButton();
            this.pnlCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.pnlCard.Controls.Add(this.lblTitle);
            this.pnlCard.Controls.Add(this.lblSubtitle);
            this.pnlCard.Controls.Add(this.pnlDivider);
            this.pnlCard.Controls.Add(this.lblEmail);
            this.pnlCard.Controls.Add(this.txtEmail);
            this.pnlCard.Controls.Add(this.lblVerificationCode);
            this.pnlCard.Controls.Add(this.txtVerificationCode);
            this.pnlCard.Controls.Add(this.lblNewPassword);
            this.pnlCard.Controls.Add(this.txtNewPassword);
            this.pnlCard.Controls.Add(this.lblConfirmPassword);
            this.pnlCard.Controls.Add(this.txtConfirmPassword);
            this.pnlCard.Controls.Add(this.btnSendCode);
            this.pnlCard.Controls.Add(this.btnResetPassword);
            this.pnlCard.Controls.Add(this.btnBack);
            this.pnlCard.Location = new System.Drawing.Point(560, 95);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(520, 620);
            this.pnlCard.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 19F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(192)))));
            this.lblTitle.Location = new System.Drawing.Point(40, 32);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(440, 40);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Forgot Password";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(95)))), ((int)(((byte)(120)))));
            this.lblSubtitle.Location = new System.Drawing.Point(40, 76);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(440, 44);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Enter your email, receive a verification code, then set a new password.";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pnlDivider
            // 
            this.pnlDivider.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(225)))), ((int)(((byte)(240)))));
            this.pnlDivider.Location = new System.Drawing.Point(40, 128);
            this.pnlDivider.Name = "pnlDivider";
            this.pnlDivider.Size = new System.Drawing.Size(440, 1);
            this.pnlDivider.TabIndex = 2;
            // 
            // lblEmail
            // 
            this.lblEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.lblEmail.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblEmail.Location = new System.Drawing.Point(40, 140);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(440, 22);
            this.lblEmail.TabIndex = 3;
            this.lblEmail.Text = "✉ Email";
            // 
            // txtEmail
            // 
            this.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtEmail.Location = new System.Drawing.Point(40, 164);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(440, 30);
            this.txtEmail.TabIndex = 4;
            // 
            // lblVerificationCode
            // 
            this.lblVerificationCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.lblVerificationCode.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVerificationCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblVerificationCode.Location = new System.Drawing.Point(40, 208);
            this.lblVerificationCode.Name = "lblVerificationCode";
            this.lblVerificationCode.Size = new System.Drawing.Size(440, 22);
            this.lblVerificationCode.TabIndex = 5;
            this.lblVerificationCode.Text = "🔐 Verification Code";
            // 
            // txtVerificationCode
            // 
            this.txtVerificationCode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtVerificationCode.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtVerificationCode.Location = new System.Drawing.Point(40, 232);
            this.txtVerificationCode.Name = "txtVerificationCode";
            this.txtVerificationCode.Size = new System.Drawing.Size(440, 30);
            this.txtVerificationCode.TabIndex = 6;
            this.txtVerificationCode.UseSystemPasswordChar = true;
            // 
            // lblNewPassword
            // 
            this.lblNewPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.lblNewPassword.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblNewPassword.Location = new System.Drawing.Point(40, 276);
            this.lblNewPassword.Name = "lblNewPassword";
            this.lblNewPassword.Size = new System.Drawing.Size(440, 22);
            this.lblNewPassword.TabIndex = 7;
            this.lblNewPassword.Text = "🔒 New Password";
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNewPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNewPassword.Location = new System.Drawing.Point(40, 300);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.Size = new System.Drawing.Size(440, 30);
            this.txtNewPassword.TabIndex = 8;
            this.txtNewPassword.UseSystemPasswordChar = true;
            // 
            // lblConfirmPassword
            // 
            this.lblConfirmPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.lblConfirmPassword.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConfirmPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lblConfirmPassword.Location = new System.Drawing.Point(40, 344);
            this.lblConfirmPassword.Name = "lblConfirmPassword";
            this.lblConfirmPassword.Size = new System.Drawing.Size(440, 22);
            this.lblConfirmPassword.TabIndex = 9;
            this.lblConfirmPassword.Text = "🔒 Confirm Password";
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtConfirmPassword.Location = new System.Drawing.Point(40, 368);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.Size = new System.Drawing.Size(440, 30);
            this.txtConfirmPassword.TabIndex = 10;
            this.txtConfirmPassword.UseSystemPasswordChar = true;
            // 
            // btnSendCode
            // 
            this.btnSendCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.btnSendCode.CornerRadius = 8;
            this.btnSendCode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSendCode.FlatAppearance.BorderSize = 0;
            this.btnSendCode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendCode.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSendCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnSendCode.IsOutlineStyle = true;
            this.btnSendCode.Location = new System.Drawing.Point(40, 418);
            this.btnSendCode.Name = "btnSendCode";
            this.btnSendCode.Size = new System.Drawing.Size(440, 44);
            this.btnSendCode.TabIndex = 11;
            this.btnSendCode.Text = "Send Code";
            this.btnSendCode.UseVisualStyleBackColor = false;
            this.btnSendCode.Click += new System.EventHandler(this.btnSendCode_Click);
            // 
            // btnResetPassword
            // 
            this.btnResetPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.btnResetPassword.CornerRadius = 8;
            this.btnResetPassword.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnResetPassword.FlatAppearance.BorderSize = 0;
            this.btnResetPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetPassword.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnResetPassword.ForeColor = System.Drawing.Color.White;
            this.btnResetPassword.IsOutlineStyle = false;
            this.btnResetPassword.Location = new System.Drawing.Point(40, 472);
            this.btnResetPassword.Name = "btnResetPassword";
            this.btnResetPassword.Size = new System.Drawing.Size(440, 44);
            this.btnResetPassword.TabIndex = 12;
            this.btnResetPassword.Text = "Reset Password";
            this.btnResetPassword.UseVisualStyleBackColor = false;
            this.btnResetPassword.Click += new System.EventHandler(this.btnResetPassword_Click);
            // 
            // btnBack
            // 
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.btnBack.CornerRadius = 8;
            this.btnBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBack.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnBack.FlatAppearance.BorderSize = 0;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnBack.IsOutlineStyle = true;
            this.btnBack.Location = new System.Drawing.Point(40, 526);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(440, 44);
            this.btnBack.TabIndex = 13;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = false;
            // 
            // ForgotPasswordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.btnBack;
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.pnlCard);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ControlBox = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "ForgotPasswordForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ClinicVets - Forgot Password";
            this.Load += new System.EventHandler(this.ForgotPasswordForm_Load);
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private CardPanel pnlCard;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Panel pnlDivider;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblVerificationCode;
        private System.Windows.Forms.TextBox txtVerificationCode;
        private System.Windows.Forms.Label lblNewPassword;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.Label lblConfirmPassword;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private RoundedActionButton btnSendCode;
        private RoundedActionButton btnResetPassword;
        private RoundedActionButton btnBack;
    }
}
