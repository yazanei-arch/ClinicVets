using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ClinicVets.UI
{
    public enum PetCardIcon
    {
        Add,
        Search,
        ViewAll,
        AnimalTypes
    }

    /// <summary>Modern dashboard action card — icon top, caption below.</summary>
    public class PetActionCard : Button
    {
        private bool _hover;
        private bool _pressed;

        public PetCardIcon IconKind { get; set; } = PetCardIcon.Add;
        public int CornerRadius { get; set; } = 16;

        public PetActionCard()
        {
            BackColor = PetDashboardTheme.CardFill;
            ForeColor = PetDashboardTheme.PrimaryBlue;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = PetDashboardTheme.CardFill;
            FlatAppearance.MouseDownBackColor = PetDashboardTheme.CardFill;
            UseVisualStyleBackColor = false;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI Semibold", 11.25F, FontStyle.Bold);
            Size = new Size(240, 188);
            TabStop = false;

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);
            SetStyle(ControlStyles.Selectable, false);
        }

        protected override bool ShowFocusCues => false;

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ApplyRoundedRegion();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyRoundedRegion();
        }

        private void ApplyRoundedRegion()
        {
            Region previous = Region;
            using (var path = CreateRoundRectPath(ClientRectangle, CornerRadius))
            {
                Region = new Region(path);
            }

            previous?.Dispose();
        }

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

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (mevent.Button == MouseButtons.Left)
            {
                _pressed = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            if (mevent.Button == MouseButtons.Left)
            {
                _pressed = false;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle card = ClientRectangle;
            card.Width -= 1;
            card.Height -= 1;

            Color fill = PetDashboardTheme.CardFill;
            if (_pressed)
            {
                fill = PetDashboardTheme.CardPressed;
            }
            else if (_hover)
            {
                fill = PetDashboardTheme.CardHover;
            }

            using (var path = CreateRoundRectPath(card, CornerRadius))
            using (var brush = new SolidBrush(fill))
            {
                g.FillPath(brush, path);
            }

            using (var path = CreateRoundRectPath(card, CornerRadius))
            using (var pen = new Pen(PetDashboardTheme.PrimaryBlue, 1f))
            {
                pen.Alignment = PenAlignment.Inset;
                g.DrawPath(pen, path);
            }

            const int iconSize = 58;
            const int iconTop = 30;
            const int textGap = 16;

            var iconRect = new Rectangle(
                card.Left + (card.Width - iconSize) / 2,
                card.Top + iconTop,
                iconSize,
                iconSize);

            switch (IconKind)
            {
                case PetCardIcon.Add:
                    DrawPlusIcon(g, iconRect, PetDashboardTheme.PrimaryBlue);
                    break;
                case PetCardIcon.Search:
                    DrawSearchIcon(g, iconRect, PetDashboardTheme.PrimaryBlue);
                    break;
                case PetCardIcon.ViewAll:
                    DrawViewAllIcon(g, iconRect, PetDashboardTheme.PrimaryBlue);
                    break;
                case PetCardIcon.AnimalTypes:
                    DrawAnimalTypesIcon(g, iconRect, PetDashboardTheme.PrimaryBlue);
                    break;
            }

            int textTop = iconRect.Bottom + textGap;
            var textRect = new Rectangle(card.Left + 12, textTop, card.Width - 24, card.Bottom - textTop - 16);
            TextRenderer.DrawText(
                g,
                Text,
                Font,
                textRect,
                PetDashboardTheme.PrimaryBlue,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private static void DrawPlusIcon(Graphics g, Rectangle bounds, Color color)
        {
            int cx = bounds.Left + bounds.Width / 2;
            int cy = bounds.Top + bounds.Height / 2;
            int arm = Math.Min(bounds.Width, bounds.Height) / 3;

            using (var pen = new Pen(color, 3f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, cx - arm, cy, cx + arm, cy);
                g.DrawLine(pen, cx, cy - arm, cx, cy + arm);
            }
        }

        private static void DrawSearchIcon(Graphics g, Rectangle bounds, Color color)
        {
            int pad = 4;
            int diameter = Math.Min(bounds.Width, bounds.Height) - 18;
            var lens = new Rectangle(
                bounds.Left + pad + 2,
                bounds.Top + pad + 2,
                diameter,
                diameter);

            using (var pen = new Pen(color, 2.6f))
            {
                g.DrawEllipse(pen, lens);
            }

            int lx = lens.Right - 3;
            int ly = lens.Bottom - 3;
            using (var pen = new Pen(color, 2.6f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, lx, ly, bounds.Right - pad, bounds.Bottom - pad);
            }
        }

        private static void DrawViewAllIcon(Graphics g, Rectangle bounds, Color color)
        {
            var list = new Rectangle(bounds.Left + 8, bounds.Top + 6, bounds.Width - 16, bounds.Height - 12);
            using (var pen = new Pen(color, 2f))
            {
                g.DrawRectangle(pen, list);
            }

            int lineY = list.Top + 10;
            using (var pen = new Pen(color, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                for (int i = 0; i < 3; i++)
                {
                    g.DrawLine(pen, list.Left + 8, lineY, list.Right - 8, lineY);
                    lineY += 10;
                }
            }
        }

        private static void DrawAnimalTypesIcon(Graphics g, Rectangle bounds, Color color)
        {
            int cx = bounds.Left + bounds.Width / 2;
            int cy = bounds.Top + bounds.Height / 2 + 2;
            using (var pen = new Pen(color, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawEllipse(pen, cx - 10, cy - 8, 9, 8);
                g.DrawEllipse(pen, cx + 1, cy - 8, 9, 8);
                g.DrawArc(pen, cx - 8, cy - 2, 16, 10, 20, 140);
            }

            using (var pen = new Pen(color, 2.4f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, cx - 4, cy - 14, cx - 4, cy - 18);
                g.DrawLine(pen, cx + 4, cy - 14, cx + 4, cy - 18);
            }
        }

        internal static GraphicsPath CreateRoundRectPath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                return path;
            }

            int r = Math.Min(radius, Math.Min(bounds.Width, bounds.Height) / 2);
            int d = r * 2;
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
