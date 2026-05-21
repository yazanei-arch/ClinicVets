using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ClinicVets
{
    public enum VetMenuIcon
    {
        Pets,
        Visits,
        Medicines,
        Logout
    }

    /// <summary>Minimal dashboard row button with blue icon and outline.</summary>
    public class VetDashboardNavButton : Button
    {
        private static readonly Color PrimaryBlue = Color.FromArgb(0, 102, 204);
        private static readonly Color FillNormal = Color.White;
        private static readonly Color FillHover = Color.FromArgb(248, 252, 255);
        private static readonly Color FillPressed = Color.FromArgb(236, 246, 252);

        private bool _hover;
        private bool _pressed;

        public VetMenuIcon MenuIcon { get; set; } = VetMenuIcon.Pets;
        public int CornerRadius { get; set; } = 10;

        public VetDashboardNavButton()
        {
            BackColor = FillNormal;
            ForeColor = PrimaryBlue;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = FillNormal;
            FlatAppearance.MouseDownBackColor = FillNormal;
            UseVisualStyleBackColor = false;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
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
            Rectangle bounds = ClientRectangle;
            using (GraphicsPath path = UiPaths.RoundedRectangle(bounds, CornerRadius))
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
            RoundedControlPaint.Configure(g);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            RectangleF fillBounds = RoundedControlPaint.FillBounds(Width, Height);
            int radius = Math.Max(6, Math.Min(CornerRadius, (int)Math.Min(fillBounds.Width, fillBounds.Height) / 2));

            Color fill = FillNormal;
            if (_pressed)
            {
                fill = FillPressed;
            }
            else if (_hover)
            {
                fill = FillHover;
            }

            using (GraphicsPath path = UiPaths.RoundedRectangle(fillBounds, radius))
            using (var brush = new SolidBrush(fill))
            {
                g.FillPath(brush, path);
            }

            using (GraphicsPath path = UiPaths.RoundedRectangle(fillBounds, radius))
            using (var pen = new Pen(PrimaryBlue, 1f))
            {
                pen.Alignment = PenAlignment.Inset;
                pen.LineJoin = LineJoin.Round;
                g.DrawPath(pen, path);
            }

            var iconRect = new Rectangle((int)fillBounds.Left + 18, (int)fillBounds.Top + (int)(fillBounds.Height - 24) / 2, 24, 24);
            DrawIcon(g, iconRect, PrimaryBlue);

            var textRect = new Rectangle(
                iconRect.Right + 14,
                (int)fillBounds.Top,
                (int)fillBounds.Width - iconRect.Right - 28,
                (int)fillBounds.Height);

            TextRenderer.DrawText(
                g,
                Text,
                Font,
                textRect,
                PrimaryBlue,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        private void DrawIcon(Graphics g, Rectangle area, Color color)
        {
            switch (MenuIcon)
            {
                case VetMenuIcon.Pets:
                    DrawPetsIcon(g, area, color);
                    break;
                case VetMenuIcon.Visits:
                    DrawVisitsIcon(g, area, color);
                    break;
                case VetMenuIcon.Medicines:
                    DrawMedicinesIcon(g, area, color);
                    break;
                case VetMenuIcon.Logout:
                    DrawLogoutIcon(g, area, color);
                    break;
            }
        }

        private static void DrawPetsIcon(Graphics g, Rectangle area, Color color)
        {
            int cx = area.Left + area.Width / 2;
            int cy = area.Top + area.Height / 2 + 2;
            using (var pen = new Pen(color, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawEllipse(pen, cx - 8, cy - 6, 8, 7);
                g.DrawEllipse(pen, cx, cy - 6, 8, 7);
                g.DrawArc(pen, cx - 6, cy - 2, 12, 10, 20, 140);
            }
        }

        private static void DrawVisitsIcon(Graphics g, Rectangle area, Color color)
        {
            var cal = new Rectangle(area.Left + 3, area.Top + 4, area.Width - 6, area.Height - 8);
            using (var pen = new Pen(color, 1.8f))
            {
                g.DrawRectangle(pen, cal.X, cal.Y + 4, cal.Width, cal.Height - 4);
                g.DrawLine(pen, cal.X, cal.Y + 10, cal.Right, cal.Y + 10);
                g.DrawLine(pen, cal.X + 6, cal.Y, cal.X + 6, cal.Y + 8);
                g.DrawLine(pen, cal.Right - 6, cal.Y, cal.Right - 6, cal.Y + 8);
            }

            using (var brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, cal.X + 8, cal.Y + 14, 4, 4);
                g.FillRectangle(brush, cal.X + 14, cal.Y + 14, 4, 4);
            }
        }

        private static void DrawMedicinesIcon(Graphics g, Rectangle area, Color color)
        {
            int midX = area.Left + area.Width / 2;
            using (var pen = new Pen(color, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, midX, area.Top + 4, midX, area.Bottom - 4);
            }

            var cap = new Rectangle(midX - 7, area.Top + 3, 14, 6);
            using (var brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, cap);
            }
        }

        private static void DrawLogoutIcon(Graphics g, Rectangle area, Color color)
        {
            int midY = area.Top + area.Height / 2;
            using (var pen = new Pen(color, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, area.Left + 6, midY, area.Right - 10, midY);
                g.DrawLine(pen, area.Right - 10, midY, area.Right - 16, midY - 6);
                g.DrawLine(pen, area.Right - 10, midY, area.Right - 16, midY + 6);
            }

            g.DrawRectangle(new Pen(color, 1.8f), area.Left + 4, area.Top + 5, 8, area.Height - 10);
        }
    }
}
