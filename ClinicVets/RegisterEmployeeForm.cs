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

        private const int MarginVertical = 18;
        private const int MarginRight = 22;
        private const int PanelShiftLeft = 50;
        private const float LeftBrandingRatio = 0.44f;
        private const int MaxPanelWidth = 450;
        private const int MinPanelWidth = 340;
        private const int InnerPad = 26;
        private const int CaptionHeight = 20;
        private const int FieldHeight = 30;
        private const int ErrorHeight = 14;
        private const int CaptionToFieldGap = 4;
        private const int FieldRowGap = 6;
        private const int ButtonHeight = 42;
        private const int ButtonGap = 10;
        private const int TitleHeight = 32;
        private const int SubtitleHeight = 22;
        private const int HeaderBottomGap = 10;

        private ValidationFieldBinder _validation;
        private LinkLabel _passwordToggle;

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
            ClinicFormLayout.ApplyStandard(this);
            WinFormsUi.SetDoubleBuffered(this);
            VetBackgroundHelper.ApplyRegisterBackground(this);
            BackgroundImageLayout = ImageLayout.Zoom;

            pnlCard.UseRegisterLightStyle = true;
            pnlCard.ShowCornerDecorations = false;
            pnlCard.CornerRadius = CardPanel.RegisterCornerRadius;
            pnlCard.Padding = new Padding(0);
            pnlCard.BackColor = CardSurface;
            pnlCard.AutoScroll = false;

            ApplyRegisterTypography();

            btnRegister.UseLoginLightStyle = true;
            btnRegister.IsOutlineStyle = false;
            btnRegister.CornerRadius = 8;
            btnBack.UseLoginLightStyle = true;
            btnBack.IsOutlineStyle = true;
            btnBack.CornerRadius = 8;

            ChromeTextPlate.WrapDirectTextBoxes(pnlCard);
            ApplyRegisterFieldChrome();

            _passwordToggle = PasswordVisibilityHelper.Attach(txtPassword);
            if (_passwordToggle != null)
            {
                _passwordToggle.LinkColor = LinkColor;
                _passwordToggle.ActiveLinkColor = LinkHoverColor;
                _passwordToggle.VisitedLinkColor = LinkColor;
                _passwordToggle.BackColor = CardSurface;
            }

            SetupValidation();
            ApplyRegisterLabelSurfaces();
            LayoutRegisterPanelAndControls();
            txtUsername.Focus();
        }

        private void RegisterEmployeeForm_Resize(object sender, EventArgs e)
        {
            if (ClientSize.Width < 200 || pnlCard == null)
            {
                return;
            }

            LayoutRegisterPanelAndControls();
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
                lblFullName,
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

        private void LayoutRegisterPanelAndControls()
        {
            int clientW = ClientSize.Width;
            int clientH = ClientSize.Height;

            int leftReserve = Math.Max(360, (int)(clientW * LeftBrandingRatio));
            int panelWidth = Math.Min(MaxPanelWidth, clientW - leftReserve - MarginRight);
            panelWidth = Math.Max(MinPanelWidth, panelWidth);

            int fieldWidth = panelWidth - (InnerPad * 2);
            int rowStride = CaptionHeight + CaptionToFieldGap + FieldHeight + ErrorHeight + FieldRowGap;
            int buttonBlock = ButtonHeight + ButtonGap + ButtonHeight;
            int headerBlock = TitleHeight + 6 + SubtitleHeight + HeaderBottomGap;
            int contentHeight = headerBlock + (rowStride * 7) + buttonBlock + (InnerPad * 2);

            int maxPanelHeight = clientH - (MarginVertical * 2);
            int panelHeight = Math.Min(contentHeight, maxPanelHeight);
            int panelLeft = clientW - panelWidth - MarginRight - PanelShiftLeft;
            int panelTop = Math.Max(MarginVertical, (clientH - panelHeight) / 2);

            pnlCard.SetBounds(panelLeft, panelTop, panelWidth, panelHeight);
            pnlCard.AutoScroll = contentHeight > maxPanelHeight;

            int y = InnerPad;

            lblTitle.SetBounds(InnerPad, y, fieldWidth, TitleHeight);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            y += TitleHeight + 6;

            lblSubtitle.SetBounds(InnerPad, y, fieldWidth, SubtitleHeight);
            lblSubtitle.TextAlign = ContentAlignment.TopCenter;
            y += SubtitleHeight + HeaderBottomGap;

            y = LayoutFieldRow(lblUsername, txtUsername, y, fieldWidth);
            y = LayoutFieldRow(lblFullName, txtFullName, y, fieldWidth);
            y = LayoutFieldRow(lblPassword, txtPassword, y, fieldWidth);
            y = LayoutFieldRow(lblEmployeeNumber, txtEmployeeNumber, y, fieldWidth);
            y = LayoutFieldRow(lblEmail, txtEmail, y, fieldWidth);
            y = LayoutFieldRow(lblId, txtId, y, fieldWidth);
            y = LayoutFieldRow(lblRole, cmbRole, y, fieldWidth);

            y += 6;
            btnRegister.SetBounds(InnerPad, y, fieldWidth, ButtonHeight);
            y += ButtonHeight + ButtonGap;
            btnBack.SetBounds(InnerPad, y, fieldWidth, ButtonHeight);
        }

        private int LayoutFieldRow(Label caption, Control input, int captionY, int fieldWidth)
        {
            caption.AutoSize = false;
            caption.SetBounds(InnerPad, captionY, fieldWidth, CaptionHeight);

            int fieldY = captionY + CaptionHeight + CaptionToFieldGap;
            Control host = GetFieldHost(input);
            host.SetBounds(InnerPad, fieldY, fieldWidth, FieldHeight);
            AlignChromePlateInner(host);
            if (input == txtPassword && host is ChromeTextPlate passwordPlate)
            {
                PasswordVisibilityHelper.ApplyToggleLayout(passwordPlate, txtPassword, _passwordToggle);
            }

            if (_validation != null)
            {
                ValidationFieldBinder.FieldEntry entry = _validation.Entries.FirstOrDefault(e => e.InputControl == input);
                if (entry != null)
                {
                    entry.ErrorLabel.SetBounds(
                        InnerPad,
                        fieldY + FieldHeight + 2,
                        fieldWidth,
                        ErrorHeight);
                }
            }

            return captionY + CaptionHeight + CaptionToFieldGap + FieldHeight + ErrorHeight + FieldRowGap;
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
                if (inner is LinkLabel)
                {
                    continue;
                }

                inner.Location = new Point(plate.Padding.Left + 2, plate.Padding.Top + 2);
                inner.Width = Math.Max(10, plate.ClientSize.Width - plate.Padding.Horizontal - 4);
                inner.Height = Math.Max(10, plate.ClientSize.Height - plate.Padding.Vertical - 4);
            }
        }

        private void SetupValidation()
        {
            _validation = new ValidationFieldBinder(pnlCard);
            _validation.BindTextBox(txtUsername, ValidationHelper.ValidateUsername, lblUsername);
            _validation.BindTextBox(txtFullName, ValidationHelper.ValidateName, lblFullName);
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
                LayoutRegisterPanelAndControls();
                return;
            }

            var employee = new Employee
            {
                EmployeeID = txtEmployeeNumber.Text.Trim(),
                Username = txtUsername.Text.Trim(),
                Password = txtPassword.Text,
                Email = txtEmail.Text.Trim(),
                NationalID = txtId.Text.Trim(),
                Role = cmbRole.SelectedItem?.ToString() ?? string.Empty,
                FullName = txtFullName.Text.Trim()
            };

            try
            {
                LoginAuthService.RegisterEmployee(employee);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    "Registration — save",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                string message = ExcelHelper.IsWorkbookLockedException(ex)
                    ? ExcelFileManager.WorkbookLockedMessage
                    : "Could not save the employee to Excel."
                      + Environment.NewLine + Environment.NewLine + ex.Message;

                MessageBox.Show(
                    this,
                    message,
                    "Registration — save",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

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
