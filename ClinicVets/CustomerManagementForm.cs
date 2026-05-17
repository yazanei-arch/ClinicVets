using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ClinicVets
{
    public partial class CustomerManagementForm : Form
    {
        private readonly ExcelHelper _excelHelper = new ExcelHelper();
        private readonly List<Customer> _allCustomers = new List<Customer>();
        private ValidationFieldBinder _validation;
        private int _fieldsBottom;

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
            ThemeHelper.ApplyThemedShell(
                this,
                pnlCard,
                centerCardVertically: false,
                headerSubtitle: "Customer records & appointments",
                backgroundStyle: FormBackgroundStyle.DashboardWorkspace);
            ThemeHelper.ApplyStandardLabels(
                lblTitle,
                lblSubtitle,
                lblCustomerId,
                lblFirstName,
                lblLastName,
                lblPhone,
                lblEmail,
                lblAddress);
            ThemeHelper.ApplyStandardButtons(
                btnAddCustomer,
                btnUpdateCustomer,
                btnDeleteCustomer,
                btnOpenSearch,
                btnBack);
            btnOpenSearch.IsOutlineStyle = true;
            ClinicUiTheme.ApplyOutlineButton(btnOpenSearch);

            pnlCard.AutoScroll = true;
            ChromeTextPlate.WrapDirectTextBoxes(pnlCard);
            SetupValidation();
            SetupGrid();
            ReloadCustomersFromExcel();
            FitCustomerLayout();
            Resize += CustomerManagementForm_Resize;
            txtCustomerId.Focus();
        }

        private void CustomerManagementForm_Resize(object sender, EventArgs e)
        {
            FitCustomerLayout();
        }

        private void SetupValidation()
        {
            _validation = new ValidationFieldBinder(pnlCard);

            ValidationFieldBinder.FieldEntry customerId = _validation.BindTextBox(txtCustomerId, ValidationHelper.ValidateIdNumber, lblCustomerId);
            ValidationFieldBinder.FieldEntry firstName = _validation.BindTextBox(txtFirstName, ValidationHelper.ValidateName, lblFirstName);
            ValidationFieldBinder.FieldEntry lastName = _validation.BindTextBox(txtLastName, ValidationHelper.ValidateName, lblLastName);
            ValidationFieldBinder.FieldEntry phone = _validation.BindTextBox(txtPhone, ValidationHelper.ValidatePhone, lblPhone);
            ValidationFieldBinder.FieldEntry email = _validation.BindTextBox(txtEmail, ValidationHelper.ValidateEmail, lblEmail);
            ValidationFieldBinder.FieldEntry address = _validation.BindTextBox(txtAddress, ValidationHelper.ValidateAddress, lblAddress);

            _validation.ReflowTwoColumn(
                88,
                44,
                420,
                16,
                new[] { customerId, lastName, email },
                new[] { firstName, phone, address },
                872,
                4,
                4);

            _fieldsBottom = Math.Max(address.HostControl.Bottom, email.HostControl.Bottom);
            foreach (ValidationFieldBinder.FieldEntry entry in _validation.Entries)
            {
                _fieldsBottom = Math.Max(_fieldsBottom, entry.ErrorLabel.Bottom);
            }
        }

        private void FitCustomerLayout()
        {
            int pad = pnlCard.Padding.Left;
            int contentW = Math.Max(500, pnlCard.ClientSize.Width - (pad * 2));
            int headerBottom = ThemeHelper.GetHeaderBottom(this);
            int cardTop = headerBottom + 8;
            int cardHeight = Math.Max(400, ClientSize.Height - cardTop - 12);
            int cardLeft = Math.Max(12, (ClientSize.Width - Math.Min(960, ClientSize.Width - 24)) / 2);
            int cardWidth = Math.Min(960, ClientSize.Width - 24);

            pnlCard.SetBounds(cardLeft, cardTop, cardWidth, cardHeight);

            lblTitle.SetBounds(pad, 20, contentW, 34);
            lblSubtitle.SetBounds(pad, 54, contentW, 26);

            int y = _fieldsBottom + 8;
            int actionGap = 8;
            int actionW = (contentW - (actionGap * 2)) / 3;
            btnAddCustomer.SetBounds(pad, y, actionW, 40);
            btnUpdateCustomer.SetBounds(pad + actionW + actionGap, y, actionW, 40);
            btnDeleteCustomer.SetBounds(pad + (actionW + actionGap) * 2, y, actionW, 40);

            y = btnAddCustomer.Bottom + 8;
            int gridHeight = GetGridHeight(cardHeight, y, pad);
            dgvCustomers.SetBounds(pad, y, contentW, gridHeight);

            y = dgvCustomers.Bottom + 8;
            int half = (contentW - actionGap) / 2;
            btnOpenSearch.SetBounds(pad, y, half, 40);
            btnBack.SetBounds(pad + half + actionGap, y, half, 40);

            int scrollHeight = btnBack.Bottom + pnlCard.Padding.Bottom + 8;
            pnlCard.AutoScrollMinSize = new Size(cardWidth, scrollHeight);
        }

        private static int GetGridHeight(int cardHeight, int gridTop, int pad)
        {
            int reservedBottom = 56 + pad;
            int available = cardHeight - gridTop - reservedBottom;
            return Math.Max(120, Math.Min(200, available));
        }

        private void SetupGrid()
        {
            dgvCustomers.AutoGenerateColumns = false;
            dgvCustomers.Columns.Clear();
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerId", HeaderText = "Customer ID", FillWeight = 14 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "FirstName", HeaderText = " First Name", FillWeight = 16 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "LastName", HeaderText = "Last Name", FillWeight = 16 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Phone", HeaderText = "Phone", FillWeight = 14 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", FillWeight = 20 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Address", HeaderText = "Address", FillWeight = 20 });
            dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.RowHeadersVisible = false;
            dgvCustomers.AllowUserToAddRows = false;
            ClinicUiTheme.ApplyDataGridView(dgvCustomers);
            dgvCustomers.SelectionChanged += dgvCustomers_SelectionChanged;
        }

        private void dgvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow == null || dgvCustomers.CurrentRow.IsNewRow)
            {
                return;
            }

            _validation.ClearAll();

            DataGridViewRow row = dgvCustomers.CurrentRow;
            txtCustomerId.Text = Convert.ToString(row.Cells["CustomerId"].Value) ?? string.Empty;
            txtFirstName.Text = Convert.ToString(row.Cells["FirstName"].Value) ?? string.Empty;
            txtLastName.Text = Convert.ToString(row.Cells["LastName"].Value) ?? string.Empty;
            txtPhone.Text = Convert.ToString(row.Cells["Phone"].Value) ?? string.Empty;
            txtEmail.Text = Convert.ToString(row.Cells["Email"].Value) ?? string.Empty;
            txtAddress.Text = Convert.ToString(row.Cells["Address"].Value) ?? string.Empty;
        }

        private void ReloadCustomersFromExcel()
        {
            _allCustomers.Clear();
            dgvCustomers.Rows.Clear();
            try
            {
                foreach (Customer customer in _excelHelper.ReadCustomers())
                {
                    _allCustomers.Add(customer);
                }

                BindCustomersToGrid(_allCustomers);
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

        private void BindCustomersToGrid(IEnumerable<Customer> customers)
        {
            dgvCustomers.Rows.Clear();
            foreach (Customer customer in customers)
            {
                dgvCustomers.Rows.Add(
                    customer.CustomerID ?? string.Empty,
                    customer.FirstName ?? string.Empty,
                    customer.LastName ?? string.Empty,
                    customer.Phone ?? string.Empty,
                    customer.Email ?? string.Empty,
                    customer.Address ?? string.Empty);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            VetBackgroundHelper.ClearBackgroundImage(this);
            base.OnFormClosed(e);
        }

        private bool TryGetValidCustomer(out Customer customer)
        {
            customer = null;
            if (!_validation.ValidateAll())
            {
                return false;
            }

            customer = new Customer
            {
                CustomerID = txtCustomerId.Text.Trim(),
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Address = txtAddress.Text.Trim()
            };
            return true;
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            if (!TryGetValidCustomer(out Customer customer))
            {
                return;
            }

            if (_allCustomers.Any(c => string.Equals(c.CustomerID, customer.CustomerID, StringComparison.OrdinalIgnoreCase)))
            {
                var customerIdEntry = _validation.Entries.First(entry => entry.InputControl == txtCustomerId);
                customerIdEntry.ErrorLabel.Text = "A customer with this ID already exists.";
                customerIdEntry.ErrorLabel.Visible = true;
                if (customerIdEntry.HostControl is ChromeTextPlate plate)
                {
                    plate.IsInvalid = true;
                }

                txtCustomerId.Focus();
                return;
            }

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

            _allCustomers.Add(customer);
            BindCustomersToGrid(_allCustomers);
            MessageBox.Show(this, "Customer added successfully.", "Customer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnUpdateCustomer_Click(object sender, EventArgs e)
        {
            if (!TryGetValidCustomer(out Customer customer))
            {
                return;
            }

            try
            {
                _excelHelper.UpdateCustomer(customer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Could not update the customer in Excel. Close the workbook if it is open, then try again."
                    + Environment.NewLine + Environment.NewLine + ex.Message,
                    "Customer — update",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            Customer existing = _allCustomers.FirstOrDefault(c =>
                string.Equals(c.CustomerID, customer.CustomerID, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.FirstName = customer.FirstName;
                existing.LastName = customer.LastName;
                existing.Phone = customer.Phone;
                existing.Email = customer.Email;
                existing.Address = customer.Address;
            }
            else
            {
                _allCustomers.Add(customer);
            }

            BindCustomersToGrid(_allCustomers);
            MessageBox.Show(this, "Customer updated successfully.", "Customer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDeleteCustomer_Click(object sender, EventArgs e)
        {
            _validation.ClearField(txtCustomerId);
            string customerId = txtCustomerId.Text.Trim();
            string idError = ValidationHelper.ValidateIdNumber(customerId);
            if (idError != null)
            {
                var customerIdEntry = _validation.Entries.First(entry => entry.InputControl == txtCustomerId);
                customerIdEntry.ErrorLabel.Text = idError;
                customerIdEntry.ErrorLabel.Visible = true;
                if (customerIdEntry.HostControl is ChromeTextPlate plate)
                {
                    plate.IsInvalid = true;
                }

                txtCustomerId.Focus();
                return;
            }

            DialogResult confirm = MessageBox.Show(
                this,
                "Delete customer " + customerId + "?",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _excelHelper.DeleteCustomer(customerId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Could not delete the customer from Excel. Close the workbook if it is open, then try again."
                    + Environment.NewLine + Environment.NewLine + ex.Message,
                    "Customer — delete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            _allCustomers.RemoveAll(c => string.Equals(c.CustomerID, customerId, StringComparison.OrdinalIgnoreCase));
            BindCustomersToGrid(_allCustomers);
            ClearFields();
            MessageBox.Show(this, "Customer deleted successfully.", "Customer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnOpenSearch_Click(object sender, EventArgs e)
        {
            using (var searchForm = new CustomerSearchForm(this))
            {
                searchForm.ShowDialog(this);
            }
        }

        private void ClearFields()
        {
            _validation.ClearAll();
            txtCustomerId.Clear();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            dgvCustomers.ClearSelection();
        }
    }
}
