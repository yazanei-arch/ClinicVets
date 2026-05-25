using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ClinicVets
{
    public partial class ViewCustomersForm : Form
    {
        private static readonly Color TitleColor = Color.FromArgb(21, 101, 192);
        private static readonly Color SubtitleColor = Color.FromArgb(71, 95, 120);
        private static readonly Color PanelSurface = Color.FromArgb(240, 255, 255, 255);
        private static readonly Color FormFallbackBack = Color.FromArgb(232, 244, 252);

        private const int InnerPad = 35;
        private const int ContentWidth = 500;
        private const int CardShiftRight = 118;

        private readonly ExcelHelper _excelHelper = new ExcelHelper();
        private Image _ownedBackgroundImage;

        public ViewCustomersForm()
            : this(null)
        {
        }

        public ViewCustomersForm(Form owner)
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.Sizable;
            ControlBox = true;
            MinimizeBox = true;
            MaximizeBox = true;
            Text = "ClinicVets - View Customers";
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;

            BackgroundImageLayout = ImageLayout.Stretch;
            TryLoadBackgroundImage();

            if (owner != null)
            {
                Owner = owner;
            }
        }

        private void TryLoadBackgroundImage()
        {
            try
            {
                Image cached = VetBackgroundHelper.GetCachedImage("view_customers_bg.png");
                if (cached != null)
                {
                    BackgroundImage = (Image)cached.Clone();
                }
            }
            catch
            {
                ClearViewCustomersBackground();
                BackColor = FormFallbackBack;
            }
        }

        private bool EnsureSecretaryAccess()
        {
            Employee user = SessionManager.CurrentUser;
            if (user != null &&
                user.Role != null &&
                user.Role.Equals("Secretary", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            MessageBox.Show(
                this,
                "View customers is available to Secretary users only.",
                "Access denied",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            Close();
            return false;
        }

        private void ViewCustomersForm_Load(object sender, EventArgs e)
        {
            if (!EnsureSecretaryAccess())
            {
                return;
            }

            ClinicFormLayout.ApplyStandard(this);
            pnlCard.UseRegisterLightStyle = true;
            pnlCard.ShowCornerDecorations = false;
            pnlCard.CornerRadius = CardPanel.RegisterCornerRadius;
            BackgroundImageLayout = ImageLayout.Stretch;
            PositionViewCustomersCard();
            pnlCard.Padding = new Padding(0);
            pnlCard.BackColor = PanelSurface;

            ApplyTypography();
            ApplyHeaderChrome();

            btnRefresh.UseLoginLightStyle = true;
            btnRefresh.IsOutlineStyle = false;
            btnRefresh.CornerRadius = 8;

            btnBack.UseLoginLightStyle = true;
            btnBack.IsOutlineStyle = true;
            btnBack.CornerRadius = 8;

            SetupGrid();
            LayoutViewCustomersControls();
            ReloadCustomersFromExcel();
            ExcelHelper.CustomersChanged += OnCustomersDataChanged;
            Resize += ViewCustomersForm_Resize;
        }

        private void ViewCustomersForm_Resize(object sender, EventArgs e)
        {
            if (ClientSize.Width < 200 || pnlCard == null)
            {
                return;
            }

            PositionViewCustomersCard();
        }

        private void PositionViewCustomersCard()
        {
            const int top = 115;
            const int sideMargin = 40;
            int cardWidth = Math.Min(570, ClientSize.Width - (sideMargin * 2));
            int cardHeight = Math.Min(500, ClientSize.Height - top - 40);
            int centeredLeft = (ClientSize.Width - cardWidth) / 2;
            int cardLeft = Math.Min(centeredLeft + CardShiftRight, ClientSize.Width - cardWidth - sideMargin);
            cardLeft = Math.Max(sideMargin, cardLeft);
            pnlCard.Location = new Point(cardLeft, top);
            pnlCard.Size = new Size(cardWidth, cardHeight);
        }

        private void ApplyTypography()
        {
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.ForeColor = TitleColor;
            lblTitle.BackColor = PanelSurface;

            lblSubtitle.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            lblSubtitle.ForeColor = SubtitleColor;
            lblSubtitle.BackColor = PanelSurface;

            Font buttonFont = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point);
            btnRefresh.Font = buttonFont;
            btnBack.Font = buttonFont;
        }

        private void ApplyHeaderChrome()
        {
            pnlDivider.BackColor = Color.FromArgb(210, 225, 240);
        }

        private void LayoutViewCustomersControls()
        {
            lblTitle.SetBounds(InnerPad, 32, ContentWidth, 32);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;

            lblSubtitle.SetBounds(InnerPad, 68, ContentWidth, 22);
            lblSubtitle.TextAlign = ContentAlignment.TopLeft;

            pnlDivider.SetBounds(InnerPad, 152, ContentWidth, 1);

            dgvCustomers.Location = new Point(35, 165);
            dgvCustomers.Size = new Size(500, 210);

            btnBack.Location = new Point(35, 415);
            btnRefresh.Location = new Point(385, 415);
            btnRefresh.Size = new Size(150, 45);
        }

        private void SetupGrid()
        {
            dgvCustomers.AutoGenerateColumns = false;
            dgvCustomers.Columns.Clear();
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CustomerName",
                HeaderText = "Customer Name",
                FillWeight = 40,
                ReadOnly = true
            });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Pets",
                HeaderText = "Pets",
                FillWeight = 60,
                ReadOnly = true
            });
            dgvCustomers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.ReadOnly = true;
            dgvCustomers.RowHeadersVisible = false;
            dgvCustomers.AllowUserToAddRows = false;
            dgvCustomers.AllowUserToDeleteRows = false;
            ApplyLightCustomerGrid();
        }

        private void ApplyLightCustomerGrid()
        {
            dgvCustomers.BackgroundColor = Color.White;
            dgvCustomers.BorderStyle = BorderStyle.None;
            dgvCustomers.RowTemplate.Height = 38;
            dgvCustomers.EnableHeadersVisualStyles = false;

            dgvCustomers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 240, 250);
            dgvCustomers.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(30, 90, 200);
            dgvCustomers.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvCustomers.ColumnHeadersHeight = 38;

            dgvCustomers.DefaultCellStyle.BackColor = Color.White;
            dgvCustomers.DefaultCellStyle.ForeColor = Color.FromArgb(33, 52, 72);
            dgvCustomers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 255);
            dgvCustomers.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvCustomers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 251, 255);
            dgvCustomers.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(33, 52, 72);
            dgvCustomers.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 255);
            dgvCustomers.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.Black;
        }

        private void ReloadCustomersFromExcel()
        {
            dgvCustomers.Rows.Clear();
            try
            {
                IReadOnlyDictionary<string, List<string>> petsByOwner;
                try
                {
                    petsByOwner = _excelHelper.ReadPetOwnerIndex();
                }
                catch
                {
                    petsByOwner = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
                }

                foreach (Customer customer in _excelHelper.ReadCustomers())
                {
                    string customerName = (customer.DisplayName ?? string.Empty).Trim();
                    if (customerName.Length == 0)
                    {
                        continue;
                    }

                    string petsText = BuildPetsCellText(customerName, petsByOwner);
                    dgvCustomers.Rows.Add(customerName, petsText);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Could not load customers from Excel." + Environment.NewLine + ex.Message,
                    "View Customers — data",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private static string BuildPetsCellText(
            string customerName,
            IReadOnlyDictionary<string, List<string>> petsByOwner)
        {
            if (petsByOwner != null
                && petsByOwner.TryGetValue(customerName, out List<string> pets)
                && pets != null
                && pets.Count > 0)
            {
                return string.Join(", ", pets);
            }

            return "No pets";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ReloadCustomersFromExcel();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ExcelHelper.CustomersChanged -= OnCustomersDataChanged;
            ClearViewCustomersBackground();
            base.OnFormClosed(e);
        }

        private void OnCustomersDataChanged(object sender, EventArgs e)
        {
            if (IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action(ReloadCustomersFromExcel));
                return;
            }

            ReloadCustomersFromExcel();
        }

        private void ClearViewCustomersBackground()
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

        private static string FindViewCustomersBackgroundPath()
        {
            string relative = Path.Combine("images", "view_customers_bg.png");
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
    }
}
