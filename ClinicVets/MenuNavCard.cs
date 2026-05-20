using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>Glass navigation tile with smooth hover lift.</summary>
    public class MenuNavCard : Panel
    {
        private float _hoverAmount;
        private float _hoverTarget;
        private readonly Timer _hoverTimer;

        public MenuNavCard()
        {
            Cursor = Cursors.Hand;
            CornerRadius = ClinicUiTheme.NavCardRadius;
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor,
                true);
            WinFormsUi.SetDoubleBuffered(this);
            BackColor = Color.Transparent;
            Font = ClinicUiTheme.LabelFont;
            ForeColor = ClinicUiTheme.TitleBlue;
            Padding = new Padding(20, 18, 20, 18);
            ShowPawAccent = false;

            _hoverTimer = new Timer { Interval = 16 };
            _hoverTimer.Tick += HoverTimer_Tick;
        }

        public int CornerRadius { get; set; }

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public bool ShowPawAccent { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _hoverTimer.Stop();
                _hoverTimer.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hoverTarget = 1f;
            _hoverTimer.Start();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoverTarget = 0f;
            _hoverTimer.Start();
        }

        private void HoverTimer_Tick(object sender, EventArgs e)
        {
            float step = 0.12f;
            if (Math.Abs(_hoverTarget - _hoverAmount) <= step)
            {
                _hoverAmount = _hoverTarget;
                _hoverTimer.Stop();
            }
            else if (_hoverAmount < _hoverTarget)
            {
                _hoverAmount = Math.Min(_hoverTarget, _hoverAmount + step);
            }
            else
            {
                _hoverAmount = Math.Max(_hoverTarget, _hoverAmount - step);
            }

            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int lift = (int)Math.Round(_hoverAmount * 3f);
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            rect.Offset(0, -lift);
            int r = Math.Max(6, Math.Min(CornerRadius, Math.Min(rect.Width, rect.Height) / 2));

            if (_hoverAmount > 0.05f)
            {
                var glowRect = new Rectangle(rect.X - 1, rect.Y + 2, rect.Width + 2, rect.Height + 4);
                using (GraphicsPath glowPath = UiPaths.RoundedRectangle(glowRect, r + 2))
                using (var glow = new SolidBrush(Color.FromArgb((int)(40 * _hoverAmount), ClinicUiTheme.ButtonGlow)))
                {
                    g.FillPath(glow, glowPath);
                }
            }

            Color fill = Blend(ClinicUiTheme.GlassFill, ClinicUiTheme.NavFillHover, _hoverAmount);
            using (GraphicsPath path = UiPaths.RoundedRectangle(rect, r))
            {
                using (var brush = new SolidBrush(fill))
                {
                    g.FillPath(brush, path);
                }

                using (var pen = new Pen(ClinicUiTheme.GlassBorder, 1f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    g.DrawPath(pen, path);
                }
            }

            var titleRect = new Rectangle(rect.Left + Padding.Left, rect.Top + Padding.Top, rect.Width - Padding.Horizontal, 28);
            TextRenderer.DrawText(
                g,
                Title ?? Text,
                Font,
                titleRect,
                ForeColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            if (!string.IsNullOrEmpty(Subtitle))
            {
                var subRect = new Rectangle(titleRect.Left, titleRect.Bottom, titleRect.Width, 22);
                TextRenderer.DrawText(
                    g,
                    Subtitle,
                    ClinicUiTheme.SubtitleFont,
                    subRect,
                    ClinicUiTheme.BodyText,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
        }

        private static Color Blend(Color from, Color to, float amount)
        {
            amount = Math.Max(0f, Math.Min(1f, amount));
            return Color.FromArgb(
                from.A,
                (int)(from.R + (to.R - from.R) * amount),
                (int)(from.G + (to.G - from.G) * amount),
                (int)(from.B + (to.B - from.B) * amount));
        }
    }
}
