using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>Premium rounded button with gradient fill and hover glow.</summary>
    public class RoundedActionButton : Button
    {
        private bool _pressed;
        private float _hoverAmount;
        private float _hoverTarget;
        private readonly Timer _hoverTimer;

        public RoundedActionButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            UseVisualStyleBackColor = false;
            Cursor = Cursors.Hand;
            Font = ClinicUiTheme.ButtonFont;
            CornerRadius = ClinicUiTheme.ButtonRadius;
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor,
                true);
            DoubleBuffered = true;
            BackColor = Color.Transparent;

            _hoverTimer = new Timer { Interval = 16 };
            _hoverTimer.Tick += HoverTimer_Tick;
        }

        public bool IsOutlineStyle { get; set; }

        public int CornerRadius { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _hoverTimer.Stop();
                _hoverTimer.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
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
            _pressed = false;
            _hoverTarget = 0f;
            _hoverTimer.Start();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _pressed = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                _pressed = false;
                Invalidate();
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        private void HoverTimer_Tick(object sender, EventArgs e)
        {
            float step = 0.14f;
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

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            int lift = (int)Math.Round(_hoverAmount * 2f);
            bounds.Offset(0, -lift);
            int r = Math.Max(6, Math.Min(CornerRadius, Math.Min(bounds.Width, bounds.Height) / 2));

            if (!Enabled)
            {
                PaintDisabled(g, bounds, r, Text, Font);
                return;
            }

            if (!IsOutlineStyle && _hoverAmount > 0.05f)
            {
                var glowRect = new Rectangle(bounds.X - 2, bounds.Y, bounds.Width + 4, bounds.Height + 6);
                using (GraphicsPath glowPath = UiPaths.RoundedRectangle(glowRect, r + 2))
                using (var glowBrush = new SolidBrush(Blend(Color.FromArgb(0, 0, 0, 0), ClinicUiTheme.ButtonGlow, _hoverAmount * 0.85f)))
                {
                    g.FillPath(glowBrush, glowPath);
                }
            }

            using (GraphicsPath path = UiPaths.RoundedRectangle(bounds, r))
            {
                if (IsOutlineStyle)
                {
                    PaintOutline(g, path, bounds);
                }
                else
                {
                    PaintPrimary(g, path, bounds);
                }
            }
        }

        private void PaintPrimary(Graphics g, GraphicsPath path, Rectangle bounds)
        {
            Color start = _pressed ? ClinicUiTheme.ButtonPrimaryPress : ClinicUiTheme.ButtonPrimaryStart;
            Color end = _pressed ? ClinicUiTheme.ButtonPrimaryPress : ClinicUiTheme.ButtonPrimaryEnd;
            if (!_pressed && _hoverAmount > 0f)
            {
                start = Blend(start, ClinicUiTheme.AccentBlueHover, _hoverAmount * 0.35f);
                end = Blend(end, ClinicUiTheme.AccentCyan, _hoverAmount * 0.25f);
            }

            using (var brush = new LinearGradientBrush(bounds, start, end, LinearGradientMode.Horizontal))
            {
                g.FillPath(brush, path);
            }

            TextRenderer.DrawText(
                g,
                Text,
                Font,
                bounds,
                Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private void PaintOutline(Graphics g, GraphicsPath path, Rectangle bounds)
        {
            Color fill = Blend(ClinicUiTheme.ButtonOutlineFill, ClinicUiTheme.ButtonOutlineHover, _hoverAmount);
            using (var brush = new SolidBrush(fill))
            {
                g.FillPath(brush, path);
            }

            using (var pen = new Pen(ClinicUiTheme.AccentBlue, 1.2f))
            {
                pen.Alignment = PenAlignment.Inset;
                g.DrawPath(pen, path);
            }

            TextRenderer.DrawText(
                g,
                Text,
                Font,
                bounds,
                ClinicUiTheme.AccentBlue,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private static void PaintDisabled(Graphics g, Rectangle bounds, int r, string text, Font font)
        {
            using (GraphicsPath path = UiPaths.RoundedRectangle(bounds, r))
            using (var brush = new SolidBrush(Color.FromArgb(90, 30, 45, 68)))
            {
                g.FillPath(brush, path);
            }

            TextRenderer.DrawText(
                g,
                text ?? string.Empty,
                font,
                bounds,
                ClinicUiTheme.MutedText,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
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
