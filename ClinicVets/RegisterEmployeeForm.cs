using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ClinicVets
{
    public partial class RegisterEmployeeForm : Form
    {
        private static readonly Color TitleColor = Color.FromArgb(21, 101, 192);
        private static readonly Color SubtitleColor = Color.FromArgb(71, 95, 120);
        private static readonly Color LabelColor = Color.FromArgb(25, 118, 210);
        private static readonly Color LinkColor = Color.FromArgb(25, 118, 210);
        private static readonly Color LinkHoverColor = Color.FromArgb(0, 151, 167);
        private static readonly Color CardSurface = CardPanel.RegisterCardFill;

        private const int InnerX = 45;
        private const int FieldWidth = 470;
        private const int FieldHeight = 32;
        private const int ErrorHeight = 16;

        private ValidationFieldBinder _validation;

        public RegisterEmployeeForm()
            : this(null)
        {
        }

        public RegisterEmployeeForm(Form1 loginForm)
        {
            InitializeComponent();
            if (loginForm != null)
            {
                Owner = loginForm;
            }
        }

        private void RegisterEmployeeForm_Load(object sender, EventArgs e)
        {
            WinFormsUi.SetDoubleBuffered(this);
            VetBackgroundHelper.ApplyRegisterBackground(this);

            pnlCard.UseRegisterLightStyle = true;
            pnlCard.ShowCornerDecorations = false;
            pnlCard.CornerRadius = CardPanel.RegisterCornerRadius;
            pnlCard.Location = new Point(630, 55);
            pnlCard.Size = new Size(560, 760);
            pnlCard.Padding = new Padding(0);
            pnlCard.BackColor = CardSurface;

            ApplyRegisterTypography();

            btnRegister.UseLoginLightStyle = true;
            btnRegister.IsOutlineStyle = false;
            btnRegister.CornerRadius = 8;
            btnBack.UseLoginLightStyle = true;
            btnBack.IsOutlineStyle = true;
            btnBack.CornerRadius = 8;

            ChromeTextPlate.WrapDirectTextBoxes(pnlCard);
            ApplyRegisterFieldChrome();

            LinkLabel passwordToggle = PasswordVisibilityHelper.Attach(txtPassword);
            if (passwordToggle != null)
            {
                passwordToggle.LinkColor = LinkColor;
                passwordToggle.ActiveLinkColor = LinkHoverColor;
                passwordToggle.VisitedLinkColor = LinkColor;
                passwordToggle.BackColor = CardSurface;
            }

            SetupValidation();
            ApplyRegisterLabelSurfaces();
            LayoutRegisterControls();
            txtUsername.Focus();
        }

        private void ApplyRegisterTypography()
        {
            lblTitle.Font = new Font("Segoe UI", 19F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.ForeColor = TitleColor;

            lblSubtitle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point);
            lblSubtitle.ForeColor = SubtitleColor;

            foreach (Label label in new[]
            {
                lblUsername,
                lblPassword,
                lblEmployeeNumber,
                lblEmail,
                lblId,
                lblRole
            })
            {
                label.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
                label.ForeColor = LabelColor;
            }

            btnRegister.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point);
            btnBack.Font = btnRegister.Font;
        }

        private void ApplyRegisterLabelSurfaces()
        {
            foreach (Control control in pnlCard.Controls)
            {
                if (control is Label label)
                {
                    label.BackColor = CardSurface;
                }
            }
        }

        private void ApplyRegisterFieldChrome()
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

                foreach (ComboBox combo in plate.Controls.OfType<ComboBox>())
                {
                    combo.ForeColor = Color.FromArgb(33, 52, 72);
                    combo.BackColor = Color.White;
                }
            }
        }

        private void LayoutRegisterControls()
        {
            lblTitle.SetBounds(InnerX, 35, FieldWidth, 45);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            lblSubtitle.SetBounds(InnerX, 85, FieldWidth, 35);
            lblSubtitle.TextAlign = ContentAlignment.TopCenter;

            LayoutCaption(lblUsername, 135);
            LayoutFieldHost(GetFieldHost(txtUsername), 160);

            LayoutCaption(lblPassword, 215);
            LayoutFieldHost(GetFieldHost(txtPassword), 240);

            LayoutCaption(lblEmployeeNumber, 295);
            LayoutFieldHost(GetFieldHost(txtEmployeeNumber), 320);

            LayoutCaption(lblEmail, 375);
            LayoutFieldHost(GetFieldHost(txtEmail), 400);

            LayoutCaption(lblId, 455);
            LayoutFieldHost(GetFieldHost(txtId), 480);

            LayoutCaption(lblRole, 535);
            LayoutFieldHost(GetFieldHost(cmbRole), 560);

            if (_validation != null)
            {
                PositionErrorLabel(_validation.Entries.First(e => e.InputControl == txtUsername), 160);
                PositionErrorLabel(_validation.Entries.First(e => e.InputControl == txtPassword), 240);
                PositionErrorLabel(_validation.Entries.First(e => e.InputControl == txtEmployeeNumber), 320);
                PositionErrorLabel(_validation.Entries.First(e => e.InputControl == txtEmail), 400);
                PositionErrorLabel(_validation.Entries.First(e => e.InputControl == txtId), 480);
                PositionErrorLabel(_validation.Entries.First(e => e.InputControl == cmbRole), 560);
            }

            btnRegister.SetBounds(InnerX, 625, FieldWidth, 48);
            btnBack.SetBounds(InnerX, 690, FieldWidth, 48);
        }

        private void LayoutCaption(Label label, int y)
        {
            label.AutoSize = false;
            label.SetBounds(InnerX, y, FieldWidth, 22);
        }

        private void LayoutFieldHost(Control host, int y)
        {
            host.SetBounds(InnerX, y, FieldWidth, FieldHeight);
            AlignChromePlateInner(host);
        }

        private static void PositionErrorLabel(ValidationFieldBinder.FieldEntry entry, int fieldY)
        {
            entry.ErrorLabel.SetBounds(InnerX, fieldY + FieldHeight + 2, FieldWidth, ErrorHeight);
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
                inner.Location = new Point(plate.Padding.Left + 2, plate.Padding.Top + 2);
                inner.Width = Math.Max(10, plate.ClientSize.Width - plate.Padding.Horizontal - 4);
                inner.Height = Math.Max(10, plate.ClientSize.Height - plate.Padding.Vertical - 4);
            }
        }

        private void SetupValidation()
        {
            _validation = new ValidationFieldBinder(pnlCard);
            _validation.BindTextBox(txtUsername, ValidationHelper.ValidateUsername, lblUsername);
            _validation.BindTextBox(txtPassword, ValidationHelper.ValidatePassword, lblPassword);
            _validation.BindTextBox(txtEmployeeNumber, ValidationHelper.ValidateEmployeeNumber, lblEmployeeNumber);
            _validation.BindTextBox(txtEmail, ValidationHelper.ValidateEmail, lblEmail);
            _validation.BindTextBox(txtId, ValidationHelper.ValidateIdNumber, lblId);
            _validation.BindComboBox(cmbRole, ValidationHelper.ValidateRole, lblRole);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            VetBackgroundHelper.ClearBackgroundImage(this);
            base.OnFormClosed(e);
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (!_validation.ValidateAll())
            {
                LayoutRegisterControls();
                return;
            }

            var employee = new Employee
            {
                EmployeeID = txtEmployeeNumber.Text.Trim(),
                Username = txtUsername.Text.Trim(),
                Password = txtPassword.Text,
                Email = txtEmail.Text.Trim(),
                NationalID = txtId.Text.Trim(),
                Role = cmbRole.SelectedItem?.ToString() ?? string.Empty
            };

            LoginAuthService.RegisterEmployee(employee);

            Form1 loginForm = Owner as Form1;
            Hide();
            using (var successForm = new RegistrationSuccessForm(loginForm))
            {
                successForm.ShowDialog(loginForm);
            }

            Close();
        }
    }
}
