using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ClinicVets
{
    public partial class CustomerSearchForm : Form
    {
        private readonly ExcelHelper _excelHelper = new ExcelHelper();
        private readonly List<Customer> _allCustomers = new List<Customer>();
        private ChromeTextPlate _searchPlate;

        public CustomerSearchForm()
            : this(null)
        {
        }

        public CustomerSearchForm(Form owner)
        {
            InitializeComponent();
            if (owner != null)
            {
                Owner = owner;
            }
        }

        private void CustomerSearchForm_Load(object sender, EventArgs e)
        {
            ThemeHelper.ApplyThemedShell(
                this,
                pnlCard,
                centerCardVertically: true,
                headerSubtitle: "Find customers by ID or phone",
                backgroundStyle: FormBackgroundStyle.SearchWorkspace);
            ThemeHelper.ApplyStandardLabels(lblTitle, lblSubtitle, lblSearchType, lblSearch);
            ThemeHelper.ApplyStandardButtons(btnSearch, btnClear, btnBack);
            btnClear.IsOutlineStyle = true;
            ClinicUiTheme.ApplyOutlineButton(btnClear);
            ClinicUiTheme.ApplyErrorLabel(lblSearchError);
            lblNoResults.Font = ClinicUiTheme.SubtitleFont;
            lblNoResults.ForeColor = ClinicUiTheme.BodyText;
            lblNoResults.BackColor = Color.Transparent;

            lblSubtitle.Text = "Search by Customer ID or Phone. Enter the full value for the selected type.";

            cmbSearchType.Items.Clear();
            cmbSearchType.Items.AddRange(new object[] { "Customer ID", "Phone" });
            cmbSearchType.SelectedIndex = 0;

            SetupSearchField();
            WrapSearchTypeCombo();
            LayoutSearchForm();
            SetupGrid();
            ReloadCustomersFromExcel();

            cmbSearchType.SelectedIndexChanged += SearchInput_Changed;
            txtSearch.TextChanged += SearchInput_Changed;
            txtSearch.Focus();
        }

        private void SetupSearchField()
        {
            txtSearch.ReadOnly = false;
            txtSearch.Enabled = true;
            txtSearch.TabStop = true;

            if (txtSearch.Parent is ChromeTextPlate existing)
            {
                _searchPlate = existing;
                return;
            }

            int tab = txtSearch.TabIndex;
            Point loc = txtSearch.Location;
            Size sz = txtSearch.Size;
            int z = pnlCard.Controls.GetChildIndex(txtSearch);

            _searchPlate = new ChromeTextPlate
            {
                Location = loc,
                Size = new Size(sz.Width, sz.Height + 4),
                TabIndex = tab,
                TabStop = true
            };

            pnlCard.Controls.Remove(txtSearch);
            pnlCard.Controls.Add(_searchPlate);
            pnlCard.Controls.SetChildIndex(_searchPlate, z);

            txtSearch.BorderStyle = BorderStyle.None;
            txtSearch.BackColor = ClinicUiTheme.FieldFill;
            txtSearch.ReadOnly = false;
            txtSearch.Enabled = true;
            txtSearch.TabIndex = 0;
            _searchPlate.Controls.Add(txtSearch);
            FitSearchTextBox();
        }

        private void WrapSearchTypeCombo()
        {
            if (cmbSearchType.Parent is ChromeTextPlate)
            {
                return;
            }

            int tab = cmbSearchType.TabIndex;
            Point loc = cmbSearchType.Location;
            Size sz = cmbSearchType.Size;
            int z = pnlCard.Controls.GetChildIndex(cmbSearchType);

            var plate = new ChromeTextPlate
            {
                Location = loc,
                Size = new Size(sz.Width, sz.Height + 4),
                TabIndex = tab,
                TabStop = false
            };

            pnlCard.Controls.Remove(cmbSearchType);
            pnlCard.Controls.Add(plate);
            pnlCard.Controls.SetChildIndex(plate, z);

            cmbSearchType.FlatStyle = FlatStyle.Flat;
            cmbSearchType.BackColor = ClinicUiTheme.FieldFill;
            cmbSearchType.TabIndex = 0;
            plate.Controls.Add(cmbSearchType);
            cmbSearchType.Location = new Point(plate.Padding.Left + 2, plate.Padding.Top + 2);
            cmbSearchType.Width = Math.Max(10, plate.ClientSize.Width - plate.Padding.Horizontal - 4);
            cmbSearchType.Height = Math.Max(10, plate.ClientSize.Height - plate.Padding.Vertical - 3);
        }

        private void FitSearchTextBox()
        {
            if (_searchPlate == null || txtSearch == null)
            {
                return;
            }

            txtSearch.ReadOnly = false;
            txtSearch.Enabled = true;
            txtSearch.Location = new Point(_searchPlate.Padding.Left + 2, _searchPlate.Padding.Top + 2);
            txtSearch.Width = Math.Max(10, _searchPlate.ClientSize.Width - _searchPlate.Padding.Horizontal - 4);
            txtSearch.Height = Math.Max(10, _searchPlate.ClientSize.Height - _searchPlate.Padding.Vertical - 3);
        }

        private void LayoutSearchForm()
        {
            int pad = pnlCard.Padding.Left;
            int contentW = 452;
            int y = 124;

            lblSearchType.SetBounds(pad, y, contentW, 23);
            y = lblSearchType.Bottom + 4;

            Control typeHost = cmbSearchType.Parent ?? cmbSearchType;
            typeHost.SetBounds(pad, y, contentW, 34);
            y = typeHost.Bottom + 10;

            lblSearch.SetBounds(pad, y, contentW, 23);
            y = lblSearch.Bottom + 4;

            _searchPlate.SetBounds(pad, y, contentW, 34);
            FitSearchTextBox();
            y = _searchPlate.Bottom + 2;

            lblSearchError.SetBounds(pad, y, contentW, 18);
            lblSearchError.BringToFront();
            y = lblSearchError.Bottom + 8;

            int half = (contentW - 8) / 2;
            btnSearch.SetBounds(pad, y, half, 40);
            btnClear.SetBounds(pad + half + 8, y, half, 40);
            y = btnSearch.Bottom + 10;

            lblNoResults.SetBounds(pad, y, contentW, 18);
            y = lblNoResults.Bottom + 6;

            dgvResults.SetBounds(pad, y, contentW, 200);
            y = dgvResults.Bottom + 12;

            btnBack.SetBounds(pad, y, contentW, 40);
            pnlCard.Height = btnBack.Bottom + pnlCard.Padding.Bottom + 8;

            _searchPlate.BringToFront();
            lblSearch.BringToFront();
            ThemeHelper.CenterCardInClient(this, pnlCard);
        }

        private void SearchInput_Changed(object sender, EventArgs e)
        {
            ClearSearchFeedback();
        }

        private void SetupGrid()
        {
            dgvResults.AutoGenerateColumns = false;
            dgvResults.Columns.Clear();
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerId", HeaderText = "Customer ID", FillWeight = 14 });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "FirstName", HeaderText = "First Name", FillWeight = 16 });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "LastName", HeaderText = "Last Name", FillWeight = 16 });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Phone", HeaderText = "Phone", FillWeight = 14 });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", FillWeight = 20 });
            dgvResults.Columns.Add(new DataGridViewTextBoxColumn { Name = "Address", HeaderText = "Address", FillWeight = 20 });
            dgvResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvResults.MultiSelect = false;
            dgvResults.RowHeadersVisible = false;
            dgvResults.AllowUserToAddRows = false;
            ClinicUiTheme.ApplyDataGridView(dgvResults);
        }

        private void ReloadCustomersFromExcel()
        {
            _allCustomers.Clear();
            try
            {
                foreach (Customer customer in _excelHelper.ReadCustomers())
                {
                    _allCustomers.Add(customer);
                }
            }
            catch
            {
                _allCustomers.Clear();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void PerformSearch()
        {
            ClearSearchFeedback();
            string term = (txtSearch.Text ?? string.Empty).Trim();
            string type = cmbSearchType.SelectedItem?.ToString() ?? string.Empty;

            string validationError = ValidateSearchInput(type, term);
            if (validationError != null)
            {
                ShowSearchError(validationError);
                txtSearch.Focus();
                txtSearch.SelectAll();
                return;
            }

            IEnumerable<Customer> results = FilterCustomers(type, term);
            BindResults(results);

            if (!dgvResults.Rows.Cast<DataGridViewRow>().Any())
            {
                lblNoResults.Text = "No matching customers found";
                lblNoResults.Visible = true;
            }
        }

        private static string ValidateSearchInput(string searchType, string term)
        {
            if (searchType == "Customer ID")
            {
                return ValidationHelper.ValidateIdNumber(term);
            }

            if (searchType == "Phone")
            {
                return ValidationHelper.ValidatePhone(term);
            }

            return ValidationHelper.RequiredMessage;
        }

        private IEnumerable<Customer> FilterCustomers(string searchType, string term)
        {
            if (searchType == "Customer ID")
            {
                return _allCustomers.Where(c =>
                    string.Equals((c.CustomerID ?? string.Empty).Trim(), term, StringComparison.OrdinalIgnoreCase));
            }

            if (searchType == "Phone")
            {
                return _allCustomers.Where(c =>
                    string.Equals((c.Phone ?? string.Empty).Trim(), term, StringComparison.Ordinal));
            }

            return Enumerable.Empty<Customer>();
        }

        private void BindResults(IEnumerable<Customer> customers)
        {
            dgvResults.Rows.Clear();
            foreach (Customer customer in customers)
            {
                dgvResults.Rows.Add(
                    customer.CustomerID ?? string.Empty,
                    customer.FirstName ?? string.Empty,
                    customer.LastName ?? string.Empty,
                    customer.Phone ?? string.Empty,
                    customer.Email ?? string.Empty,
                    customer.Address ?? string.Empty);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cmbSearchType.SelectedIndex = 0;
            ClearSearchFeedback();
            dgvResults.Rows.Clear();
            txtSearch.Focus();
        }

        private void ClearSearchFeedback()
        {
            lblSearchError.Visible = false;
            lblSearchError.Text = string.Empty;
            lblNoResults.Visible = false;
            lblNoResults.Text = string.Empty;
            if (_searchPlate != null)
            {
                _searchPlate.IsInvalid = false;
            }
        }

        private void ShowSearchError(string message)
        {
            lblSearchError.Text = message;
            lblSearchError.Visible = true;
            if (_searchPlate != null)
            {
                _searchPlate.IsInvalid = true;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            VetBackgroundHelper.ClearBackgroundImage(this);
            base.OnFormClosed(e);
        }
    }
}
