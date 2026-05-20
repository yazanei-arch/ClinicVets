using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>Rounded dark container for DataGridView tables.</summary>
    public class GridContainerPanel : Panel
    {
        public GridContainerPanel()
        {
            Padding = new Padding(8, 8, 8, 8);
            BackColor = Color.Transparent;
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);
            DoubleBuffered = true;
        }

        public int CornerRadius { get; set; } = ClinicUiTheme.FieldRadius;

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            int r = Math.Max(6, CornerRadius);

            using (GraphicsPath path = UiPaths.RoundedRectangle(rect, r))
            {
                using (var fill = new SolidBrush(ClinicUiTheme.GridContainerFill))
                {
                    g.FillPath(fill, path);
                }

                using (var pen = new Pen(ClinicUiTheme.GlassBorder, 1f))
                {
                    pen.Alignment = PenAlignment.Inset;
                    g.DrawPath(pen, path);
                }
            }
        }
    }
}
