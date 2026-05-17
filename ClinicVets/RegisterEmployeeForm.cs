using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClinicVets
{
    public partial class RegisterEmployeeForm : Form
    {
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
            ThemeHelper.ApplyThemedShell(
                this,
                pnlCard,
                showCardPaws: true,
                centerCardVertically: true,
                headerSubtitle: "Create your staff account",
                backgroundStyle: FormBackgroundStyle.RegisterFocus);
            ThemeHelper.ApplyStandardLabels(
                lblTitle,
                lblSubtitle,
                lblUsername,
                lblPassword,
                lblEmployeeNumber,
                lblEmail,
                lblId,
                lblRole);
            ThemeHelper.ApplyStandardButtons(btnRegister, btnBack);

            ChromeTextPlate.WrapDirectTextBoxes(pnlCard);
            PasswordVisibilityHelper.Attach(txtPassword);
            SetupValidation();
            txtUsername.Focus();
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

            _validation.ReflowSingleColumn(124, 44, 452, btnRegister, btnBack);
            ThemeHelper.CenterCardInClient(this, pnlCard);
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

            MessageBox.Show(
                this,
                "Employee registered successfully.",
                "Register",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}
