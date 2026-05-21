using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ClinicVets.UI
{
    /// <summary>Rounded outline button with hover fill for pet management screens.</summary>
    public class PetModernButton : Button
    {
        private static readonly Color OutlineBlue = Color.FromArgb(25, 118, 210);
        private static readonly Color OutlineHover = Color.FromArgb(0, 151, 167);
        private static readonly Color FillNormal = Color.FromArgb(252, 253, 255);
        private static readonly Color FillHover = Color.FromArgb(232, 246, 252);
        private static readonly Color GlassFillNormal = Color.FromArgb(52, 255, 255, 255);
        private static readonly Color GlassFillHover = Color.FromArgb(108, 240, 248, 255);

        private bool _pressed;
        private float _hoverAmount;
        private float _hoverTarget;
        private readonly Timer _hoverTimer;

        public int CornerRadius { get; set; } = 12;

        public bool GlassStyle { get; set; }

        public PetModernButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            UseVisualStyleBackColor = false;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
            ForeColor = OutlineBlue;
            BackColor = Color.Transparent;
            Size = new Size(220, 56);
            Margin = new Padding(12, 8, 12, 8);

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor,
                true);
            DoubleBuffered = true;

            _hoverTimer = new Timer { Interval = 16 };
            _hoverTimer.Tick += (s, e) =>
            {
                float step = 0.14f;
                if (_hoverAmount < _hoverTarget)
                {
                    _hoverAmount = Math.Min(_hoverTarget, _hoverAmount + step);
                }
                else if (_hoverAmount > _hoverTarget)
                {
                    _hoverAmount = Math.Max(_hoverTarget, _hoverAmount - step);
                }
                else
                {
                    _hoverTimer.Stop();
                }

                Invalidate();
            };
        }

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
            _pressed = false;
            _hoverTarget = 0f;
            _hoverTimer.Start();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            _pressed = true;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            _pressed = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pevent.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Rectangle bounds = ClientRectangle;
            bounds.Inflate(-1, -1);
            int radius = CornerRadius;

            Color fillNormal = GlassStyle ? GlassFillNormal : FillNormal;
            Color fillHover = GlassStyle ? GlassFillHover : FillHover;
            Color fill = InterpolateColor(fillNormal, fillHover, _hoverAmount);
            if (_pressed)
            {
                fill = GlassStyle
                    ? Color.FromArgb(140, 220, 238, 252)
                    : Color.FromArgb(220, 210, 236, 252);
            }

            using (var path = CreateRoundRect(bounds, radius))
            using (var brush = new SolidBrush(fill))
            {
                pevent.Graphics.FillPath(brush, path);
            }

            Color border = InterpolateColor(OutlineBlue, OutlineHover, _hoverAmount);
            using (var path = CreateRoundRect(bounds, radius))
            using (var pen = new Pen(border, _pressed ? 2.2f : 1.6f))
            {
                pevent.Graphics.DrawPath(pen, path);
            }

            TextRenderer.DrawText(
                pevent.Graphics,
                Text,
                Font,
                bounds,
                ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private static Color InterpolateColor(Color from, Color to, float amount)
        {
            amount = Math.Max(0f, Math.Min(1f, amount));
            int a = (int)(from.A + (to.A - from.A) * amount);
            int r = (int)(from.R + (to.R - from.R) * amount);
            int g = (int)(from.G + (to.G - from.G) * amount);
            int b = (int)(from.B + (to.B - from.B) * amount);
            return Color.FromArgb(a, r, g, b);
        }

        private static GraphicsPath CreateRoundRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
