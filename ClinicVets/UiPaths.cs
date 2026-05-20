using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;

namespace ClinicVets
{
    internal static class UiPaths
    {
        internal static GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
        {
            return RoundedRectangle(
                new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height),
                radius);
        }

        internal static GraphicsPath RoundedRectangle(RectangleF bounds, float radius)
        {
            float d = Math.Min(radius * 2f, Math.Min(bounds.Width, bounds.Height));
            var path = new GraphicsPath();
            if (d <= 1f)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(bounds.Left, bounds.Top, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Top, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    /// <summary>Shared anti-aliased painting helpers for rounded custom controls.</summary>
    internal static class RoundedControlPaint
    {
        internal static void Configure(Graphics g)
        {
            if (g == null)
            {
                return;
            }

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }

        /// <summary>Fully opaque fill avoids dark halos on anti-aliased edges.</summary>
        internal static Color OpaqueFill(Color color)
        {
            if (color.A == 255)
            {
                return color;
            }

            return Color.FromArgb(255, color.R, color.G, color.B);
        }

        internal static RectangleF FillBounds(int width, int height, float inset = 0f)
        {
            return new RectangleF(inset, inset, width - inset * 2f, height - inset * 2f);
        }

        internal static RectangleF BorderBounds(int width, int height, float inset = 0.5f)
        {
            return new RectangleF(inset, inset, width - inset * 2f, height - inset * 2f);
        }
    }

    internal static class WinFormsUi
    {
        internal static void SetDoubleBuffered(Control control)
        {
            if (control == null)
            {
                return;
            }

            typeof(Control).InvokeMember(
                "DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetProperty,
                null,
                control,
                new object[] { true });
        }
    }
}
