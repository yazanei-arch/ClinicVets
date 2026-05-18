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
            if (loginForm != null)
            {
                Owner = loginForm;
            }
        }

        private void RegistrationSuccessForm_Load(object sender, EventArgs e)
        {
            WinFormsUi.SetDoubleBuffered(this);
            VetBackgroundHelper.ApplyRegisterBackground(this);

            pnlCard.UseRegisterLightStyle = true;
            pnlCard.ShowCornerDecorations = false;
            pnlCard.CornerRadius = CardPanel.RegisterCornerRadius;
            pnlCard.Location = new Point(630, 55);
            pnlCard.Size = new Size(560, 760);
            pnlCard.BackColor = CardSurface;

            lblTitle.BackColor = CardSurface;
            lblSubtitle.BackColor = CardSurface;
            chkSuccess.BackColor = CardSurface;

            btnBackToLogin.UseLoginLightStyle = true;
            btnBackToLogin.IsOutlineStyle = false;
            btnBackToLogin.CornerRadius = 8;

            btnRegisterAnother.UseLoginLightStyle = true;
            btnRegisterAnother.IsOutlineStyle = true;
            btnRegisterAnother.CornerRadius = 8;

            chkSuccess.PlayPopAnimation();
        }

        private void btnBackToLogin_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnRegisterAnother_Click(object sender, EventArgs e)
        {
            Form1 login = _loginForm ?? Owner as Form1;
            Hide();
            using (var registerForm = new RegisterEmployeeForm(login))
            {
                registerForm.ShowDialog(login);
            }

            Close();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            VetBackgroundHelper.ClearBackgroundImage(this);
            base.OnFormClosed(e);
        }
    }
}
