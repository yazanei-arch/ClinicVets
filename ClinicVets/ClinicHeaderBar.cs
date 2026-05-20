using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>Dark frosted top navigation bar with cyan accents.</summary>
    public class ClinicHeaderBar : Panel
    {
        public ClinicHeaderBar()
        {
            Dock = DockStyle.Top;
            Height = ClinicUiTheme.HeaderHeight;
            BackColor = Color.Transparent;
            Padding = new Padding(24, 0, 24, 0);
            SubtitleText = "Veterinary Management System";
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);
            WinFormsUi.SetDoubleBuffered(this);
        }

        public string SubtitleText { get; set; }

        public bool ShowUserChip { get; set; } = true;

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bounds = new Rectangle(0, 0, Width, Height);

            using (var brush = new LinearGradientBrush(
                bounds,
                ClinicUiTheme.HeaderGlassTop,
                ClinicUiTheme.HeaderGlassBottom,
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, bounds);
            }

            using (var glowPen = new Pen(Color.FromArgb(90, ClinicUiTheme.AccentCyan), 1f))
            {
                g.DrawLine(glowPen, 0, Height - 1, Width, Height - 1);
            }

            var logoRect = new Rectangle(24, (Height - 40) / 2, 40, 40);
            ClinicDecorations.DrawLogoMark(g, logoRect);

            int textLeft = logoRect.Right + 14;
            int userWidth = 0;
            string userText = GetUserDisplayText();
            if (ShowUserChip && userText.Length > 0)
            {
                Size userSize = TextRenderer.MeasureText(userText, ClinicUiTheme.HeaderUserFont);
                userWidth = userSize.Width + 28;
                var chipRect = new Rectangle(Width - 24 - userWidth, (Height - 30) / 2, userWidth, 30);
                using (GraphicsPath chipPath = UiPaths.RoundedRectangle(chipRect, 15))
                using (var chipBrush = new SolidBrush(ClinicUiTheme.HeaderUserChip))
                {
                    g.FillPath(chipBrush, chipPath);
                }

                TextRenderer.DrawText(
                    g,
                    userText,
                    ClinicUiTheme.HeaderUserFont,
                    chipRect,
                    ClinicUiTheme.HeaderUserText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }

            int textRight = Width - 24 - userWidth - (userWidth > 0 ? 12 : 0);
            var titleRect = new Rectangle(textLeft, 12, Math.Max(80, textRight - textLeft), 26);
            TextRenderer.DrawText(
                g,
                "ClinicVets",
                ClinicUiTheme.HeaderTitleFont,
                titleRect,
                ClinicUiTheme.HeaderTitle,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            var subRect = new Rectangle(textLeft, 36, titleRect.Width, 20);
            TextRenderer.DrawText(
                g,
                SubtitleText ?? string.Empty,
                ClinicUiTheme.HeaderSubtitleFont,
                subRect,
                ClinicUiTheme.HeaderSubtitle,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private static string GetUserDisplayText()
        {
            Employee user = SessionManager.CurrentUser;
            if (user == null)
            {
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(user.Username))
            {
                return user.Username.Trim();
            }

            return "Staff";
        }
    }
}
