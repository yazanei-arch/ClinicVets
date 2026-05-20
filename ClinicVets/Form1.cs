using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ClinicVets
{
    public partial class Form1 : Form
    {
        private const string InvalidCredentialsMessage = "Invalid username or password.";
        private const int LoginErrorHeight = 20;
        private const int FieldGap = 10;
        private const int SectionGap = 14;

        private static readonly Color LoginTitleColor = Color.FromArgb(21, 101, 192);
        private static readonly Color LoginSubtitleColor = Color.FromArgb(71, 95, 120);
        private static readonly Color LoginLabelColor = Color.FromArgb(0, 137, 168);
        private static readonly Color LoginLinkColor = Color.FromArgb(25, 118, 210);
        private static readonly Color LoginLinkHoverColor = Color.FromArgb(0, 151, 167);
        private static readonly Color LoginErrorColor = Color.FromArgb(211, 47, 47);

        private ValidationFieldBinder _validation;
        private LinkLabel _passwordToggle;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WinFormsUi.SetDoubleBuffered(this);
            VetBackgroundHelper.ApplyLoginBackground(this);

            pnlCard.CornerRadius = 12;
            pnlCard.Padding = new Padding(32, 28, 32, 28);
            pnlCard.Width = 400;

            lblTitle.Text = "Welcome Back";
            lblSubtitle.Text = "Sign in to access your clinic workspace";

            ApplyLoginTypography();

            lnkForgotPassword.LinkColor = LoginLinkColor;
            lnkForgotPassword.ActiveLinkColor = LoginLinkHoverColor;
            lnkForgotPassword.VisitedLinkColor = LoginLinkColor;
            lnkForgotPassword.BackColor = ClinicUiTheme.SoftChromeSurface;

            btnLogin.UseLoginLightStyle = true;
            btnLogin.IsOutlineStyle = false;
            btnLogin.CornerRadius = 10;
            btnRegister.UseLoginLightStyle = true;
            btnRegister.IsOutlineStyle = true;
            btnRegister.CornerRadius = 10;

            ChromeTextPlate.WrapDirectTextBoxes(pnlCard);
            ApplyLoginFieldChrome();
            _passwordToggle = PasswordVisibilityHelper.Attach(txtPassword);
            if (_passwordToggle != null)
            {
                _passwordToggle.LinkColor = LoginLinkColor;
                _passwordToggle.ActiveLinkColor = LoginLinkHoverColor;
                _passwordToggle.VisitedLinkColor = LoginLinkColor;
            }

            SetupValidation();
            LayoutLoginControls();
            Resize += Form1_Resize;
            txtUsername.Focus();
        }

        private void ApplyLoginTypography()
        {
            lblTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.ForeColor = LoginTitleColor;
            lblTitle.BackColor = Color.Transparent;

            lblSubtitle.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            lblSubtitle.ForeColor = LoginSubtitleColor;
            lblSubtitle.BackColor = Color.Transparent;

            foreach (Label label in new[] { lblUsername, lblPassword })
            {
                label.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
                label.ForeColor = LoginLabelColor;
                label.BackColor = Color.Transparent;
            }

            lblUsernameError.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lblUsernameError.ForeColor = LoginErrorColor;
            lblUsernameError.BackColor = Color.Transparent;

            lblPasswordError.Font = lblUsernameError.Font;
            lblPasswordError.ForeColor = LoginErrorColor;
            lblPasswordError.BackColor = Color.Transparent;

            btnLogin.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point);
            btnRegister.Font = btnLogin.Font;
        }

        private void ApplyLoginFieldChrome()
        {
            foreach (ChromeTextPlate plate in pnlCard.Controls.OfType<ChromeTextPlate>())
            {
                plate.UseLoginLightStyle = true;
                foreach (TextBox box in plate.Controls.OfType<TextBox>())
                {
                    box.ForeColor = Color.FromArgb(33, 52, 72);
                    box.BackColor = Color.White;
                }
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            LayoutLoginControls();
        }

        private void SetupValidation()
        {
            _validation = new ValidationFieldBinder(pnlCard);
            _validation.BindExisting(txtUsername, lblUsernameError, ValidationHelper.ValidateUsername);
            _validation.BindExisting(txtPassword, lblPasswordError, ValidationHelper.ValidatePassword);
        }

        private void LayoutLoginControls()
        {
            int padL = pnlCard.Padding.Left;
            int padR = pnlCard.Padding.Right;
            int contentW = Math.Max(200, pnlCard.ClientSize.Width - padL - padR);
            int x = padL;
            int y = pnlCard.Padding.Top;
            int fieldH = 38;

            lblTitle.SetBounds(x, y, contentW, 36);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            y += 40;

            lblSubtitle.SetBounds(x, y, contentW, 36);
            lblSubtitle.TextAlign = ContentAlignment.TopCenter;
            y += 40 + SectionGap;

            lblUsername.SetBounds(x, y, contentW, 22);
            lblUsername.AutoSize = false;
            y += 24;

            Control usernameHost = GetFieldHost(txtUsername);
            usernameHost.SetBounds(x, y, contentW, fieldH);
            y += fieldH + 2;

            lblUsernameError.SetBounds(x, y, contentW, LoginErrorHeight);
            y += LoginErrorHeight + FieldGap;

            lblPassword.SetBounds(x, y, contentW, 22);
            lblPassword.AutoSize = false;
            y += 24;

            Control passwordHost = GetFieldHost(txtPassword);
            passwordHost.SetBounds(x, y, contentW, fieldH);
            y += fieldH + 2;

            lblPasswordError.SetBounds(x, y, contentW, LoginErrorHeight);
            y += LoginErrorHeight + FieldGap;

            lnkForgotPassword.AutoSize = true;
            lnkForgotPassword.Location = new Point(x, y);
            y += lnkForgotPassword.Height + SectionGap;

            btnLogin.SetBounds(x, y, contentW, 44);
            y += 44 + 12;

            btnRegister.SetBounds(x, y, contentW, 44);
            y += 44;

            pnlCard.Height = y + pnlCard.Padding.Bottom;
            PositionLoginCard();
        }

        private void PositionLoginCard()
        {
            const int margin = 36;
            int zoneStart = (int)(ClientSize.Width * 0.54f);
            int zoneEnd = ClientSize.Width - margin;
            int zoneWidth = Math.Max(0, zoneEnd - zoneStart);
            pnlCard.Left = zoneStart + Math.Max(0, (zoneWidth - pnlCard.Width) / 2);
            pnlCard.Top = Math.Max(margin, (ClientSize.Height - pnlCard.Height) / 2);
            pnlCard.BringToFront();
        }

        private static Control GetFieldHost(TextBox textBox)
        {
            if (textBox?.Parent is ChromeTextPlate plate)
            {
                return plate;
            }

            return textBox;
        }

        private void lnkForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var forgotForm = new ForgotPasswordForm(this))
            {
                forgotForm.ShowDialog(this);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            VetBackgroundHelper.ClearBackgroundImage(this);
            base.OnFormClosed(e);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!_validation.ValidateAll())
            {
                LayoutLoginControls();
                FocusFirstInvalidField();
                return;
            }

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            Employee employee = LoginAuthService.Authenticate(username, password);
            if (employee == null)
            {
                lblPasswordError.Text = InvalidCredentialsMessage;
                lblPasswordError.Visible = true;
                if (txtPassword.Parent is ChromeTextPlate passwordPlate)
                {
                    passwordPlate.IsInvalid = true;
                }

                LayoutLoginControls();
                txtPassword.Focus();
                txtPassword.SelectAll();
                return;
            }

            Hide();
            try
            {
                RoleNavigationHelper.TryNavigateAfterLogin(employee, this);
            }
            finally
            {
                SessionManager.CurrentUser = null;
                if (!IsDisposed)
                {
                    Show();
                }
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            using (var registerForm = new RegisterEmployeeForm(this))
            {
                registerForm.ShowDialog(this);
            }
        }

        private void FocusFirstInvalidField()
        {
            if (lblUsernameError.Visible)
            {
                txtUsername.Focus();
                return;
            }

            if (lblPasswordError.Visible)
            {
                txtPassword.Focus();
            }
        }

        public void ClearLoginFields()
        {
            txtUsername.Clear();
            txtPassword.Clear();
            PasswordVisibilityHelper.ResetToHidden(txtPassword, _passwordToggle);
            _validation?.ClearAll();
            txtUsername.Focus();
        }
    }
}
