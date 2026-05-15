using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>
    /// Subtle raised chrome behind a text field (soft shadow + rounded plate). TextBox is docked inside.
    /// </summary>
    public class ChromeTextPlate : Panel
    {
        public ChromeTextPlate()
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
            Padding = new Padding(2, 2, 2, 3);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Rectangle outer = new Rectangle(1, 1, Width - 3, Height - 3);
            int r = Math.Max(4, Math.Min(10, Math.Min(outer.Width, outer.Height) / 3));

            var shadow = new Rectangle(outer.X + 2, outer.Y + 2, outer.Width - 2, outer.Height - 2);
            using (GraphicsPath sp = UiPaths.RoundedRectangle(shadow, r))
            using (var sb = new SolidBrush(Color.FromArgb(28, 18, 55, 100)))
            {
                g.FillPath(sb, sp);
            }

            using (GraphicsPath path = UiPaths.RoundedRectangle(outer, r))
            {
                using (var fill = new LinearGradientBrush(
                    outer,
                    Color.FromArgb(255, 254, 255, 255),
                    Color.FromArgb(255, 244, 249, 255),
                    LinearGradientMode.Vertical))
                {
                    g.FillPath(fill, path);
                }

                using (var pen = new Pen(Color.FromArgb(110, 144, 202, 230), 1f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    g.DrawPath(pen, path);
                }

                using (var hi = new Pen(Color.FromArgb(70, 255, 255, 255), 1f))
                {
                    hi.Alignment = PenAlignment.Inset;
                    var inner = new Rectangle(outer.X + 1, outer.Y + 1, outer.Width - 3, outer.Height - 3);
                    using (GraphicsPath ip = UiPaths.RoundedRectangle(inner, Math.Max(2, r - 2)))
                    {
                        g.DrawPath(hi, ip);
                    }
                }
            }
        }

        /// <summary>
        /// Wraps each direct child <see cref="TextBox"/> of <paramref name="host"/> in a chrome plate (once).
        /// </summary>
        public static void WrapDirectTextBoxes(Panel host)
        {
            if (host == null)
            {
                return;
            }

            List<TextBox> boxes = host.Controls.OfType<TextBox>().ToList();
            foreach (TextBox tb in boxes)
            {
                if (tb.Parent is ChromeTextPlate)
                {
                    continue;
                }

                AnchorStyles anchor = tb.Anchor;
                int tab = tb.TabIndex;
                Point loc = tb.Location;
                Size sz = tb.Size;
                int z = host.Controls.GetChildIndex(tb);

                var plate = new ChromeTextPlate
                {
                    Location = new Point(loc.X - 2, loc.Y - 2),
                    Size = new Size(sz.Width + 4, sz.Height + 5),
                    TabIndex = tab,
                    TabStop = false,
                };

                host.Controls.Remove(tb);
                host.Controls.Add(plate);
                host.Controls.SetChildIndex(plate, z);

                tb.BorderStyle = BorderStyle.None;
                tb.BackColor = Color.FromArgb(255, 252, 253, 255);
                tb.TabIndex = tab;
                plate.Controls.Add(tb);
                tb.Location = new Point(plate.Padding.Left, plate.Padding.Top);
                tb.Width = plate.ClientSize.Width - plate.Padding.Horizontal;
                tb.Height = plate.ClientSize.Height - plate.Padding.Vertical;
                tb.Anchor = anchor;
            }
        }
    }
}
