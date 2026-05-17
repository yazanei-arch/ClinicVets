using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ClinicVets
{
    public partial class ForgotPasswordForm : Form
    {
        private ValidationFieldBinder _validation;

        public ForgotPasswordForm()
            : this(null)
        {
        }

        public ForgotPasswordForm(Form1 loginForm)
        {
            InitializeComponent();
            if (loginForm != null)
            {
                Owner = loginForm;
            }
        }

        private void ForgotPasswordForm_Load(object sender, EventArgs e)
        {
            ThemeHelper.ApplyThemedShell(
                this,
                pnlCard,
                showCardPaws: true,
                centerCardVertically: true,
                headerSubtitle: "Secure password recovery",
                backgroundStyle: FormBackgroundStyle.RegisterFocus);
            ThemeHelper.ApplyStandardLabels(
                lblTitle,
                lblSubtitle,
                lblEmail,
                lblVerificationCode,
                lblNewPassword,
                lblConfirmPassword);
            ThemeHelper.ApplyStandardButtons(btnSendCode, btnResetPassword, btnBack);
            btnSendCode.IsOutlineStyle = true;
            ClinicUiTheme.ApplyOutlineButton(btnSendCode);

            ChromeTextPlate.WrapDirectTextBoxes(pnlCard);
            PasswordVisibilityHelper.Attach(txtNewPassword);
            PasswordVisibilityHelper.Attach(txtConfirmPassword);
            SetupValidation();
            txtEmail.Focus();
        }

        private void SetupValidation()
        {
            _validation = new ValidationFieldBinder(pnlCard);
            _validation.BindTextBox(txtEmail, ValidationHelper.ValidateEmail, lblEmail);
            _validation.BindTextBox(txtVerificationCode, ValidationHelper.ValidateVerificationCode, lblVerificationCode);
            _validation.BindTextBox(txtNewPassword, ValidationHelper.ValidatePassword, lblNewPassword);
            _validation.BindTextBox(
                txtConfirmPassword,
                value => ValidationHelper.ValidateConfirmPassword(txtNewPassword.Text, value),
                lblConfirmPassword);

            _validation.ReflowSingleColumn(124, 44, 452, btnSendCode, btnResetPassword, btnBack);
            ThemeHelper.CenterCardInClient(this, pnlCard);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            VetBackgroundHelper.ClearBackgroundImage(this);
            base.OnFormClosed(e);
        }

        private void btnSendCode_Click(object sender, EventArgs e)
        {
            _validation.ClearField(txtEmail);
            string email = txtEmail.Text.Trim();
            string emailError = ValidationHelper.ValidateEmail(email);
            if (emailError != null)
            {
                ShowFieldError(txtEmail, emailError);
                txtEmail.Focus();
                return;
            }

            if (!PasswordResetService.TrySendVerificationCode(email, out string codeForTesting, out string errorMessage))
            {
                ShowFieldError(txtEmail, errorMessage);
                txtEmail.Focus();
                return;
            }

            MessageBox.Show(
                this,
                "Your verification code is: " + codeForTesting + Environment.NewLine + Environment.NewLine
                + "(Shown for testing only — email is not sent yet.)",
                "Verification Code",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (!_validation.ValidateAll())
            {
                return;
            }

            string email = txtEmail.Text.Trim();
            string code = txtVerificationCode.Text.Trim();
            string newPassword = txtNewPassword.Text;
            string confirm = txtConfirmPassword.Text;

            if (!PasswordResetService.TryResetPassword(email, code, newPassword, confirm, out string errorMessage))
            {
                if (errorMessage == "Verification code is incorrect.")
                {
                    ShowFieldError(txtVerificationCode, errorMessage);
                    txtVerificationCode.Focus();
                    return;
                }

                if (ValidationHelper.ValidatePassword(newPassword) == errorMessage)
                {
                    ShowFieldError(txtNewPassword, errorMessage);
                    txtNewPassword.Focus();
                    return;
                }

                if (ValidationHelper.ValidateConfirmPassword(newPassword, confirm) == errorMessage)
                {
                    ShowFieldError(txtConfirmPassword, errorMessage);
                    txtConfirmPassword.Focus();
                    return;
                }

                ShowFieldError(txtEmail, errorMessage ?? "Could not reset password.");
                return;
            }

            MessageBox.Show(
                this,
                "Your password has been reset successfully. You can now sign in.",
                "Password Reset",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ShowFieldError(TextBox field, string message)
        {
            ValidationFieldBinder.FieldEntry entry = _validation.Entries
                .FirstOrDefault(e => e.InputControl == field);
            if (entry == null)
            {
                return;
            }

            entry.ErrorLabel.Text = message;
            entry.ErrorLabel.Visible = true;
            if (entry.HostControl is ChromeTextPlate plate)
            {
                plate.IsInvalid = true;
            }
        }
    }
}
