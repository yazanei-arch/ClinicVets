using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>
    /// Elevated glass-style card with layered depth, soft shadow, and light corner decorations.
    /// </summary>
    public class CardPanel : Panel
    {
        public CardPanel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor,
                true);
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            CornerRadius = 22;
        }

        public int CornerRadius { get; set; }

        public bool ShowCornerDecorations { get; set; } = true;

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Rectangle full = new Rectangle(0, 0, Width - 1, Height - 1);
            int r = Math.Max(2, Math.Min(CornerRadius, Math.Min(full.Width - 20, full.Height - 20) / 2));

            DrawElevatedShadow(g, full, r);

            var cardRect = new Rectangle(0, 0, full.Width - 6, full.Height - 6);
            using (GraphicsPath cardPath = UiPaths.RoundedRectangle(cardRect, r))
            {
                using (var edgePen = new Pen(Color.FromArgb(150, 30, 120, 200), 1.15f))
                {
                    edgePen.Alignment = PenAlignment.Inset;

                    using (var grad = new LinearGradientBrush(
                        cardRect,
                        Color.FromArgb(238, 255, 255, 255),
                        Color.FromArgb(175, 218, 236, 252),
                        LinearGradientMode.Vertical))
                    {
                        g.SetClip(cardPath);
                        try
                        {
                            g.FillRectangle(grad, cardRect);
                            using (var clinicTint = new SolidBrush(Color.FromArgb(32, 33, 150, 243)))
                            {
                                g.FillRectangle(clinicTint, cardRect);
                            }

                            int sheenH = Math.Min(56, cardRect.Height / 3);
                            using (var innerLight = new LinearGradientBrush(
                                new Rectangle(cardRect.Left, cardRect.Top, cardRect.Width, sheenH),
                                Color.FromArgb(70, 255, 255, 255),
                                Color.FromArgb(0, 255, 255, 255),
                                LinearGradientMode.Vertical))
                            {
                                g.FillRectangle(innerLight, cardRect.Left, cardRect.Top, cardRect.Width, sheenH);
                            }

                            int floorH = Math.Min(72, cardRect.Height / 3);
                            var floor = new Rectangle(
                                cardRect.Left,
                                cardRect.Bottom - floorH,
                                cardRect.Width,
                                floorH);
                            using (var floorShade = new LinearGradientBrush(
                                floor,
                                Color.FromArgb(0, 25, 80, 140),
                                Color.FromArgb(38, 25, 80, 140),
                                LinearGradientMode.Vertical))
                            {
                                g.FillRectangle(floorShade, floor);
                            }
                        }
                        finally
                        {
                            g.ResetClip();
                        }

                        using (var rim = new Pen(Color.FromArgb(90, 255, 255, 255), 1f))
                        {
                            rim.Alignment = PenAlignment.Inset;
                            var rimRect = new Rectangle(cardRect.Left + 1, cardRect.Top + 1, cardRect.Width - 3, cardRect.Height - 3);
                            using (GraphicsPath rimPath = UiPaths.RoundedRectangle(rimRect, Math.Max(2, r - 2)))
                            {
                                g.DrawPath(rim, rimPath);
                            }
                        }

                        g.DrawPath(edgePen, cardPath);
                    }
                }
            }

            if (ShowCornerDecorations)
            {
                DrawCornerDecorations(g, cardRect, r);
            }
        }

        private static void DrawElevatedShadow(Graphics g, Rectangle full, int r)
        {
            var layers = new[]
            {
                new { Rect = new Rectangle(14, 16, full.Width - 18, full.Height - 18), Alpha = 14, R = Math.Max(2, r - 4) },
                new { Rect = new Rectangle(11, 13, full.Width - 14, full.Height - 14), Alpha = 20, R = Math.Max(2, r - 3) },
                new { Rect = new Rectangle(8, 10, full.Width - 12, full.Height - 12), Alpha = 28, R = Math.Max(2, r - 2) },
                new { Rect = new Rectangle(6, 8, full.Width - 10, full.Height - 10), Alpha = 36, R = Math.Max(2, r - 1) },
                new { Rect = new Rectangle(4, 6, full.Width - 8, full.Height - 8), Alpha = 44, R = r },
            };

            foreach (var layer in layers)
            {
                using (GraphicsPath path = UiPaths.RoundedRectangle(layer.Rect, layer.R))
                using (var brush = new SolidBrush(Color.FromArgb(layer.Alpha, 18, 55, 110)))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        private static void DrawCornerDecorations(Graphics g, Rectangle cardRect, int r)
        {
            int pad = Math.Min(48, cardRect.Width / 8);
            using (var paw = new SolidBrush(Color.FromArgb(22, 25, 118, 210)))
            using (var crossPen = new Pen(Color.FromArgb(26, 25, 118, 210), 1.2f))
            {
                crossPen.StartCap = LineCap.Round;
                crossPen.EndCap = LineCap.Round;

                float x1 = cardRect.Right - pad;
                float y1 = cardRect.Top + pad * 0.35f;
                g.FillEllipse(paw, x1 - 4, y1 - 4, 8, 8);
                g.FillEllipse(paw, x1 - 18, y1 + 2, 7, 7);
                g.FillEllipse(paw, x1 + 10, y1 + 2, 7, 7);
                g.FillEllipse(paw, x1 - 12, y1 + 14, 7, 7);
                g.FillEllipse(paw, x1 + 4, y1 + 14, 7, 7);

                float cx = cardRect.Left + pad * 0.55f;
                float cy = cardRect.Bottom - pad * 0.55f;
                float s = 10f;
                g.DrawLine(crossPen, cx - s * 0.45f, cy, cx + s * 0.45f, cy);
                g.DrawLine(crossPen, cx, cy - s * 0.45f, cx, cy + s * 0.45f);
            }
        }
    }
}
