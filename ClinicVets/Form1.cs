using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClinicVets
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WinFormsUi.SetDoubleBuffered(this);
            VetBackgroundHelper.ApplyVetBackground(this);
            ChromeTextPlate.WrapDirectTextBoxes(pnlCard);
            CenterLoginCard();
            txtUsername.Focus();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Image img = BackgroundImage;
            BackgroundImage = null;
            img?.Dispose();
            base.OnFormClosed(e);
        }

        private void CenterLoginCard()
        {
            pnlCard.Left = (ClientSize.Width - pnlCard.Width) / 2;
            pnlCard.Top = (ClientSize.Height - pnlCard.Height) / 2;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Hide();
            try
            {
                using (var customerForm = new CustomerManagementForm(this))
                {
                    customerForm.ShowDialog(this);
                }
            }
            finally
            {
                if (!IsDisposed)
                {
                    Show();
                }
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            using (var registerForm = new RegisterEmployeeForm(this))
            {
                registerForm.ShowDialog(this);
            }
        }
    }
}
