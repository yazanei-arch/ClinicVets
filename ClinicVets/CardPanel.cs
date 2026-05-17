using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>Dark glass content card with soft cyan edge glow.</summary>
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
            CornerRadius = ClinicUiTheme.PanelRadius;
        }

        public int CornerRadius { get; set; }

        public bool ShowCornerDecorations { get; set; }

        /// <summary>Solid card fill to avoid transparency artifacts on login.</summary>
        public bool UseOpaqueFill { get; set; }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            ApplyChildChrome(e.Control);
        }

        internal static void ApplyChildChrome(Control control)
        {
            if (control == null)
            {
                return;
            }

            if (control is Label label)
            {
                label.BackColor = Color.Transparent;
            }
            else if (control is RoundedActionButton || control is DataGridView)
            {
                control.BackColor = Color.Transparent;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            int r = Math.Max(6, Math.Min(CornerRadius, Math.Min(rect.Width, rect.Height) / 2));

            using (GraphicsPath path = UiPaths.RoundedRectangle(rect, r))
            {
                Color fillColor = UseOpaqueFill ? ClinicUiTheme.SolidCardFill : ClinicUiTheme.GlassFill;
                using (var fill = new SolidBrush(fillColor))
                {
                    g.FillPath(fill, path);
                }

                if (!UseOpaqueFill)
                {
                    using (var glowPen = new Pen(Color.FromArgb(60, ClinicUiTheme.GlassBorder), 1.5f))
                    {
                        glowPen.Alignment = PenAlignment.Inset;
                        g.DrawPath(glowPen, path);
                    }
                }

                using (var border = new Pen(ClinicUiTheme.GlassBorder, 1f))
                {
                    border.Alignment = PenAlignment.Inset;
                    g.DrawPath(border, path);
                }
            }

            if (ShowCornerDecorations)
            {
                ClinicDecorations.PaintCardAccent(g, rect, true);
            }
        }
    }
}
