using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ClinicVets
{
    public partial class CustomerSearchForm : Form
    {
        private static readonly Color TitleColor = Color.FromArgb(21, 101, 192);
        private static readonly Color SubtitleColor = Color.FromArgb(71, 95, 120);
        private static readonly Color HintColor = Color.FromArgb(100, 120, 140);
        private static readonly Color LabelAccent = Color.FromArgb(25, 118, 210);
        private static readonly Color CardSurface = CardPanel.RegisterCardFill;
        private static readonly Color FormFallbackBack = Color.FromArgb(232, 244, 252);

        private const int ContentWidth = 580;
        private const int ContentLeft = 70;

        private readonly ExcelHelper _excelHelper = new ExcelHelper();
        private readonly List<Customer> _allCustomers = new List<Customer>();
        private Image _ownedBackgroundImage;

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
            WinFormsUi.SetDoubleBuffered(this);
            ApplySearchCustomerBackground();

            pnlContent.BackColor = Color.Transparent;

            ApplySearchTypography();
            ApplyLightChrome();

            lblTitle.Text = "Search Customer";
            lblSubtitle.Text = "Find your customer information quickly and easily.";
            lblHint.Text = "Search by customer ID or phone number.";

            cmbSearchType.Items.Clear();
            cmbSearchType.Items.AddRange(new object[] { "Customer ID", "Phone" });
            cmbSearchType.SelectedIndex = 0;

            btnSearch.UseLoginLightStyle = true;
            btnSearch.IsOutlineStyle = false;
            btnSearch.CornerRadius = 8;

            btnClear.UseLoginLightStyle = true;
            btnClear.IsOutlineStyle = true;
            btnClear.CornerRadius = 8;

            btnBack.UseLoginLightStyle = true;
            btnBack.IsOutlineStyle = true;
            btnBack.CornerRadius = 8;

            pnlResults.UseRegisterLightStyle = true;
            pnlResults.ShowCornerDecorations = false;
            pnlResults.CornerRadius = CardPanel.RegisterCornerRadius;
            pnlResults.BackColor = CardSurface;
            pnlResults.Padding = new Padding(8, 10, 8, 8);

            SetupSearchField();
            LayoutSearchForm();
            SetupGrid();
            ReloadCustomersFromExcel();

            AcceptButton = btnSearch;
            cmbSearchType.SelectedIndexChanged += SearchInput_Changed;
            txtSearch.TextChanged += SearchInput_Changed;
            txtSearch.Focus();
        }

        private void ApplySearchTypography()
        {
            lblTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.ForeColor = TitleColor;
            lblTitle.BackColor = Color.Transparent;

            lblSubtitle.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point);
            lblSubtitle.ForeColor = SubtitleColor;
            lblSubtitle.BackColor = Color.Transparent;

            lblHint.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            lblHint.ForeColor = HintColor;
            lblHint.BackColor = Color.Transparent;

            lblSearchError.Font = ClinicUiTheme.ErrorFont;
            lblSearchError.ForeColor = ClinicUiTheme.ErrorText;
            lblSearchError.BackColor = Color.Transparent;

            lblNoResults.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblNoResults.ForeColor = SubtitleColor;
            lblNoResults.BackColor = Color.Transparent;

            lblBarIcon.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point);
            lblBarIcon.ForeColor = LabelAccent;
            lblBarIcon.BackColor = Color.White;

            cmbSearchType.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            cmbSearchType.ForeColor = Color.FromArgb(33, 52, 72);

            Font buttonFont = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            btnSearch.Font = buttonFont;
            btnClear.Font = buttonFont;
            btnBack.Font = buttonFont;
        }

        private void ApplyLightChrome()
        {
            txtSearch.BorderStyle = BorderStyle.None;
            txtSearch.BackColor = Color.White;
            txtSearch.ForeColor = Color.FromArgb(33, 52, 72);
            txtSearch.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point);

            cmbSearchType.FlatStyle = FlatStyle.Flat;
            cmbSearchType.BackColor = Color.FromArgb(248, 252, 255);
        }

        private void SetupSearchField()
        {
            txtSearch.ReadOnly = false;
            txtSearch.Enabled = true;
            txtSearch.TabStop = true;
        }

        private void LayoutSearchForm()
        {
            pnlContent.SetBounds(480, 48, 720, 620);

            int centerX = ContentLeft + (ContentWidth / 2);
            pnlHeaderIcon.SetBounds(centerX - 28, 12, 56, 56);

            lblTitle.SetBounds(ContentLeft, 78, ContentWidth, 38);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            lblSubtitle.SetBounds(ContentLeft, 118, ContentWidth, 44);
            lblSubtitle.TextAlign = ContentAlignment.TopCenter;

            cmbSearchType.SetBounds(ContentLeft, 176, 200, 30);
            btnClear.SetBounds(ContentLeft + ContentWidth - 88, 176, 88, 30);

            pnlSearchBar.SetBounds(ContentLeft, 218, ContentWidth, 52);
            FitSearchBarContents();

            lblHint.SetBounds(ContentLeft, 278, ContentWidth, 20);
            lblHint.TextAlign = ContentAlignment.MiddleCenter;

            lblSearchError.SetBounds(ContentLeft, 300, ContentWidth, 18);
            lblNoResults.SetBounds(ContentLeft, 320, ContentWidth, 18);

            pnlResults.SetBounds(ContentLeft, 344, ContentWidth, 220);
            dgvResults.SetBounds(8, 10, ContentWidth - 16, 200);

            btnBack.SetBounds(36, 668, 130, 40);

            lblSearchType.Visible = false;
            lblSearch.Visible = false;
        }

        private void FitSearchBarContents()
        {
            const int barPad = 6;
            const int iconWidth = 40;
            const int buttonWidth = 104;
            int innerH = pnlSearchBar.Height - (barPad * 2);

            lblBarIcon.SetBounds(barPad + 4, barPad, iconWidth, innerH);
            btnSearch.SetBounds(pnlSearchBar.Width - buttonWidth - barPad, barPad, buttonWidth, innerH);

            int textLeft = lblBarIcon.Right + 4;
            int textWidth = btnSearch.Left - textLeft - 4;
            txtSearch.SetBounds(textLeft, barPad + 1, Math.Max(80, textWidth), innerH - 2);
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
            ApplyLightResultsGrid();
        }

        private void ApplyLightResultsGrid()
        {
            dgvResults.BorderStyle = BorderStyle.None;
            dgvResults.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvResults.GridColor = Color.FromArgb(210, 225, 240);
            dgvResults.BackgroundColor = Color.FromArgb(252, 253, 255);
            dgvResults.EnableHeadersVisualStyles = false;
            dgvResults.Font = new Font("Segoe UI", 9F);

            dgvResults.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(227, 242, 253);
            dgvResults.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(21, 101, 192);
            dgvResults.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            dgvResults.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(227, 242, 253);
            dgvResults.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.FromArgb(21, 101, 192);
            dgvResults.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvResults.ColumnHeadersHeight = 32;

            dgvResults.DefaultCellStyle.BackColor = Color.White;
            dgvResults.DefaultCellStyle.ForeColor = Color.FromArgb(33, 52, 72);
            dgvResults.DefaultCellStyle.SelectionBackColor = Color.FromArgb(187, 222, 251);
            dgvResults.DefaultCellStyle.SelectionForeColor = Color.FromArgb(13, 71, 161);

            dgvResults.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 250, 255);
            dgvResults.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(33, 52, 72);
            dgvResults.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(187, 222, 251);
            dgvResults.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.FromArgb(13, 71, 161);

            dgvResults.RowTemplate.Height = 28;
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
            pnlSearchBar.IsInvalid = false;
        }

        private void ShowSearchError(string message)
        {
            lblSearchError.Text = message;
            lblSearchError.Visible = true;
            pnlSearchBar.IsInvalid = true;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ClearSearchCustomerBackground();
            base.OnFormClosed(e);
        }

        private void ApplySearchCustomerBackground()
        {
            ClearSearchCustomerBackground();
            BackgroundImageLayout = ImageLayout.Stretch;

            string path = FindSearchCustomerBackgroundPath();
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
                ClearSearchCustomerBackground();
                BackColor = FormFallbackBack;
            }
        }

        private void ClearSearchCustomerBackground()
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

        private static string FindSearchCustomerBackgroundPath()
        {
            string relative = Path.Combine("images", "search_customer_bg.png");
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

        private sealed class SearchHeaderIconPanel : Panel
        {
            public SearchHeaderIconPanel()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
                BackColor = Color.Transparent;
                Size = new Size(56, 56);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var circle = new Rectangle(2, 2, Width - 5, Height - 5);
                using (var fill = new SolidBrush(Color.FromArgb(235, 245, 255)))
                using (var border = new Pen(Color.FromArgb(144, 202, 249), 2f))
                {
                    g.FillEllipse(fill, circle);
                    g.DrawEllipse(border, circle);
                }

                using (var glass = new Pen(Color.FromArgb(25, 118, 210), 2.2f))
                {
                    int cx = Width / 2;
                    int cy = Height / 2;
                    g.DrawEllipse(glass, cx - 10, cy - 11, 18, 18);
                    g.DrawLine(glass, cx + 7, cy + 5, cx + 14, cy + 12);
                }
            }
        }

        private sealed class SearchBarPanel : Panel
        {
            private bool _isInvalid;

            public SearchBarPanel()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
                BackColor = Color.Transparent;
            }

            public bool IsInvalid
            {
                get => _isInvalid;
                set
                {
                    if (_isInvalid != value)
                    {
                        _isInvalid = value;
                        Invalidate();
                    }
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                RectangleF fillRect = new RectangleF(1f, 1f, Width - 3f, Height - 3f);
                int radius = 24;

                RectangleF shadowRect = fillRect;
                shadowRect.Offset(0f, 2f);
                using (GraphicsPath shadowPath = UiPaths.RoundedRectangle(shadowRect, radius))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(36, 120, 150, 175)))
                {
                    g.FillPath(shadowBrush, shadowPath);
                }

                using (GraphicsPath path = UiPaths.RoundedRectangle(fillRect, radius))
                using (var fillBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(fillBrush, path);
                }

                Color borderColor = _isInvalid
                    ? Color.FromArgb(220, 120, 140)
                    : Color.FromArgb(200, 220, 235);
                using (GraphicsPath borderPath = UiPaths.RoundedRectangle(fillRect, radius))
                using (var borderPen = new Pen(borderColor, 1.2f))
                {
                    g.DrawPath(borderPen, borderPath);
                }
            }
        }
    }
}
