using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ClinicVets
{
    public partial class ForgotPasswordForm : Form
    {
        private static readonly Color TitleColor = Color.FromArgb(21, 101, 192);
        private static readonly Color SubtitleColor = Color.FromArgb(71, 95, 120);
        private static readonly Color LabelColor = Color.FromArgb(25, 118, 210);
        private static readonly Color LinkColor = Color.FromArgb(25, 118, 210);
        private static readonly Color LinkHoverColor = Color.FromArgb(0, 151, 167);
        private static readonly Color CardSurface = Color.FromArgb(240, 255, 255, 255);
        private static readonly Color FormFallbackBack = Color.FromArgb(232, 244, 252);

        private const int InnerX = 40;
        private const int FieldWidth = 440;
        private const int FieldHeight = 32;
        private const int CaptionHeight = 22;
        private const int ErrorHeight = 16;

        private ValidationFieldBinder _validation;
        private Image _ownedBackgroundImage;

        public ForgotPasswordForm()
            : this(null)
        {
        }

        public ForgotPasswordForm(Form1 loginForm)
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.Sizable;
            ControlBox = true;
            MinimizeBox = true;
            MaximizeBox = true;
            Text = "ClinicVets - Forgot Password";
            StartPosition = FormStartPosition.CenterParent;
            DoubleBuffered = true;

            if (loginForm != null)
            {
                Owner = loginForm;
            }
        }

        private void ForgotPasswordForm_Load(object sender, EventArgs e)
        {
            WinFormsUi.SetDoubleBuffered(this);
            ApplyForgotPasswordBackground();

            pnlCard.UseRegisterLightStyle = true;
            pnlCard.ShowCornerDecorations = false;
            pnlCard.CornerRadius = CardPanel.RegisterCornerRadius;
            pnlCard.Location = new Point(560, 95);
            pnlCard.Size = new Size(520, 620);
            pnlCard.Padding = new Padding(0);
            pnlCard.BackColor = CardSurface;

            ApplyForgotTypography();
            ApplyHeaderChrome();

            btnSendCode.UseLoginLightStyle = true;
            btnSendCode.IsOutlineStyle = true;
            btnSendCode.CornerRadius = 8;

            btnResetPassword.UseLoginLightStyle = true;
            btnResetPassword.IsOutlineStyle = false;
            btnResetPassword.CornerRadius = 8;

            btnBack.UseLoginLightStyle = true;
            btnBack.IsOutlineStyle = true;
            btnBack.CornerRadius = 8;

            ChromeTextPlate.WrapDirectTextBoxes(pnlCard);
            ApplyFieldChrome();
            ApplyPasswordToggles();
            SetupValidation();
            ApplyLabelSurfaces();
            LayoutForgotPasswordControls();
            txtEmail.Focus();
        }

        private void ApplyForgotTypography()
        {
            lblTitle.Font = new Font("Segoe UI", 19F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.ForeColor = TitleColor;

            lblSubtitle.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblSubtitle.ForeColor = SubtitleColor;

            foreach (Label label in new[] { lblEmail, lblVerificationCode, lblNewPassword, lblConfirmPassword })
            {
                label.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
                label.ForeColor = LabelColor;
            }

            Font buttonFont = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point);
            btnSendCode.Font = buttonFont;
            btnResetPassword.Font = buttonFont;
            btnBack.Font = buttonFont;
        }

        private void ApplyHeaderChrome()
        {
            lblTitle.BackColor = CardSurface;
            lblSubtitle.BackColor = CardSurface;
            pnlDivider.BackColor = Color.FromArgb(210, 225, 240);
        }

        private void ApplyLabelSurfaces()
        {
            foreach (Control control in pnlCard.Controls)
            {
                if (control is Label label)
                {
                    label.BackColor = CardSurface;
                }
            }
        }

        private void ApplyFieldChrome()
        {
            foreach (ChromeTextPlate plate in pnlCard.Controls.OfType<ChromeTextPlate>())
            {
                plate.UseLoginLightStyle = true;
                plate.BackColor = CardSurface;
                foreach (TextBox box in plate.Controls.OfType<TextBox>())
                {
                    box.ForeColor = Color.FromArgb(33, 52, 72);
                    box.BackColor = Color.White;
                }
            }
        }

        private void ApplyPasswordToggles()
        {
            txtVerificationCode.UseSystemPasswordChar = true;
            txtNewPassword.UseSystemPasswordChar = true;
            txtConfirmPassword.UseSystemPasswordChar = true;

            StyleToggle(PasswordVisibilityHelper.Attach(txtVerificationCode));
            StyleToggle(PasswordVisibilityHelper.Attach(txtNewPassword));
            StyleToggle(PasswordVisibilityHelper.Attach(txtConfirmPassword));
        }

        private void StyleToggle(LinkLabel toggle)
        {
            if (toggle == null)
            {
                return;
            }

            toggle.LinkColor = LinkColor;
            toggle.ActiveLinkColor = LinkHoverColor;
            toggle.VisitedLinkColor = LinkColor;
            toggle.BackColor = CardSurface;
        }

        private void LayoutForgotPasswordControls()
        {
            lblTitle.SetBounds(InnerX, 32, FieldWidth, 40);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            lblSubtitle.SetBounds(InnerX, 76, FieldWidth, 44);
            lblSubtitle.TextAlign = ContentAlignment.TopCenter;

            pnlDivider.SetBounds(InnerX, 128, FieldWidth, 1);

            LayoutFieldRow(lblEmail, txtEmail, 140);
            LayoutFieldRow(lblVerificationCode, txtVerificationCode, 208);
            LayoutFieldRow(lblNewPassword, txtNewPassword, 276);
            LayoutFieldRow(lblConfirmPassword, txtConfirmPassword, 344);

            PositionFieldError(txtEmail, 140);
            PositionFieldError(txtVerificationCode, 208);
            PositionFieldError(txtNewPassword, 276);
            PositionFieldError(txtConfirmPassword, 344);

            btnSendCode.SetBounds(InnerX, 418, FieldWidth, 44);
            btnResetPassword.SetBounds(InnerX, 472, FieldWidth, 44);
            btnBack.SetBounds(InnerX, 526, FieldWidth, 44);
        }

        private void LayoutFieldRow(Label caption, TextBox input, int captionY)
        {
            caption.AutoSize = false;
            caption.SetBounds(InnerX, captionY, FieldWidth, CaptionHeight);
            Control host = GetFieldHost(input);
            host.SetBounds(InnerX, captionY + CaptionHeight + 2, FieldWidth, FieldHeight);
            AlignChromePlateInner(host);
        }

        private void PositionFieldError(TextBox input, int captionY)
        {
            if (_validation == null)
            {
                return;
            }

            ValidationFieldBinder.FieldEntry entry = _validation.Entries.FirstOrDefault(e => e.InputControl == input);
            if (entry == null)
            {
                return;
            }

            int y = captionY + CaptionHeight + 2 + FieldHeight + 2;
            entry.ErrorLabel.SetBounds(InnerX, y, FieldWidth, ErrorHeight);
        }

        private static Control GetFieldHost(Control input)
        {
            if (input?.Parent is ChromeTextPlate plate)
            {
                return plate;
            }

            return input;
        }

        private static void AlignChromePlateInner(Control host)
        {
            if (!(host is ChromeTextPlate plate))
            {
                return;
            }

            foreach (Control inner in plate.Controls)
            {
                if (inner is LinkLabel toggle)
                {
                    continue;
                }

                inner.Location = new Point(plate.Padding.Left + 2, plate.Padding.Top + 2);
                int toggleReserve = 52;
                inner.Width = Math.Max(10, plate.ClientSize.Width - plate.Padding.Horizontal - toggleReserve);
                inner.Height = Math.Max(10, plate.ClientSize.Height - plate.Padding.Vertical - 4);
            }

            LinkLabel showHide = plate.Controls.OfType<LinkLabel>().FirstOrDefault();
            if (showHide != null)
            {
                showHide.AutoSize = true;
                showHide.Location = new Point(
                    Math.Max(plate.Padding.Left, plate.ClientSize.Width - showHide.PreferredWidth - 10),
                    Math.Max(0, (plate.ClientSize.Height - showHide.Height) / 2));
            }
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
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ClearForgotPasswordBackground();
            base.OnFormClosed(e);
        }

        private void ApplyForgotPasswordBackground()
        {
            ClearForgotPasswordBackground();
            BackgroundImageLayout = ImageLayout.Stretch;

            string path = FindForgotPasswordBackgroundPath();
            if (path == null)
            {
                BackColor = FormFallbackBack;
                Invalidate(true);
                return;
            }

            try
            {
                _ownedBackgroundImage = Image.FromFile(path);
                BackgroundImage = (Image)_ownedBackgroundImage.Clone();
                BackColor = FormFallbackBack;
                Invalidate(true);
            }
            catch
            {
                ClearForgotPasswordBackground();
                BackColor = FormFallbackBack;
            }
        }

        private void ClearForgotPasswordBackground()
        {
            Image previous = BackgroundImage;
            BackgroundImage = null;
            if (!ReferenceEquals(previous, _ownedBackgroundImage))
            {
                previous?.Dispose();
            }

            _ownedBackgroundImage?.Dispose();
            _ownedBackgroundImage = null;
        }

        private static string FindForgotPasswordBackgroundPath()
        {
            string relative = Path.Combine("images", "forgot_password_bg.png");
            var tried = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string root in GetBackgroundSearchRoots())
            {
                if (string.IsNullOrWhiteSpace(root))
                {
                    continue;
                }

                string candidate = Path.GetFullPath(Path.Combine(root, relative));
                if (tried.Add(candidate) && File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static IEnumerable<string> GetBackgroundSearchRoots()
        {
            yield return Application.StartupPath;
            yield return AppDomain.CurrentDomain.BaseDirectory;

            string location = Assembly.GetExecutingAssembly().Location;
            if (!string.IsNullOrEmpty(location))
            {
                string dir = Path.GetDirectoryName(location);
                if (!string.IsNullOrEmpty(dir))
                {
                    yield return dir;
                }
            }

            foreach (string start in new[] { Application.StartupPath, AppDomain.CurrentDomain.BaseDirectory })
            {
                if (string.IsNullOrEmpty(start))
                {
                    continue;
                }

                yield return Path.GetFullPath(Path.Combine(start, "..", ".."));
                yield return Path.GetFullPath(Path.Combine(start, "..", "..", ".."));
            }

            yield return Environment.CurrentDirectory;
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

            if (!PasswordResetService.TrySendVerificationCode(email, out string errorMessage))
            {
                ShowFieldError(txtEmail, errorMessage);
                txtEmail.Focus();
                return;
            }

            MessageBox.Show(
                this,
                "A verification code has been sent to your email address.",
                "Verification Code Sent",
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
