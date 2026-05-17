using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClinicVets
{
    public partial class Form1 : Form
    {
        private const string InvalidCredentialsMessage = "Invalid username or password.";
        private const int LoginErrorHeight = 20;
        private const int FieldGap = 10;
        private const int SectionGap = 14;

        private ValidationFieldBinder _validation;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            VetBackgroundHelper.ClearBackgroundImage(this);

            ThemeHelper.ApplyThemedShell(
                this,
                pnlCard,
                centerCardVertically: true,
                showHeader: false,
                backgroundStyle: FormBackgroundStyle.LoginMinimal);

            pnlCard.UseOpaqueFill = true;
            pnlCard.ShowCornerDecorations = false;
            pnlCard.Padding = new Padding(36, 32, 36, 32);
            pnlCard.Width = 440;

            lblTitle.Text = "Sign In";
            lblSubtitle.Text = "Welcome back. Sign in to access the clinic management hub.";

            ThemeHelper.ApplyStandardLabels(lblTitle, lblSubtitle, lblUsername, lblPassword);
            ThemeHelper.ApplyErrorLabels(lblUsernameError, lblPasswordError);
            ThemeHelper.ApplyStandardButtons(btnLogin, btnRegister);

            lnkForgotPassword.LinkColor = ClinicUiTheme.AccentCyan;
            lnkForgotPassword.ActiveLinkColor = ClinicUiTheme.AccentBlueHover;
            lnkForgotPassword.VisitedLinkColor = ClinicUiTheme.AccentCyan;
            lnkForgotPassword.BackColor = Color.Transparent;

            ChromeTextPlate.WrapDirectTextBoxes(pnlCard);
            PasswordVisibilityHelper.Attach(txtPassword);
            SetupValidation();

            LayoutLoginControls();
            Resize += Form1_Resize;
            txtUsername.Focus();
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
            int fieldH = ClinicUiTheme.FieldHeight;

            lblTitle.SetBounds(x, y, contentW, 36);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            y += 40;

            lblSubtitle.SetBounds(x, y, contentW, 32);
            lblSubtitle.TextAlign = ContentAlignment.TopCenter;
            y += 36 + SectionGap;

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

            btnLogin.SetBounds(x, y, contentW, ClinicUiTheme.ButtonHeight);
            y += ClinicUiTheme.ButtonHeight + 12;

            btnRegister.SetBounds(x, y, contentW, ClinicUiTheme.ButtonHeight);
            y += ClinicUiTheme.ButtonHeight;

            pnlCard.Height = y + pnlCard.Padding.Bottom;
            ThemeHelper.CenterCardInClient(this, pnlCard, verticalBias: 0);
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

            SessionManager.CurrentUser = employee;

            Hide();
            try
            {
                using (var customerForm = new CustomerManagementForm(this))
                {
                    customerForm.ShowDialog(this);
                }
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
    }
}
