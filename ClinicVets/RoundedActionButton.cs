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
            FlatAppearance.BorderColor = Color.FromArgb(0, 25, 118, 210);
            FlatAppearance.MouseOverBackColor = ClinicUiTheme.SoftChromeSurface;
            FlatAppearance.MouseDownBackColor = ClinicUiTheme.SoftChromeSurface;
            FlatAppearance.CheckedBackColor = ClinicUiTheme.SoftChromeSurface;
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
            BackColor = ClinicUiTheme.SoftChromeSurface;

            _hoverTimer = new Timer { Interval = 16 };
            _hoverTimer.Tick += HoverTimer_Tick;
        }

        public bool IsOutlineStyle { get; set; }

        /// <summary>Calm blue/turquoise styling for Form1 login buttons only.</summary>
        public bool UseLoginLightStyle { get; set; }

        private static readonly Color LoginButtonBlue = Color.FromArgb(25, 118, 210);
        private static readonly Color LoginButtonCyan = Color.FromArgb(0, 151, 167);
        private static readonly Color LoginOutlineFill = Color.FromArgb(252, 253, 255);
        private static readonly Color LoginOutlineHoverFill = Color.FromArgb(232, 246, 252);
        private static readonly Color LoginButtonBackdrop = Color.FromArgb(248, 252, 255);

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
            RoundedControlPaint.Configure(g);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (UseLoginLightStyle)
            {
                using (var backdrop = new SolidBrush(LoginButtonBackdrop))
                {
                    g.FillRectangle(backdrop, 0, 0, Width, Height);
                }
            }

            int lift = UseLoginLightStyle ? 0 : (int)Math.Round(_hoverAmount * 2f);
            RectangleF fillBounds = RoundedControlPaint.FillBounds(Width, Height);
            fillBounds.Offset(0, -lift);
            int r = Math.Max(6, Math.Min(CornerRadius, (int)Math.Min(fillBounds.Width, fillBounds.Height) / 2));
            var textBounds = Rectangle.Round(fillBounds);

            if (!Enabled)
            {
                PaintDisabled(g, fillBounds, r, textBounds, Text, Font);
                return;
            }

            if (!UseLoginLightStyle && !IsOutlineStyle && _hoverAmount > 0.05f)
            {
                RectangleF glowRect = RectangleF.Inflate(fillBounds, 2f, 3f);
                using (GraphicsPath glowPath = UiPaths.RoundedRectangle(glowRect, r + 2))
                using (var glowBrush = new SolidBrush(Color.FromArgb(
                    (int)(ClinicUiTheme.ButtonGlow.A * _hoverAmount * 0.85f),
                    ClinicUiTheme.ButtonGlow.R,
                    ClinicUiTheme.ButtonGlow.G,
                    ClinicUiTheme.ButtonGlow.B)))
                {
                    g.FillPath(glowBrush, glowPath);
                }
            }

            using (GraphicsPath path = UiPaths.RoundedRectangle(fillBounds, r))
            {
                if (IsOutlineStyle)
                {
                    PaintOutline(g, path, textBounds);
                }
                else
                {
                    PaintPrimary(g, path, textBounds);
                }
            }
        }

        private void PaintPrimary(Graphics g, GraphicsPath path, Rectangle textBounds)
        {
            Color start;
            Color end;
            if (UseLoginLightStyle)
            {
                start = _pressed ? Color.FromArgb(21, 101, 192) : LoginButtonBlue;
                end = _pressed ? Color.FromArgb(0, 131, 143) : LoginButtonCyan;
            }
            else
            {
                start = _pressed ? ClinicUiTheme.ButtonPrimaryPress : ClinicUiTheme.ButtonPrimaryStart;
                end = _pressed ? ClinicUiTheme.ButtonPrimaryPress : ClinicUiTheme.ButtonPrimaryEnd;
            }

            if (!_pressed && _hoverAmount > 0f)
            {
                Color hover = UseLoginLightStyle ? Color.FromArgb(66, 165, 245) : ClinicUiTheme.AccentBlueHover;
                start = Blend(start, hover, _hoverAmount * 0.35f);
                end = Blend(end, UseLoginLightStyle ? Color.FromArgb(38, 198, 218) : ClinicUiTheme.AccentCyan, _hoverAmount * 0.25f);
            }

            using (var brush = new LinearGradientBrush(textBounds, start, end, LinearGradientMode.Horizontal))
            {
                g.FillPath(brush, path);
            }

            TextRenderer.DrawText(
                g,
                Text,
                Font,
                textBounds,
                Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private void PaintOutline(Graphics g, GraphicsPath path, Rectangle textBounds)
        {
            if (UseLoginLightStyle)
            {
                Color fill = Blend(LoginOutlineFill, LoginOutlineHoverFill, _hoverAmount);
                using (var brush = new SolidBrush(fill))
                {
                    g.FillPath(brush, path);
                }

                using (var pen = new Pen(LoginButtonBlue, 1.2f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    pen.LineJoin = LineJoin.Round;
                    g.DrawPath(pen, path);
                }

                TextRenderer.DrawText(
                    g,
                    Text,
                    Font,
                    textBounds,
                    LoginButtonBlue,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                return;
            }

            Color outlineFill = RoundedControlPaint.OpaqueFill(
                Blend(ClinicUiTheme.ButtonOutlineFill, ClinicUiTheme.ButtonOutlineHover, _hoverAmount));
            using (var brush = new SolidBrush(outlineFill))
            {
                g.FillPath(brush, path);
            }

            using (var pen = new Pen(ClinicUiTheme.AccentBlue, 1.2f))
            {
                pen.Alignment = PenAlignment.Inset;
                pen.LineJoin = LineJoin.Round;
                g.DrawPath(pen, path);
            }

            TextRenderer.DrawText(
                g,
                Text,
                Font,
                textBounds,
                ClinicUiTheme.AccentBlue,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private static void PaintDisabled(Graphics g, RectangleF fillBounds, int r, Rectangle textBounds, string text, Font font)
        {
            using (GraphicsPath path = UiPaths.RoundedRectangle(fillBounds, r))
            {
                using (var brush = new SolidBrush(Color.FromArgb(200, 72, 96, 128)))
                {
                    g.FillPath(brush, path);
                }

                using (var pen = new Pen(Color.FromArgb(100, 100, 140, 170), 1f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    pen.LineJoin = LineJoin.Round;
                    g.DrawPath(pen, path);
                }
            }

            TextRenderer.DrawText(
                g,
                text ?? string.Empty,
                font,
                textBounds,
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
