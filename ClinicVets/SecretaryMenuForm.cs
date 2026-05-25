using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ClinicVets
{
    public partial class SecretaryMenuForm : Form
    {
        private static readonly Color CardSurface = Color.FromArgb(248, 252, 255);
        private static readonly Color FormFallbackBack = Color.FromArgb(232, 244, 252);

        private readonly Form1 _loginForm;
        private Image _ownedBackgroundImage;

        public SecretaryMenuForm(Form1 loginForm)
        {
            _loginForm = loginForm;
            InitializeComponent();
            if (loginForm != null)
            {
                Owner = loginForm;
            }
        }

        private void SecretaryMenuForm_Load(object sender, EventArgs e)
        {
            if (!RolePermissions.IsSecretary())
            {
                MessageBox.Show(
                    this,
                    "This menu is for Secretary users only.",
                    "Access denied",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                Close();
                return;
            }

            WinFormsUi.SetDoubleBuffered(this);
            ClinicFormLayout.ApplyStandard(this);
            ApplySecretaryMenuBackground();

            pnlCard.BackColor = CardSurface;
            lblTitle.BackColor = CardSurface;
            lblSubtitle.BackColor = CardSurface;

            btnAddCustomer.UseLoginLightStyle = true;
            btnAddCustomer.IsOutlineStyle = true;
            btnAddCustomer.CornerRadius = 8;

            btnSearchCustomer.UseLoginLightStyle = true;
            btnSearchCustomer.IsOutlineStyle = true;
            btnSearchCustomer.CornerRadius = 8;

            btnViewCustomers.UseLoginLightStyle = true;
            btnViewCustomers.IsOutlineStyle = true;
            btnViewCustomers.CornerRadius = 8;

            btnManagePets.UseLoginLightStyle = true;
            btnManagePets.IsOutlineStyle = true;
            btnManagePets.CornerRadius = 8;

            btnLogout.UseLoginLightStyle = true;
            btnLogout.IsOutlineStyle = true;
            btnLogout.CornerRadius = 8;

            LayoutSecretaryMenuPanel();
            Resize += SecretaryMenuForm_Resize;
        }

        private void SecretaryMenuForm_Resize(object sender, EventArgs e)
        {
            LayoutSecretaryMenuPanel();
        }

        private void LayoutSecretaryMenuPanel()
        {
            const int panelWidth = 560;
            const int panelHeight = 480;
            const int panelTop = 155;
            int panelLeft = Math.Max(24, (ClientSize.Width - panelWidth) / 2);
            pnlCard.SetBounds(panelLeft, panelTop, panelWidth, panelHeight);
        }

        private void btnManagePets_Click(object sender, EventArgs e)
        {
            Form host = _loginForm ?? (Form)Owner;
            Hide();
            try
            {
                NavigationHelper.OpenSecretaryPetManagement(host, null);
            }
            finally
            {
                Show();
            }
        }

        private void ApplySecretaryMenuBackground()
        {
            ClearSecretaryMenuBackground();
            BackgroundImageLayout = ImageLayout.Stretch;

            Image cached = VetBackgroundHelper.GetCachedImage("secretary_menu_bg.png");
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
                ClearSecretaryMenuBackground();
                BackColor = FormFallbackBack;
            }
        }

        private void ClearSecretaryMenuBackground()
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

        private static string FindSecretaryMenuBackgroundPath()
        {
            string relative = Path.Combine("images", "secretary_menu_bg.png");
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

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            OpenCustomerManagement();
        }

        private void btnSearchCustomer_Click(object sender, EventArgs e)
        {
            Form host = _loginForm ?? (Form)Owner;
            Hide();
            try
            {
                using (var searchForm = new CustomerSearchForm(host))
                {
                    searchForm.ShowDialog(host);
                }
            }
            finally
            {
                Show();
            }
        }

        private void btnViewCustomers_Click(object sender, EventArgs e)
        {
            OpenViewCustomers();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            SessionManager.Clear();
            _loginForm?.ClearLoginFields();
            Close();
        }

        private void OpenCustomerManagement()
        {
            Form host = _loginForm ?? (Form)Owner;
            Hide();
            try
            {
                using (var customerForm = new CustomerManagementForm(host))
                {
                    customerForm.ShowDialog(host);
                }
            }
            finally
            {
                Show();
            }
        }

        private void OpenViewCustomers()
        {
            Form host = _loginForm ?? (Form)Owner;
            Hide();
            try
            {
                using (var viewForm = new ViewCustomersForm(host))
                {
                    viewForm.ShowDialog(host);
                }
            }
            finally
            {
                Show();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ClearSecretaryMenuBackground();
            base.OnFormClosed(e);
        }
    }
}
