using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ClinicVets
{
    public partial class RegisterEmployeeForm : Form
    {
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
            VetBackgroundHelper.ApplyVetBackground(this);
            ChromeTextPlate.WrapDirectTextBoxes(pnlCard);
            CenterCard();
            txtUsername.Focus();
        }

        private void CenterCard()
        {
            pnlCard.Left = (ClientSize.Width - pnlCard.Width) / 2;
            pnlCard.Top = (ClientSize.Height - pnlCard.Height) / 2;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Image img = BackgroundImage;
            BackgroundImage = null;
            img?.Dispose();
            base.OnFormClosed(e);
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            var errors = new List<string>();
            AddIfInvalid(errors, ValidateUsername(txtUsername.Text));
            AddIfInvalid(errors, ValidatePassword(txtPassword.Text));
            AddIfInvalid(errors, ValidateEmployeeNumber(txtEmployeeNumber.Text));
            AddIfInvalid(errors, ValidateEmail(txtEmail.Text));
            AddIfInvalid(errors, ValidateId(txtId.Text));
            AddIfInvalid(errors, ValidateRole(cmbRole));

            if (errors.Count > 0)
            {
                MessageBox.Show(
                    this,
                    string.Join(Environment.NewLine, errors),
                    "Register — validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show(
                this,
                "Employee registered successfully.",
                "Register",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private static void AddIfInvalid(List<string> errors, string errorMessage)
        {
            if (errorMessage != null)
            {
                errors.Add(errorMessage);
            }
        }

        private static string ValidateUsername(string username)
        {
            username = (username ?? string.Empty).Trim();
            if (username.Length < 6 || username.Length > 8)
            {
                return "Username must be between 6 and 8 characters.";
            }

            int digitCount = 0;
            foreach (char c in username)
            {
                if (char.IsDigit(c))
                {
                    digitCount++;
                }
                else if (!IsEnglishLetter(c))
                {
                    return "Username may only contain English letters (A–Z, a–z) and digits. Up to 2 digits are allowed; all other characters must be English letters.";
                }
            }

            if (digitCount > 2)
            {
                return "Username may contain at most 2 digits. All other characters must be English letters.";
            }

            return null;
        }

        private static bool IsEnglishLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        private static string ValidatePassword(string password)
        {
            if (password == null)
            {
                password = string.Empty;
            }

            if (password.Length < 8 || password.Length > 10)
            {
                return "Password must be between 8 and 10 characters.";
            }

            bool hasLetter = false;
            bool hasDigit = false;
            bool hasSpecial = false;
            const string allowedSpecials = "!$#,";

            foreach (char c in password)
            {
                if (IsEnglishLetter(c))
                {
                    hasLetter = true;
                }
                else if (char.IsDigit(c))
                {
                    hasDigit = true;
                }
                else if (allowedSpecials.IndexOf(c) >= 0)
                {
                    hasSpecial = true;
                }
            }

            if (!hasLetter)
            {
                return "Password must contain at least one English letter (A–Z, a–z).";
            }

            if (!hasDigit)
            {
                return "Password must contain at least one digit.";
            }

            if (!hasSpecial)
            {
                return "Password must contain at least one special character from: ! $ # ,";
            }

            return null;
        }

        private static string ValidateEmployeeNumber(string value)
        {
            value = (value ?? string.Empty).Trim();
            if (value.Length != 4)
            {
                return "Employee Number must be exactly 4 digits.";
            }

            foreach (char c in value)
            {
                if (!char.IsDigit(c))
                {
                    return "Employee Number must contain only digits (exactly 4).";
                }
            }

            return null;
        }

        private static string ValidateId(string value)
        {
            value = (value ?? string.Empty).Trim();
            if (value.Length != 9)
            {
                return "ID must be exactly 9 digits.";
            }

            foreach (char c in value)
            {
                if (!char.IsDigit(c))
                {
                    return "ID must contain only digits (exactly 9).";
                }
            }

            return null;
        }

        private static string ValidateEmail(string email)
        {
            email = email ?? string.Empty;
            if (!email.Contains("@"))
            {
                return "Email must contain an '@' character.";
            }

            return null;
        }

        private static string ValidateRole(ComboBox combo)
        {
            if (combo == null)
            {
                return "Role must be Vet or Secretary.";
            }

            if (combo.SelectedIndex < 0)
            {
                return "Please select a role: Vet or Secretary.";
            }

            string role = combo.SelectedItem?.ToString() ?? string.Empty;
            if (role != "Vet" && role != "Secretary")
            {
                return "Role must be Vet or Secretary.";
            }

            return null;
        }
    }
}
