using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ClinicVets
{
    internal static class ClinicDecorations
    {
        internal static void PaintFormBackground(Graphics g, Rectangle area, FormBackgroundStyle style, Image heroImage)
        {
            if (g == null || area.Width <= 0 || area.Height <= 0)
            {
                return;
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;
            PaintBaseGradient(g, area);

            switch (style)
            {
                case FormBackgroundStyle.LoginMinimal:
                    break;
                case FormBackgroundStyle.LoginHero:
                    PaintLoginHero(g, area, heroImage);
                    break;
                case FormBackgroundStyle.RegisterFocus:
                    PaintRegisterFocus(g, area);
                    break;
                case FormBackgroundStyle.DashboardWorkspace:
                    PaintDashboardWorkspace(g, area);
                    break;
                case FormBackgroundStyle.SearchWorkspace:
                    PaintSearchWorkspace(g, area);
                    break;
            }
        }

        internal static void PaintLuxuryBackground(Graphics g, Rectangle area)
        {
            PaintFormBackground(g, area, FormBackgroundStyle.RegisterFocus, null);
        }

        internal static void PaintAmbientPaws(Graphics g, Rectangle area)
        {
            DrawPaw(g, new PointF(area.Right - 64, area.Top + 56), 0.9f, ClinicUiTheme.DecorPaw);
            DrawPaw(g, new PointF(area.Left + 48, area.Bottom - 72), 0.7f, ClinicUiTheme.DecorPaw);
        }

        private static void PaintBaseGradient(Graphics g, Rectangle area)
        {
            using (var brush = new LinearGradientBrush(area, ClinicUiTheme.BgTop, ClinicUiTheme.BgBottom, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, area);
            }

            using (var mid = new LinearGradientBrush(
                new Rectangle(area.X, area.Y, area.Width, area.Height / 2),
                Color.FromArgb(0, 255, 255, 255),
                ClinicUiTheme.BgMid,
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(mid, area);
            }
        }

        private static void PaintLoginHero(Graphics g, Rectangle area, Image heroImage)
        {
            int splitX = (int)(area.Width * 0.48f);
            if (heroImage != null)
            {
                var dest = new Rectangle(splitX, 0, area.Width - splitX, area.Height);
                g.DrawImage(heroImage, dest);
                using (var overlay = new LinearGradientBrush(
                    dest,
                    Color.FromArgb(220, 8, 16, 32),
                    Color.FromArgb(40, 8, 16, 32),
                    LinearGradientMode.Horizontal))
                {
                    g.FillRectangle(overlay, dest);
                }
            }
            else
            {
                DrawBlurredOrb(g, area.Right - 120, area.Height / 2, 220, ClinicUiTheme.OrbCyan);
                DrawBlurredOrb(g, area.Right - 80, area.Height / 2 + 40, 160, ClinicUiTheme.OrbBlue);
                DrawPaw(g, new PointF(area.Right - 100, area.Height * 0.55f), 1.4f, ClinicUiTheme.DecorPaw);
            }

            using (var linePen = new Pen(Color.FromArgb(120, ClinicUiTheme.NeonLine), 1f))
            {
                g.DrawLine(linePen, splitX, 40, splitX, area.Height - 40);
            }

            DrawGlowLine(g, 44, area.Height - 120, area.Width / 2, area.Height - 120);
            DrawPaw(g, new PointF(56, area.Bottom - 48), 0.55f, ClinicUiTheme.DecorPaw);
            DrawMedicalCross(g, new Rectangle(24, area.Height - 52, 22, 22), Color.FromArgb(80, ClinicUiTheme.AccentCyan));
        }

        private static void PaintRegisterFocus(Graphics g, Rectangle area)
        {
            DrawBlurredOrb(g, area.Width / 2, area.Height / 4, 180, ClinicUiTheme.OrbCyan);
            DrawBlurredOrb(g, area.Width / 3, area.Height * 2 / 3, 140, ClinicUiTheme.OrbBlue);
            DrawGlowLine(g, 60, 100, area.Width - 60, 100);
            DrawPaw(g, new PointF(area.Width - 70, 80), 0.5f, ClinicUiTheme.DecorPaw);
            DrawPaw(g, new PointF(70, area.Height - 60), 0.45f, ClinicUiTheme.DecorPaw);
            DrawMedicalCross(g, new Rectangle(area.Width - 44, 24, 20, 20), Color.FromArgb(60, ClinicUiTheme.AccentCyan));
        }

        private static void PaintDashboardWorkspace(Graphics g, Rectangle area)
        {
            int sidebarW = Math.Min(ClinicUiTheme.SidebarDecorWidth, area.Width / 3);
            var sidebar = new Rectangle(0, ClinicUiTheme.HeaderHeight, sidebarW, area.Height - ClinicUiTheme.HeaderHeight);
            using (var sb = new LinearGradientBrush(sidebar, ClinicUiTheme.SidebarFill, Color.FromArgb(180, 14, 24, 42), LinearGradientMode.Horizontal))
            {
                g.FillRectangle(sb, sidebar);
            }

            using (var glow = new Pen(Color.FromArgb(100, ClinicUiTheme.SidebarGlow), 2f))
            {
                g.DrawLine(glow, sidebarW, sidebar.Top + 8, sidebarW, sidebar.Bottom - 8);
            }

            string[] navLabels = { "Customers", "Search", "Register", "Reports" };
            int y = sidebar.Top + 36;
            foreach (string label in navLabels)
            {
                var itemRect = new Rectangle(20, y, sidebarW - 36, 32);
                using (var brush = new SolidBrush(Color.FromArgb(40, 30, 58, 92)))
                using (var path = UiPaths.RoundedRectangle(itemRect, 8))
                {
                    g.FillPath(brush, path);
                }

                TextRenderer.DrawText(
                    g,
                    label,
                    ClinicUiTheme.LabelFont,
                    itemRect,
                    ClinicUiTheme.MutedText,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                y += 44;
            }

            DrawBlurredOrb(g, area.Width - 80, 100, 100, ClinicUiTheme.OrbCyan);
            DrawGlowLine(g, sidebarW + 40, area.Height - 80, area.Width - 40, area.Height - 80);
            DrawPaw(g, new PointF(area.Width - 56, area.Height - 48), 0.6f, ClinicUiTheme.DecorPaw);
        }

        private static void PaintSearchWorkspace(Graphics g, Rectangle area)
        {
            using (var arcPen = new Pen(Color.FromArgb(70, ClinicUiTheme.AccentCyan), 2f))
            {
                g.DrawArc(arcPen, area.Width / 4, -area.Height / 4, area.Width / 2, area.Height / 2, 20, 140);
            }

            for (int i = 0; i < 5; i++)
            {
                int y = 80 + i * 70;
                using (var pen = new Pen(Color.FromArgb(18, ClinicUiTheme.NeonLine), 1f))
                {
                    g.DrawLine(pen, 0, y, area.Width, y);
                }
            }

            DrawBlurredOrb(g, (int)(area.Width * 0.15f), (int)(area.Height * 0.2f), 110, ClinicUiTheme.OrbBlue);
            DrawBlurredOrb(g, (int)(area.Width * 0.75f), (int)(area.Height * 0.65f), 130, ClinicUiTheme.OrbCyan);
            DrawPaw(g, new PointF(area.Width * 0.12f, area.Height - 50), 0.55f, ClinicUiTheme.DecorPaw);
            DrawMedicalCross(g, new Rectangle(area.Width - 40, area.Height - 44, 18, 18), Color.FromArgb(55, ClinicUiTheme.AccentCyan));
        }

        private static void DrawGlowLine(Graphics g, int x1, int y1, int x2, int y2)
        {
            using (var glowPen = new Pen(Color.FromArgb(50, ClinicUiTheme.GlowCyan), 6f))
            {
                glowPen.StartCap = LineCap.Round;
                glowPen.EndCap = LineCap.Round;
                g.DrawLine(glowPen, x1, y1, x2, y2);
            }

            using (var pen = new Pen(Color.FromArgb(140, ClinicUiTheme.NeonLine), 1.2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, x1, y1, x2, y2);
            }
        }

        private static void DrawBlurredOrb(Graphics g, int centerX, int centerY, int diameter, Color color)
        {
            var rect = new Rectangle(centerX - diameter / 2, centerY - diameter / 2, diameter, diameter);
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(rect);
                using (var brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = color;
                    brush.SurroundColors = new[] { Color.FromArgb(0, color) };
                    brush.FocusScales = new PointF(0.55f, 0.55f);
                    g.FillPath(brush, path);
                }
            }
        }

        internal static void DrawPaw(Graphics g, PointF center, float scale, Color color)
        {
            if (g == null)
            {
                return;
            }

            using (var brush = new SolidBrush(color))
            {
                float pad = 3f * scale;
                float toe = 5f * scale;
                g.FillEllipse(brush, center.X - pad, center.Y + pad, toe * 2f, toe * 2f);
                g.FillEllipse(brush, center.X - pad - 10f * scale, center.Y - 2f * scale, toe * 1.6f, toe * 1.6f);
                g.FillEllipse(brush, center.X + pad + 2f * scale, center.Y - 2f * scale, toe * 1.6f, toe * 1.6f);
                g.FillEllipse(brush, center.X - pad - 5f * scale, center.Y - 10f * scale, toe * 1.4f, toe * 1.4f);
                g.FillEllipse(brush, center.X + pad - 3f * scale, center.Y - 10f * scale, toe * 1.4f, toe * 1.4f);
            }
        }

        internal static void DrawMedicalCross(Graphics g, Rectangle bounds, Color color)
        {
            if (g == null)
            {
                return;
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;
            int arm = Math.Max(2, bounds.Width / 5);
            int cx = bounds.Left + bounds.Width / 2;
            int cy = bounds.Top + bounds.Height / 2;
            int half = bounds.Width / 2;

            using (var brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, cx - arm / 2, cy - half, arm, bounds.Height);
                g.FillRectangle(brush, cx - half, cy - arm / 2, bounds.Width, arm);
            }
        }

        internal static void DrawLogoMark(Graphics g, Rectangle bounds)
        {
            if (g == null)
            {
                return;
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = UiPaths.RoundedRectangle(bounds, bounds.Width / 2))
            using (var brush = new LinearGradientBrush(
                bounds,
                ClinicUiTheme.ButtonPrimaryStart,
                ClinicUiTheme.ButtonPrimaryEnd,
                LinearGradientMode.ForwardDiagonal))
            {
                g.FillPath(brush, path);
            }

            using (var pen = new Pen(Color.FromArgb(100, ClinicUiTheme.AccentCyan), 1.5f))
            using (var path = UiPaths.RoundedRectangle(bounds, bounds.Width / 2))
            {
                g.DrawPath(pen, path);
            }

            DrawMedicalCross(g, new Rectangle(bounds.X + bounds.Width / 3, bounds.Y + bounds.Height / 3, bounds.Width / 3, bounds.Height / 3), Color.White);
        }

        internal static void PaintCardAccent(Graphics g, Rectangle rect, bool showPaws)
        {
            if (showPaws)
            {
                DrawPaw(g, new PointF(rect.Right - 28, rect.Top + 24), 0.5f, ClinicUiTheme.DecorPaw);
                DrawPaw(g, new PointF(rect.Left + 24, rect.Bottom - 20), 0.4f, ClinicUiTheme.DecorPaw);
            }
        }
    }
}
