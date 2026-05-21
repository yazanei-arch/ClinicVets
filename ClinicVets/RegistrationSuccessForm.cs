using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClinicVets
{
    public partial class RegistrationSuccessForm : Form
    {
        private static readonly Color CardSurface = CardPanel.RegisterCardFill;
        private readonly Form1 _loginForm;

        public RegistrationSuccessForm(Form1 loginForm)
        {
            _loginForm = loginForm;
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            Shown += RegistrationSuccessForm_Shown;
            if (loginForm != null)
            {
                Owner = loginForm;
            }
        }

        private void RegistrationSuccessForm_Shown(object sender, EventArgs e)
        {
            StartPosition = FormStartPosition.CenterScreen;
            CenterToScreen();
        }

        private void RegistrationSuccessForm_Load(object sender, EventArgs e)
        {
            ClinicFormLayout.ApplyStandard(this);
            StartPosition = FormStartPosition.CenterScreen;
            WinFormsUi.SetDoubleBuffered(this);
            VetBackgroundHelper.ApplyRegisterBackground(this);

            pnlCard.UseRegisterLightStyle = true;
            pnlCard.ShowCornerDecorations = false;
            pnlCard.CornerRadius = CardPanel.RegisterCornerRadius;
            PositionSuccessCard();
            pnlCard.BackColor = CardSurface;

            lblTitle.BackColor = CardSurface;
            lblSubtitle.BackColor = CardSurface;
            lblSuccessIcon.BackColor = CardSurface;

            btnBackToLogin.UseLoginLightStyle = true;
            btnBackToLogin.IsOutlineStyle = false;
            btnBackToLogin.CornerRadius = 8;

            btnRegisterAnother.UseLoginLightStyle = true;
            btnRegisterAnother.IsOutlineStyle = true;
            btnRegisterAnother.CornerRadius = 8;
            Resize += RegistrationSuccessForm_Resize;
        }

        private void RegistrationSuccessForm_Resize(object sender, EventArgs e)
        {
            if (ClientSize.Width < 200 || pnlCard == null)
            {
                return;
            }

            PositionSuccessCard();
        }

        private void PositionSuccessCard()
        {
            const int baseCardTop = 42;
            const int panelShiftRight = 80;
            const int panelShiftDown = 40;
            const int panelExtraHeight = 60;
            const int sideMargin = 40;
            const int bottomMargin = 40;
            const int leftBrandingReserve = 360;
            const int gapAfterLogo = 12;
            int cardTop = baseCardTop + panelShiftDown;
            int cardWidth = Math.Min(560, ClientSize.Width - (sideMargin * 2));
            int cardLeft = leftBrandingReserve + gapAfterLogo + panelShiftRight;
            cardLeft = Math.Min(cardLeft, ClientSize.Width - cardWidth - sideMargin);
            cardLeft = Math.Max(sideMargin, cardLeft);

            int contentHeight = LayoutSuccessCardContent(cardWidth, 0);
            int cardHeight = Math.Min(
                contentHeight + panelExtraHeight,
                ClientSize.Height - cardTop - bottomMargin);
            int verticalOffset = Math.Max(0, (cardHeight - contentHeight) / 2);
            LayoutSuccessCardContent(cardWidth, verticalOffset);

            pnlCard.Location = new Point(cardLeft, cardTop);
            pnlCard.Size = new Size(cardWidth, cardHeight);
        }

        private int LayoutSuccessCardContent(int cardWidth, int verticalOffset)
        {
            const int topPadding = 32;
            const int bottomPadding = 32;
            const int iconSize = 120;
            const int titleHeight = 48;
            const int subtitleHeight = 56;
            const int buttonHeight = 48;
            const int buttonGap = 12;
            const int gapIconTitle = 16;
            const int gapTitleSubtitle = 8;
            const int gapSubtitleButtons = 26;

            int contentWidth = Math.Min(470, cardWidth - 90);
            int contentLeft = (cardWidth - contentWidth) / 2;
            int iconLeft = (cardWidth - iconSize) / 2;
            int y = topPadding + verticalOffset;

            lblSuccessIcon.SetBounds(iconLeft, y, iconSize, iconSize);
            y += iconSize + gapIconTitle;

            lblTitle.SetBounds(contentLeft, y, contentWidth, titleHeight);
            y += titleHeight + gapTitleSubtitle;

            lblSubtitle.SetBounds(contentLeft, y, contentWidth, subtitleHeight);
            y += subtitleHeight + gapSubtitleButtons;

            btnBackToLogin.SetBounds(contentLeft, y, contentWidth, buttonHeight);
            y += buttonHeight + buttonGap;

            btnRegisterAnother.SetBounds(contentLeft, y, contentWidth, buttonHeight);

            return y + buttonHeight + bottomPadding;
        }

        private void btnBackToLogin_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnRegisterAnother_Click(object sender, EventArgs e)
        {
            Form1 login = _loginForm ?? Owner as Form1;
            using (var registerForm = new RegisterEmployeeForm(login))
            {
                registerForm.ShowDialog(login);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            VetBackgroundHelper.ClearBackgroundImage(this);
            base.OnFormClosed(e);
        }
    }
}
