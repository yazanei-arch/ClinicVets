using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ClinicVets
{
    public partial class CustomerManagementForm : Form
    {
        private readonly ExcelHelper _excelHelper = new ExcelHelper();

        public CustomerManagementForm()
            : this(null)
        {
        }

        public CustomerManagementForm(Form owner)
        {
            InitializeComponent();
            if (owner != null)
            {
                Owner = owner;
            }
        }

        private void CustomerManagementForm_Load(object sender, EventArgs e)
        {
            WinFormsUi.SetDoubleBuffered(this);
            VetBackgroundHelper.ApplyVetBackground(this);
            ChromeTextPlate.WrapDirectTextBoxes(pnlCard);
            CenterCard();
            SetupGrid();
            ReloadCustomersFromExcel();
            txtFullName.Focus();
        }

        private void SetupGrid()
        {
            dgvCustomers.AutoGenerateColumns = false;
            dgvCustomers.Columns.Clear();
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "FullName", HeaderText = "Full Name", FillWeight = 30 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerId", HeaderText = "ID", FillWeight = 15 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Phone", HeaderText = "Phone", FillWeight = 20 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", FillWeight = 35 });
            dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.RowHeadersVisible = false;
            dgvCustomers.AllowUserToAddRows = false;
            dgvCustomers.BackgroundColor = Color.FromArgb(252, 253, 255);
            dgvCustomers.BorderStyle = BorderStyle.None;
            dgvCustomers.EnableHeadersVisualStyles = true;
        }

        private void ReloadCustomersFromExcel()
        {
            dgvCustomers.Rows.Clear();
            try
            {
                foreach (Customer customer in _excelHelper.ReadCustomers())
                {
                    dgvCustomers.Rows.Add(
                        customer.FullName ?? string.Empty,
                        GridIdDisplay(customer),
                        customer.Phone ?? string.Empty,
                        customer.Email ?? string.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Could not load customers from Excel." + Environment.NewLine + ex.Message,
                    "Customer — data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private static string GridIdDisplay(Customer customer)
        {
            if (!string.IsNullOrWhiteSpace(customer.IDNumber))
            {
                return customer.IDNumber.Trim();
            }

            return customer.CustomerID ?? string.Empty;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Image img = BackgroundImage;
            BackgroundImage = null;
            img?.Dispose();
            base.OnFormClosed(e);
        }

        private void CenterCard()
        {
            pnlCard.Left = (ClientSize.Width - pnlCard.Width) / 2;
            pnlCard.Top = (ClientSize.Height - pnlCard.Height) / 2;
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            var errors = new List<string>();
            AddIfInvalid(errors, ValidateFullName(txtFullName.Text));
            AddIfInvalid(errors, ValidateCustomerId(txtCustomerId.Text));
            AddIfInvalid(errors, ValidatePhone(txtPhone.Text));
            AddIfInvalid(errors, ValidateEmail(txtEmail.Text));

            if (errors.Count > 0)
            {
                MessageBox.Show(
                    this,
                    string.Join(Environment.NewLine, errors),
                    "Customer — validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var customer = new Customer
            {
                CustomerID = Guid.NewGuid().ToString("N"),
                FullName = txtFullName.Text.Trim(),
                IDNumber = txtCustomerId.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim()
            };

            try
            {
                _excelHelper.AppendCustomer(customer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Could not save the customer to Excel. Close the workbook if it is open in Excel, then try again."
                    + Environment.NewLine + Environment.NewLine + ex.Message,
                    "Customer — save",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            dgvCustomers.Rows.Add(
                customer.FullName,
                customer.IDNumber,
                customer.Phone,
                customer.Email);

            MessageBox.Show(this, "Customer added to the list.", "Customer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Search will be connected to your data source later.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnShowPets_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Pet list for the selected customer will open here later.", "Customer pets", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void AddIfInvalid(List<string> errors, string message)
        {
            if (message != null)
            {
                errors.Add(message);
            }
        }

        private static string ValidateFullName(string value)
        {
            value = (value ?? string.Empty).Trim();
            if (value.Length == 0)
            {
                return "Full name is required.";
            }

            bool hasLetter = false;
            foreach (char c in value)
            {
                if (c == ' ')
                {
                    continue;
                }

                if (IsEnglishLetter(c))
                {
                    hasLetter = true;
                }
                else
                {
                    return "Full name may only contain English letters and spaces.";
                }
            }

            if (!hasLetter)
            {
                return "Full name must include at least one letter.";
            }

            return null;
        }

        private static bool IsEnglishLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        private static string ValidateCustomerId(string value)
        {
            value = (value ?? string.Empty).Trim();
            if (value.Length != 9)
            {
                return "Customer ID must be exactly 9 digits.";
            }

            foreach (char c in value)
            {
                if (!char.IsDigit(c))
                {
                    return "Customer ID must contain only digits (exactly 9).";
                }
            }

            return null;
        }

        private static string ValidatePhone(string value)
        {
            value = (value ?? string.Empty).Trim();
            if (value.Length < 7 || value.Length > 15)
            {
                return "Phone must be between 7 and 15 digits.";
            }

            foreach (char c in value)
            {
                if (!char.IsDigit(c))
                {
                    return "Phone may only contain digits.";
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
    }
}
