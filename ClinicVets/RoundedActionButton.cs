using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>
    /// Rounded button with drop shadow, vertical gradient face, and hover/press feedback.
    /// </summary>
    public class RoundedActionButton : Button
    {
        private static readonly Color PrimaryTop = Color.FromArgb(255, 66, 165, 245);
        private static readonly Color PrimaryBottom = Color.FromArgb(255, 13, 92, 196);
        private static readonly Color PrimaryHoverTop = Color.FromArgb(255, 92, 181, 250);
        private static readonly Color PrimaryHoverBottom = Color.FromArgb(255, 21, 118, 214);
        private static readonly Color PrimaryPressedTop = Color.FromArgb(255, 18, 100, 188);
        private static readonly Color PrimaryPressedBottom = Color.FromArgb(255, 10, 70, 160);

        private static readonly Color OutlineBorder = Color.FromArgb(25, 118, 210);
        private static readonly Color OutlineTop = Color.FromArgb(255, 255, 255, 255);
        private static readonly Color OutlineBottom = Color.FromArgb(255, 225, 240, 252);
        private static readonly Color OutlineHoverTop = Color.FromArgb(255, 242, 249, 255);
        private static readonly Color OutlineHoverBottom = Color.FromArgb(255, 210, 232, 250);
        private static readonly Color OutlinePressedTop = Color.FromArgb(255, 210, 230, 248);
        private static readonly Color OutlinePressedBottom = Color.FromArgb(255, 188, 218, 244);

        private bool _hover;
        private bool _pressed;

        public RoundedActionButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            UseVisualStyleBackColor = false;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point);
            CornerRadius = 12;
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor,
                true);
            DoubleBuffered = true;
            BackColor = Color.Transparent;
        }

        public bool IsOutlineStyle { get; set; }

        public int CornerRadius { get; set; }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hover = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hover = false;
            _pressed = false;
            Invalidate();
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

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            int r = Math.Max(2, Math.Min(CornerRadius, Math.Min(bounds.Width, bounds.Height) / 2));

            if (!Enabled)
            {
                using (GraphicsPath path = UiPaths.RoundedRectangle(bounds, r))
                using (var b = new SolidBrush(Color.FromArgb(189, 189, 189)))
                {
                    g.FillPath(b, path);
                }

                TextRenderer.DrawText(
                    g,
                    Text,
                    Font,
                    bounds,
                    Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                return;
            }

            DrawButtonShadow(g, bounds, r);

            using (GraphicsPath path = UiPaths.RoundedRectangle(bounds, r))
            {
                if (IsOutlineStyle)
                {
                    Color top = OutlineTop;
                    Color bottom = OutlineBottom;
                    if (_pressed)
                    {
                        top = OutlinePressedTop;
                        bottom = OutlinePressedBottom;
                    }
                    else if (_hover)
                    {
                        top = OutlineHoverTop;
                        bottom = OutlineHoverBottom;
                    }

                    using (var grad = new LinearGradientBrush(bounds, top, bottom, LinearGradientMode.Vertical))
                    {
                        g.FillPath(grad, path);
                    }

                    using (var pen = new Pen(OutlineBorder, 1.8f))
                    {
                        pen.Alignment = PenAlignment.Inset;
                        g.DrawPath(pen, path);
                    }

                    DrawTopSheen(g, path);
                    TextRenderer.DrawText(
                        g,
                        Text,
                        Font,
                        bounds,
                        OutlineBorder,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }
                else
                {
                    Color top = PrimaryTop;
                    Color bottom = PrimaryBottom;
                    if (_pressed)
                    {
                        top = PrimaryPressedTop;
                        bottom = PrimaryPressedBottom;
                    }
                    else if (_hover)
                    {
                        top = PrimaryHoverTop;
                        bottom = PrimaryHoverBottom;
                    }

                    using (var grad = new LinearGradientBrush(bounds, top, bottom, LinearGradientMode.Vertical))
                    {
                        g.FillPath(grad, path);
                    }

                    DrawTopSheen(g, path);
                    TextRenderer.DrawText(
                        g,
                        Text,
                        Font,
                        bounds,
                        Color.White,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }
            }
        }

        private static void DrawButtonShadow(Graphics g, Rectangle bounds, int r)
        {
            var layers = new[]
            {
                new { OffX = 4, OffY = 5, Inflate = -6, Alpha = 18 },
                new { OffX = 3, OffY = 4, Inflate = -4, Alpha = 26 },
                new { OffX = 2, OffY = 3, Inflate = -2, Alpha = 34 },
            };

            foreach (var layer in layers)
            {
                var rect = new Rectangle(
                    bounds.X + layer.OffX,
                    bounds.Y + layer.OffY,
                    bounds.Width + layer.Inflate,
                    bounds.Height + layer.Inflate);
                using (GraphicsPath p = UiPaths.RoundedRectangle(rect, Math.Max(2, r - 1)))
                using (var brush = new SolidBrush(Color.FromArgb(layer.Alpha, 12, 40, 90)))
                {
                    g.FillPath(brush, p);
                }
            }
        }

        private static void DrawTopSheen(Graphics g, GraphicsPath path)
        {
            RectangleF b = path.GetBounds();
            float bandH = Math.Max(6f, b.Height * 0.32f);
            var topBand = new RectangleF(b.X + 2, b.Y + 2, b.Width - 4, bandH);
            g.SetClip(path);
            try
            {
                using (var br = new LinearGradientBrush(
                    topBand,
                    Color.FromArgb(72, 255, 255, 255),
                    Color.FromArgb(0, 255, 255, 255),
                    LinearGradientMode.Vertical))
                {
                    g.FillRectangle(br, Rectangle.Round(topBand));
                }
            }
            finally
            {
                g.ResetClip();
            }
        }
    }
}
