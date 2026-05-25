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
    public partial class CustomerManagementForm : Form
    {
        private static readonly Color TitleColor = Color.FromArgb(21, 101, 192);
        private static readonly Color SubtitleColor = Color.FromArgb(71, 95, 120);
        private static readonly Color LabelColor = Color.FromArgb(25, 118, 210);
        private static readonly Color SuccessColor = Color.FromArgb(46, 125, 50);
        private static readonly Color CardSurface = CardPanel.RegisterCardFill;
        private static readonly Color FormFallbackBack = Color.FromArgb(232, 244, 252);

        private const int InnerPad = 28;
        private const int CaptionHeight = 22;
        private const int FieldHeight = 32;
        private const int ErrorHeight = 18;
        private const int RowStride = 84;
        private const int MaxCardWidth = 520;
        private const int MaxCardHeight = 580;
        private const int CardTop = 100;
        private const int CardShiftRight = 118;

        private int _fieldWidth = 464;

        private readonly ExcelHelper _excelHelper = new ExcelHelper();
        private readonly List<Customer> _allCustomers = new List<Customer>();
        private ValidationFieldBinder _validation;
        private Image _ownedBackgroundImage;

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
            if (!EnsureSecretaryAccess())
            {
                return;
            }

            ClinicFormLayout.ApplyStandard(this);
            WinFormsUi.SetDoubleBuffered(this);
            ApplyAddCustomerBackground();

            pnlCard.UseRegisterLightStyle = true;
            pnlCard.ShowCornerDecorations = false;
            pnlCard.CornerRadius = CardPanel.RegisterCornerRadius;
            PositionCustomerCard();
            pnlCard.Padding = new Padding(0);
            pnlCard.BackColor = CardSurface;
            pnlCard.AutoScroll = false;

            ApplyCustomerTypography();
            ApplyHeaderChrome();

            btnAddCustomer.UseLoginLightStyle = true;
            btnAddCustomer.IsOutlineStyle = false;
            btnAddCustomer.CornerRadius = 8;

            btnBack.UseLoginLightStyle = true;
            btnBack.IsOutlineStyle = true;
            btnBack.CornerRadius = 8;

            ChromeTextPlate.WrapDirectTextBoxes(pnlCard);
            SetupValidation();
            ApplyFieldChrome();
            ApplyLabelSurfaces();
            LayoutCustomerControls();
            ReloadCustomersFromExcel();
            txtFullName.Focus();
            Resize += CustomerManagementForm_Resize;
        }

        private void CustomerManagementForm_Resize(object sender, EventArgs e)
        {
            if (ClientSize.Width < 200 || pnlCard == null)
            {
                return;
            }

            PositionCustomerCard();
            LayoutCustomerControls();
        }

        private void PositionCustomerCard()
        {
            const int sideMargin = 44;
            int cardWidth = Math.Min(MaxCardWidth, ClientSize.Width - sideMargin - CardShiftRight - 24);
            cardWidth = Math.Max(400, cardWidth);
            int cardHeight = Math.Min(MaxCardHeight, ClientSize.Height - CardTop - 32);
            cardHeight = Math.Max(540, cardHeight);

            int centeredLeft = (ClientSize.Width - cardWidth) / 2;
            int cardLeft = Math.Min(centeredLeft + CardShiftRight, ClientSize.Width - cardWidth - sideMargin);
            cardLeft = Math.Max(sideMargin, cardLeft);

            pnlCard.Location = new Point(cardLeft, CardTop);
            pnlCard.Size = new Size(cardWidth, cardHeight);
            _fieldWidth = Math.Max(320, cardWidth - (InnerPad * 2));
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
                "Customer management is available to Secretary users only.",
                "Access denied",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            Close();
            return false;
        }

        private void ApplyCustomerTypography()
        {
            lblTitle.Font = new Font("Segoe UI", 17F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.ForeColor = TitleColor;

            lblSubtitle.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            lblSubtitle.ForeColor = SubtitleColor;

            foreach (Label label in new[] { lblFullName, lblCustomerId, lblPhone, lblEmail })
            {
                label.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
                label.ForeColor = LabelColor;
            }

            lblSuccess.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            lblSuccess.ForeColor = SuccessColor;

            Font buttonFont = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point);
            btnAddCustomer.Font = buttonFont;
            btnBack.Font = buttonFont;
        }

        private void ApplyHeaderChrome()
        {
            lblTitle.BackColor = CardSurface;
            lblSubtitle.BackColor = CardSurface;
            lblSuccess.BackColor = CardSurface;
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

        private void LayoutCustomerControls()
        {
            const int titleLeft = 84;
            int headerTextWidth = Math.Max(240, _fieldWidth - (titleLeft - InnerPad));

            pnlHeaderIcon.SetBounds(InnerPad, 22, 44, 44);
            lblTitle.SetBounds(titleLeft, 22, headerTextWidth, 32);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;

            lblSubtitle.SetBounds(titleLeft, 56, headerTextWidth, 22);
            lblSubtitle.TextAlign = ContentAlignment.TopLeft;

            pnlDivider.SetBounds(InnerPad, 86, _fieldWidth, 1);

            int y = 100;
            LayoutFieldRow(lblFullName, txtFullName, y);
            PositionFieldError(txtFullName, y, FieldHeight);

            y += RowStride;
            LayoutFieldRow(lblCustomerId, txtCustomerId, y);
            PositionFieldError(txtCustomerId, y, FieldHeight);

            y += RowStride;
            LayoutFieldRow(lblPhone, txtPhone, y);
            PositionFieldError(txtPhone, y, FieldHeight);

            y += RowStride;
            LayoutFieldRow(lblEmail, txtEmail, y);
            PositionFieldError(txtEmail, y, FieldHeight);

            int actionsTop = y + RowStride + 10;
            lblSuccess.SetBounds(InnerPad, actionsTop, _fieldWidth, 20);
            btnAddCustomer.SetBounds(InnerPad, actionsTop + 28, _fieldWidth, 42);
            btnBack.SetBounds(InnerPad, actionsTop + 78, 132, 38);

            btnClear.Visible = false;
            btnOpenSearch.Visible = false;
        }

        private void LayoutFieldRow(Label caption, TextBox input, int captionY)
        {
            caption.AutoSize = false;
            caption.SetBounds(InnerPad, captionY, _fieldWidth, CaptionHeight);
            Control host = GetFieldHost(input);
            host.SetBounds(InnerPad, captionY + CaptionHeight + 2, _fieldWidth, FieldHeight);
            AlignChromePlateInner(host);
        }

        private void PositionFieldError(TextBox input, int captionY, int fieldHeight)
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

            int y = captionY + CaptionHeight + 2 + fieldHeight + 2;
            entry.ErrorLabel.SetBounds(InnerPad, y, _fieldWidth, ErrorHeight);
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

            _validation.BindTextBox(txtFullName, ValidationHelper.ValidateName, lblFullName);
            _validation.BindTextBox(txtCustomerId, ValidationHelper.ValidateIdNumber, lblCustomerId);
            _validation.BindTextBox(txtPhone, ValidationHelper.ValidatePhone, lblPhone);
            _validation.BindTextBox(txtEmail, ValidationHelper.ValidateEmail, lblEmail);
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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ClearAddCustomerBackground();
            base.OnFormClosed(e);
        }

        private void ApplyAddCustomerBackground()
        {
            ClearAddCustomerBackground();
            BackgroundImageLayout = ImageLayout.Stretch;

            Image cached = VetBackgroundHelper.GetCachedImage("add_customer_bg.png");
            if (cached == null)
            {
                BackColor = FormFallbackBack;
                Invalidate(true);
                return;
            }

            try
            {
                BackgroundImage = (Image)cached.Clone();
                BackColor = FormFallbackBack;
                Invalidate(true);
            }
            catch
            {
                ClearAddCustomerBackground();
                BackColor = FormFallbackBack;
            }
        }

        private void ClearAddCustomerBackground()
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

        private static string FindAddCustomerBackgroundPath()
        {
            string relative = Path.Combine("images", "add_customer_bg.png");
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

        private bool TryGetValidCustomer(out Customer customer)
        {
            customer = null;
            if (!_validation.ValidateAll())
            {
                return false;
            }

            string fullName = txtFullName.Text.Trim();
            string idNumber = txtCustomerId.Text.Trim();
            SplitFullName(fullName, out string firstName, out string lastName);

            customer = new Customer
            {
                CustomerID = idNumber,
                IDNumber = idNumber,
                FullName = fullName,
                FirstName = firstName,
                LastName = lastName,
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Address = string.Empty
            };
            return true;
        }

        private static void SplitFullName(string fullName, out string firstName, out string lastName)
        {
            firstName = fullName ?? string.Empty;
            lastName = string.Empty;
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return;
            }

            int space = fullName.IndexOf(' ');
            if (space <= 0)
            {
                firstName = fullName.Trim();
                return;
            }

            firstName = fullName.Substring(0, space).Trim();
            lastName = fullName.Substring(space + 1).Trim();
        }

        private static bool CustomerIdExists(IEnumerable<Customer> customers, string idNumber)
        {
            return customers.Any(c =>
                string.Equals(c.CustomerID, idNumber, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(c.IDNumber, idNumber, StringComparison.OrdinalIgnoreCase));
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            HideSuccessMessage();

            if (!TryGetValidCustomer(out Customer customer))
            {
                return;
            }

            if (CustomerIdExists(_allCustomers, customer.CustomerID))
            {
                var idEntry = _validation.Entries.First(entry => entry.InputControl == txtCustomerId);
                idEntry.ErrorLabel.Text = "A customer with this ID already exists.";
                idEntry.ErrorLabel.Visible = true;
                if (idEntry.HostControl is ChromeTextPlate plate)
                {
                    plate.IsInvalid = true;
                }

                txtCustomerId.Focus();
                return;
            }

            DialogResult confirm = MessageBox.Show(
                this,
                "Are you sure you want to add this customer?",
                "Confirm add customer",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _excelHelper.AppendCustomer(customer);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    "Customer — save",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                string message = ExcelHelper.IsWorkbookLockedException(ex)
                    ? ExcelFileManager.WorkbookLockedMessage
                    : "Could not save the customer to Excel."
                      + Environment.NewLine + Environment.NewLine + ex.Message;

                MessageBox.Show(
                    this,
                    message,
                    "Customer — save",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            ReloadCustomersFromExcel();
            ShowSuccessMessage("Customer added successfully.");
            ClearFields();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            HideSuccessMessage();
            ClearFields();
        }

        private void btnOpenSearch_Click(object sender, EventArgs e)
        {
            using (var searchForm = new CustomerSearchForm(this))
            {
                searchForm.ShowDialog(this);
            }
        }

        private void ShowSuccessMessage(string message)
        {
            lblSuccess.Text = message;
            lblSuccess.Visible = true;
        }

        private void HideSuccessMessage()
        {
            lblSuccess.Text = string.Empty;
            lblSuccess.Visible = false;
        }

        private void ClearFields()
        {
            _validation.ClearAll();
            txtFullName.Clear();
            txtCustomerId.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
        }

        private sealed class CustomerHeaderIconPanel : Panel
        {
            public CustomerHeaderIconPanel()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
                BackColor = CardPanel.RegisterCardFill;
                Size = new Size(44, 44);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var circle = new Rectangle(1, 1, Width - 3, Height - 3);
                using (var fill = new SolidBrush(Color.FromArgb(227, 242, 253)))
                using (var border = new Pen(Color.FromArgb(144, 202, 249), 1.5f))
                {
                    g.FillEllipse(fill, circle);
                    g.DrawEllipse(border, circle);
                }

                using (var iconPen = new Pen(Color.FromArgb(25, 118, 210), 2f))
                {
                    int cx = Width / 2;
                    int cy = Height / 2 - 2;
                    g.DrawEllipse(iconPen, cx - 7, cy - 9, 14, 14);
                    g.DrawArc(iconPen, cx - 10, cy + 3, 20, 14, 0, 180);
                }
            }
        }
    }
}
