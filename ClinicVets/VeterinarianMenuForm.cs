using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ClinicVets
{
    public partial class VeterinarianMenuForm : Form
    {
        private static readonly Color CardSurface = Color.FromArgb(248, 252, 255);
        private static readonly Color FormFallbackBack = Color.FromArgb(232, 244, 252);

        private const int PanelWidth = 520;
        private const int PanelHeight = 450;
        private const int PanelOffsetRight = 44;
        private const int PanelTop = 192;
        private const int ButtonHeight = 52;
        private const int ButtonGap = 14;
        private const int LogoutExtraGap = 22;

        private readonly Form1 _loginForm;
        private Image _ownedBackgroundImage;

        public VeterinarianMenuForm(Form1 loginForm)
        {
            _loginForm = loginForm;
            InitializeComponent();
            if (loginForm != null)
            {
                Owner = loginForm;
            }
        }

        private void VeterinarianMenuForm_Load(object sender, EventArgs e)
        {
            if (!RolePermissions.IsVeterinarian())
            {
                MessageBox.Show(
                    this,
                    "This menu is for Veterinarian users only.",
                    "Access denied",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                Close();
                return;
            }

            WinFormsUi.SetDoubleBuffered(this);
            ClinicFormLayout.ApplyStandard(this);
            ApplyMenuBackground();

            pnlCard.BackColor = CardSurface;
            lblTitle.BackColor = CardSurface;
            lblSubtitle.BackColor = CardSurface;

            LayoutMenuPanel();
        }

        private void VeterinarianMenuForm_Resize(object sender, EventArgs e)
        {
            LayoutMenuPanel();
        }

        private void LayoutMenuPanel()
        {
            int centerX = ClientSize.Width / 2;
            int panelLeft = centerX - (PanelWidth / 2) + PanelOffsetRight;
            pnlCard.SetBounds(panelLeft, PanelTop, PanelWidth, PanelHeight);

            const int innerPad = 40;
            int btnWidth = PanelWidth - (innerPad * 2);
            int y = 128;

            btnManagePets.SetBounds(innerPad, y, btnWidth, ButtonHeight);
            y += ButtonHeight + ButtonGap;
            btnVisits.SetBounds(innerPad, y, btnWidth, ButtonHeight);
            y += ButtonHeight + ButtonGap;
            btnMedicines.SetBounds(innerPad, y, btnWidth, ButtonHeight);
            y += ButtonHeight + ButtonGap + LogoutExtraGap;
            btnLogout.SetBounds(innerPad, y, btnWidth, ButtonHeight);

            lblTitle.SetBounds(innerPad, 32, btnWidth, 42);
            lblSubtitle.SetBounds(innerPad, 78, btnWidth, 32);
        }

        private void ApplyMenuBackground()
        {
            ClearMenuBackground();
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
                ClearMenuBackground();
                BackColor = FormFallbackBack;
            }
        }

        private void ClearMenuBackground()
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

        private static string FindMenuBackgroundPath()
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

            yield return Environment.CurrentDirectory;
        }

        private void btnManagePets_Click(object sender, EventArgs e)
        {
            Form host = _loginForm ?? (Form)Owner;
            Hide();
            try
            {
                NavigationHelper.OpenVetPetManagement(host);
            }
            finally
            {
                Show();
            }
        }

        private void btnVisits_Click(object sender, EventArgs e)
        {
            VetVisitsLauncher.TryLaunchVisitsApplication(this, null);
        }

        private void btnMedicines_Click(object sender, EventArgs e)
        {
            VetVisitsLauncher.TryLaunchMedicinesApplication(this);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            SessionManager.Clear();
            _loginForm?.ClearLoginFields();
            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ClearMenuBackground();
            base.OnFormClosed(e);
        }
    }
}
