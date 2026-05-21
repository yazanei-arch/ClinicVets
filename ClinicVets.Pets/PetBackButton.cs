using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ClinicVets.UI
{
    /// <summary>Top-left back control — rounded white pill with blue outline and arrow.</summary>
    public class PetBackButton : Button
    {
        private bool _hover;
        private bool _pressed;

        public int CornerRadius { get; set; } = 10;

        public PetBackButton()
        {
            BackColor = PetDashboardTheme.CardFill;
            ForeColor = PetDashboardTheme.PrimaryBlue;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = PetDashboardTheme.CardFill;
            FlatAppearance.MouseDownBackColor = PetDashboardTheme.CardFill;
            UseVisualStyleBackColor = false;
            Cursor = Cursors.Hand;
            Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            Size = new Size(112, 40);
            Text = "Back";
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
            using (var path = PetActionCard.CreateRoundRectPath(ClientRectangle, CornerRadius))
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
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle bounds = ClientRectangle;
            bounds.Width -= 1;
            bounds.Height -= 1;

            Color fill = PetDashboardTheme.CardFill;
            if (_pressed)
            {
                fill = PetDashboardTheme.ButtonPressed;
            }
            else if (_hover)
            {
                fill = PetDashboardTheme.ButtonHover;
            }

            using (var path = PetActionCard.CreateRoundRectPath(bounds, CornerRadius))
            using (var brush = new SolidBrush(fill))
            {
                g.FillPath(brush, path);
            }

            using (var path = PetActionCard.CreateRoundRectPath(bounds, CornerRadius))
            using (var pen = new Pen(PetDashboardTheme.PrimaryBlue, 1f))
            {
                pen.Alignment = PenAlignment.Inset;
                g.DrawPath(pen, path);
            }

            DrawArrow(g, new Rectangle(bounds.Left + 14, bounds.Top + 8, 20, bounds.Height - 16), PetDashboardTheme.PrimaryBlue);

            var textRect = new Rectangle(bounds.Left + 36, bounds.Top, bounds.Width - 40, bounds.Height);
            TextRenderer.DrawText(
                g,
                Text,
                Font,
                textRect,
                PetDashboardTheme.PrimaryBlue,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        private static void DrawArrow(Graphics g, Rectangle area, Color color)
        {
            int y = area.Top + area.Height / 2;
            int tipX = area.Left + 2;
            int joinX = area.Left + 10;
            int tailX = area.Right - 2;

            using (var pen = new Pen(color, 2.2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, tailX, y, joinX, y);
                g.DrawLine(pen, joinX, y, tipX, y - 6);
                g.DrawLine(pen, joinX, y, tipX, y + 6);
            }
        }
    }
}
