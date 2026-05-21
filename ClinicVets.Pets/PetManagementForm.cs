using System;
using System.Drawing;
using System.Windows.Forms;
using ClinicVets;

namespace ClinicVets.UI
{
    public partial class PetManagementForm : Form
    {
        private const int CardWidth = 220;
        private const int CardHeight = 168;
        private const int CardGap = 20;
        private const int LogoAreaBottom = 162;
        private const int TitleHeight = 48;
        private const int TitleSubtitleGap = 14;
        private const int SubtitleHeight = 40;
        private const int SubtitleCardsGap = 44;

        private readonly string _ownerId;
        private readonly bool _vetWorkflow;

        private string _titleText = "Pet Management";
        private string _subtitleText = "Add, search, and manage pets and animal types.";
        private Rectangle _titleBounds;
        private Rectangle _subtitleBounds;
        private Font _titleFont;
        private Font _subtitleFont;
        private bool _uiInitialized;

        public PetManagementForm()
            : this(null, false)
        {
        }

        public PetManagementForm(string ownerId, bool vetWorkflow)
        {
            _ownerId = string.IsNullOrWhiteSpace(ownerId) ? null : ownerId.Trim();
            _vetWorkflow = vetWorkflow;

            try
            {
                InitializeComponent();
                StartPosition = FormStartPosition.CenterScreen;
                Shown += PetManagementForm_Shown;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Pet Management screen could not be created: " + (ex.InnerException?.Message ?? ex.Message),
                    ex);
            }
        }

        private void PetManagementForm_Shown(object sender, EventArgs e)
        {
            StartPosition = FormStartPosition.CenterScreen;
            CenterToScreen();
        }

        private void PetManagementForm_Load(object sender, EventArgs e)
        {
            if (_uiInitialized)
            {
                return;
            }

            try
            {
                SafeInitializeUi();
                _uiInitialized = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Pet Management could not be displayed." + Environment.NewLine + Environment.NewLine
                    + (ex.InnerException?.Message ?? ex.Message),
                    "ClinicVets",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                Close();
            }
        }

        private void SafeInitializeUi()
        {
            _titleFont = new Font("Segoe UI", 22F, FontStyle.Bold);
            _subtitleFont = new Font("Segoe UI", 11.25F, FontStyle.Regular);

            pictureBox1.Paint += PictureBox1_Paint;
            FormClosed += PetManagementForm_FormClosed;

            ClinicFormLayout.ApplyStandard(this);
            StartPosition = FormStartPosition.CenterScreen;
            Text = _vetWorkflow ? "Pet Management — Veterinarian" : "Pet Management";
            _titleText = "Pet Management";
            _subtitleText = "Add, search, and manage pets and animal types.";
            BackColor = PetDashboardTheme.PageBack;

            PetBackgroundHelper.ApplyToPictureBox(pictureBox1);
            pictureBox1.SendToBack();
            LayoutContent();
        }

        private void PetManagementForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            pictureBox1.Paint -= PictureBox1_Paint;
            FormClosed -= PetManagementForm_FormClosed;
            _titleFont?.Dispose();
            _subtitleFont?.Dispose();
            PetBackgroundHelper.DisposePictureBoxImage(pictureBox1);
        }

        private void PetManagementForm_Resize(object sender, EventArgs e)
        {
            if (!_uiInitialized)
            {
                return;
            }

            LayoutContent();
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (_titleFont == null || _subtitleFont == null)
            {
                return;
            }

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            TextRenderer.DrawText(
                e.Graphics,
                _titleText,
                _titleFont,
                _titleBounds,
                PetDashboardTheme.TitleBlue,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            TextRenderer.DrawText(
                e.Graphics,
                _subtitleText,
                _subtitleFont,
                _subtitleBounds,
                PetDashboardTheme.SubtitleGray,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.WordBreak);
        }

        private void LayoutContent()
        {
            if (btnAddPet == null)
            {
                return;
            }

            int centerX = ClientSize.Width / 2;

            int titleY = LogoAreaBottom + TitleSubtitleGap;
            int subtitleY = titleY + TitleHeight + 8;
            int cardsTop = subtitleY + SubtitleHeight + SubtitleCardsGap;

            _titleBounds = new Rectangle(centerX - 320, titleY, 640, TitleHeight);
            _subtitleBounds = new Rectangle(centerX - 360, subtitleY, 720, SubtitleHeight);

            int availableWidth = ClientSize.Width - 48;
            int cardWidth = Math.Min(CardWidth, (availableWidth - (CardGap * 3)) / 4);
            cardWidth = Math.Max(180, cardWidth);

            int rowWidth = (cardWidth * 4) + (CardGap * 3);
            int cardsLeft = centerX - (rowWidth / 2);

            int x = cardsLeft;
            btnAddPet.SetBounds(x, cardsTop, cardWidth, CardHeight);
            x += cardWidth + CardGap;
            btnSearchPet.SetBounds(x, cardsTop, cardWidth, CardHeight);
            x += cardWidth + CardGap;
            btnViewAllPets.SetBounds(x, cardsTop, cardWidth, CardHeight);
            x += cardWidth + CardGap;
            btnAnimalTypes.SetBounds(x, cardsTop, cardWidth, CardHeight);
            btnBack.Location = new Point(28, 24);

            btnBack.BringToFront();
            btnAddPet.BringToFront();
            btnSearchPet.BringToFront();
            btnViewAllPets.BringToFront();
            btnAnimalTypes.BringToFront();
            pictureBox1.Invalidate();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAddPet_Click(object sender, EventArgs e)
        {
            PetFormHost.ShowAddPet(this, _ownerId);
        }

        private void btnSearchPet_Click(object sender, EventArgs e)
        {
            PetFormHost.ShowSearchPet(this, _vetWorkflow);
        }

        private void btnViewAllPets_Click(object sender, EventArgs e)
        {
            PetFormHost.ShowAllPets(this);
        }

        private void btnAnimalTypes_Click(object sender, EventArgs e)
        {
            PetFormHost.ShowAnimalTypes(this);
        }
    }
}
