using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>Rounded input host with focus glow and soft error state.</summary>
    public class ChromeTextPlate : Panel
    {
        private bool _isInvalid;
        private bool _isFocused;

        public bool IsInvalid
        {
            get => _isInvalid;
            set
            {
                if (_isInvalid == value)
                {
                    return;
                }

                _isInvalid = value;
                Invalidate();
            }
        }

        public bool IsFocused
        {
            get => _isFocused;
            set
            {
                if (_isFocused == value)
                {
                    return;
                }

                _isFocused = value;
                Invalidate();
            }
        }

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
            Padding = new Padding(2, 2, 2, 2);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            int radius = ClinicUiTheme.FieldRadius;

            if (_isFocused && !_isInvalid)
            {
                var glowRect = new Rectangle(rect.X - 1, rect.Y - 1, rect.Width + 2, rect.Height + 2);
                using (GraphicsPath glowPath = UiPaths.RoundedRectangle(glowRect, radius + 2))
                using (var glowBrush = new SolidBrush(ClinicUiTheme.FieldFocusGlow))
                {
                    g.FillPath(glowBrush, glowPath);
                }
            }

            Color fill = _isInvalid ? ClinicUiTheme.FieldInvalidFill : ClinicUiTheme.FieldFill;
            Color borderColor = _isInvalid
                ? ClinicUiTheme.FieldBorderError
                : (_isFocused ? ClinicUiTheme.FieldBorderFocus : ClinicUiTheme.FieldBorder);

            using (GraphicsPath path = UiPaths.RoundedRectangle(rect, radius))
            {
                using (var brush = new SolidBrush(fill))
                {
                    g.FillPath(brush, path);
                }

                float borderWidth = _isFocused && !_isInvalid ? 1.6f : 1f;
                using (var pen = new Pen(borderColor, borderWidth))
                {
                    pen.Alignment = PenAlignment.Inset;
                    g.DrawPath(pen, path);
                }
            }
        }

        public static void WrapDirectTextBoxes(Panel host)
        {
            if (host == null)
            {
                return;
            }

            foreach (Control child in host.Controls.Cast<Control>().ToList())
            {
                CardPanel.ApplyChildChrome(child);
            }

            foreach (TextBox tb in host.Controls.OfType<TextBox>().ToList())
            {
                WrapTextBox(host, tb);
            }

            foreach (ComboBox combo in host.Controls.OfType<ComboBox>().ToList())
            {
                WrapComboBox(host, combo);
            }
        }

        private static void WrapTextBox(Panel host, TextBox tb)
        {
            if (tb.Parent is ChromeTextPlate)
            {
                return;
            }

            AnchorStyles anchor = tb.Anchor;
            int tab = tb.TabIndex;
            Point loc = tb.Location;
            Size sz = tb.Size;
            int z = host.Controls.GetChildIndex(tb);

            var plate = new ChromeTextPlate
            {
                Location = loc,
                Size = sz,
                TabIndex = tab,
                TabStop = true,
            };

            host.Controls.Remove(tb);
            host.Controls.Add(plate);
            host.Controls.SetChildIndex(plate, z);

            try
            {
                tb.BorderStyle = BorderStyle.None;
                tb.BackColor = System.Drawing.Color.FromArgb(245, 250, 255);
                tb.ForeColor = System.Drawing.Color.FromArgb(20, 70, 110);
            }
            catch
            {
            }
            tb.TabIndex = 0;
            plate.Controls.Add(tb);
            FitInnerControl(tb, plate);
            tb.Anchor = anchor;
            WireFocusHandlers(plate, tb);
        }

        private static void WrapComboBox(Panel host, ComboBox combo)
        {
            if (combo.Parent is ChromeTextPlate)
            {
                return;
            }

            AnchorStyles anchor = combo.Anchor;
            int tab = combo.TabIndex;
            Point loc = combo.Location;
            Size sz = combo.Size;
            int z = host.Controls.GetChildIndex(combo);

            var plate = new ChromeTextPlate
            {
                Location = loc,
                Size = sz,
                TabIndex = tab,
                TabStop = true,
            };

            host.Controls.Remove(combo);
            host.Controls.Add(plate);
            host.Controls.SetChildIndex(plate, z);

            combo.FlatStyle = FlatStyle.Flat;
            combo.BackColor = ClinicUiTheme.FieldFill;
            combo.ForeColor = ClinicUiTheme.TitleText;
            combo.TabIndex = 0;
            plate.Controls.Add(combo);
            FitInnerControl(combo, plate);
            combo.Anchor = anchor;
            WireFocusHandlers(plate, combo);
        }

        private static void WireFocusHandlers(ChromeTextPlate plate, Control inner)
        {
            inner.GotFocus += (sender, args) => plate.IsFocused = true;
            inner.LostFocus += (sender, args) =>
            {
                if (!plate.ContainsFocus)
                {
                    plate.IsFocused = false;
                }
            };
        }

        private static void FitInnerControl(Control inner, ChromeTextPlate plate)
        {
            inner.Location = new Point(plate.Padding.Left + 4, plate.Padding.Top + 3);
            inner.Width = Math.Max(10, plate.ClientSize.Width - plate.Padding.Horizontal - 8);
            inner.Height = Math.Max(10, plate.ClientSize.Height - plate.Padding.Vertical - 6);
        }
    }
}
