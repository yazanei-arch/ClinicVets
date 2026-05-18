namespace ClinicVets
{
    public partial class Form1
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
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblUsernameError = new System.Windows.Forms.Label();
            this.lblPasswordError = new System.Windows.Forms.Label();
            this.lnkForgotPassword = new System.Windows.Forms.LinkLabel();
            this.btnLogin = new ClinicVets.RoundedActionButton();
            this.btnRegister = new ClinicVets.RoundedActionButton();
            this.pnlCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pnlCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.pnlCard.Controls.Add(this.lblTitle);
            this.pnlCard.Controls.Add(this.lblSubtitle);
            this.pnlCard.Controls.Add(this.lblUsername);
            this.pnlCard.Controls.Add(this.txtUsername);
            this.pnlCard.Controls.Add(this.lblPassword);
            this.pnlCard.Controls.Add(this.txtPassword);
            this.pnlCard.Controls.Add(this.lblUsernameError);
            this.pnlCard.Controls.Add(this.lblPasswordError);
            this.pnlCard.Controls.Add(this.lnkForgotPassword);
            this.pnlCard.Controls.Add(this.btnLogin);
            this.pnlCard.Controls.Add(this.btnRegister);
            this.pnlCard.Location = new System.Drawing.Point(620, 120);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Padding = new System.Windows.Forms.Padding(32, 28, 32, 28);
            this.pnlCard.Size = new System.Drawing.Size(400, 480);
            this.pnlCard.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = false;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(192)))));
            this.lblTitle.Location = new System.Drawing.Point(32, 28);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(336, 36);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Welcome Back";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.BackColor = System.Drawing.Color.Transparent;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(95)))), ((int)(((byte)(120)))));
            this.lblSubtitle.Location = new System.Drawing.Point(32, 68);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(336, 36);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Sign in to access your clinic workspace";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.BackColor = System.Drawing.Color.Transparent;
            this.lblUsername.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsername.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(168)))));
            this.lblUsername.Location = new System.Drawing.Point(32, 128);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(71, 17);
            this.lblUsername.TabIndex = 3;
            this.lblUsername.Text = "Username";
            // 
            // txtUsername
            // 
            this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsername.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(52)))), ((int)(((byte)(72)))));
            this.txtUsername.Location = new System.Drawing.Point(32, 152);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(336, 25);
            this.txtUsername.TabIndex = 4;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.BackColor = System.Drawing.Color.Transparent;
            this.lblPassword.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(137)))), ((int)(((byte)(168)))));
            this.lblPassword.Location = new System.Drawing.Point(32, 196);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(67, 17);
            this.lblPassword.TabIndex = 5;
            this.lblPassword.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(52)))), ((int)(((byte)(72)))));
            this.txtPassword.Location = new System.Drawing.Point(32, 220);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(336, 25);
            this.txtPassword.TabIndex = 6;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // lblUsernameError
            // 
            this.lblUsernameError.AutoSize = false;
            this.lblUsernameError.BackColor = System.Drawing.Color.Transparent;
            this.lblUsernameError.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsernameError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.lblUsernameError.Location = new System.Drawing.Point(32, 184);
            this.lblUsernameError.Name = "lblUsernameError";
            this.lblUsernameError.Size = new System.Drawing.Size(336, 18);
            this.lblUsernameError.TabIndex = 9;
            this.lblUsernameError.Visible = false;
            // 
            // lblPasswordError
            // 
            this.lblPasswordError.AutoSize = false;
            this.lblPasswordError.BackColor = System.Drawing.Color.Transparent;
            this.lblPasswordError.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPasswordError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(47)))), ((int)(((byte)(47)))));
            this.lblPasswordError.Location = new System.Drawing.Point(32, 252);
            this.lblPasswordError.Name = "lblPasswordError";
            this.lblPasswordError.Size = new System.Drawing.Size(336, 18);
            this.lblPasswordError.TabIndex = 10;
            this.lblPasswordError.Visible = false;
            // 
            // lnkForgotPassword
            // 
            this.lnkForgotPassword.AutoSize = true;
            this.lnkForgotPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.lnkForgotPassword.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lnkForgotPassword.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkForgotPassword.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkForgotPassword.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lnkForgotPassword.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(151)))), ((int)(((byte)(167)))));
            this.lnkForgotPassword.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.lnkForgotPassword.Location = new System.Drawing.Point(32, 278);
            this.lnkForgotPassword.Name = "lnkForgotPassword";
            this.lnkForgotPassword.Size = new System.Drawing.Size(103, 15);
            this.lnkForgotPassword.TabIndex = 11;
            this.lnkForgotPassword.TabStop = true;
            this.lnkForgotPassword.Text = "Forgot Password?";
            this.lnkForgotPassword.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkForgotPassword_LinkClicked);
            // 
            // btnLogin
            // 
            this.btnLogin.CornerRadius = 10;
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.IsOutlineStyle = false;
            this.btnLogin.Location = new System.Drawing.Point(32, 308);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(336, 44);
            this.btnLogin.TabIndex = 7;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnRegister
            // 
            this.btnRegister.CornerRadius = 10;
            this.btnRegister.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegister.FlatAppearance.BorderSize = 0;
            this.btnRegister.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegister.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRegister.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(118)))), ((int)(((byte)(210)))));
            this.btnRegister.IsOutlineStyle = true;
            this.btnRegister.Location = new System.Drawing.Point(32, 364);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(336, 44);
            this.btnRegister.TabIndex = 8;
            this.btnRegister.Text = "Register New Employee";
            this.btnRegister.UseVisualStyleBackColor = false;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(1120, 700);
            this.Controls.Add(this.pnlCard);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ClinicVets — Login";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private LoginCardPanel pnlCard;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblUsernameError;
        private System.Windows.Forms.Label lblPasswordError;
        private System.Windows.Forms.LinkLabel lnkForgotPassword;
        private RoundedActionButton btnLogin;
        private RoundedActionButton btnRegister;
    }

}
