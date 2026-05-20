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

        /// <summary>Light borders/fill for Form1 login fields only.</summary>
        public bool UseLoginLightStyle { get; set; }

        private static readonly Color LoginFieldBorder = Color.FromArgb(190, 220, 240);
        private static readonly Color LoginFieldBorderFocus = Color.FromArgb(0, 172, 193);
        private static readonly Color LightInputSurface = Color.FromArgb(248, 252, 255);
        private static readonly Color LightInputText = Color.FromArgb(33, 52, 72);
        private static readonly Color DarkInputSurface = Color.FromArgb(255, 18, 30, 50);
        private static readonly Color LoginFieldBackdrop = LightInputSurface;
        private static readonly Color LoginFieldFill = Color.White;
        private static readonly Color LoginFieldInvalidBorder = Color.FromArgb(229, 115, 115);
        private static readonly Color LoginFieldInvalidFill = Color.FromArgb(255, 251, 250);
        private const int LoginFieldCornerRadius = 8;

        public ChromeTextPlate()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer,
                true);
            DoubleBuffered = true;
            BackColor = LoginFieldBackdrop;
            Padding = new Padding(2, 2, 2, 2);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            SyncSurfaceBackColor();
            UpdateRoundedRegion();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateRoundedRegion();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            Color backdrop = ResolveBackdropColor();
            using (var brush = new SolidBrush(backdrop))
            {
                pevent.Graphics.FillRectangle(brush, ClientRectangle);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            RoundedControlPaint.Configure(g);

            int radius = UseLoginLightStyle ? LoginFieldCornerRadius : ClinicUiTheme.FieldRadius;
            RectangleF fillRect = RoundedControlPaint.FillBounds(Width, Height);

            Color fill;
            Color borderColor;
            Color backdrop = ResolveBackdropColor();
            if (UseLoginLightStyle)
            {
                fill = _isInvalid ? LoginFieldInvalidFill : LoginFieldFill;
                borderColor = _isInvalid
                    ? LoginFieldInvalidBorder
                    : (_isFocused ? LoginFieldBorderFocus : LoginFieldBorder);
            }
            else
            {
                fill = RoundedControlPaint.OpaqueFill(
                    _isInvalid ? ClinicUiTheme.FieldInvalidFill : ClinicUiTheme.FieldFill);
                borderColor = _isInvalid
                    ? ClinicUiTheme.FieldBorderError
                    : (_isFocused ? ClinicUiTheme.FieldBorderFocus : ClinicUiTheme.FieldBorder);
            }

            var client = new RectangleF(0, 0, Width, Height);
            using (var backdropBrush = new SolidBrush(backdrop))
            {
                g.FillRectangle(backdropBrush, client);
            }

            if (_isFocused && !_isInvalid && !UseLoginLightStyle)
            {
                RectangleF glowRect = RectangleF.Inflate(fillRect, 1.5f, 1.5f);
                using (GraphicsPath glowPath = UiPaths.RoundedRectangle(glowRect, radius + 2))
                using (var glowBrush = new SolidBrush(ClinicUiTheme.FieldFocusGlow))
                {
                    g.FillPath(glowBrush, glowPath);
                }
            }

            using (GraphicsPath path = UiPaths.RoundedRectangle(fillRect, radius))
            using (var brush = new SolidBrush(fill))
            {
                g.FillPath(brush, path);
            }

            RectangleF borderRect = RoundedControlPaint.BorderBounds(Width, Height);
            using (GraphicsPath borderPath = UiPaths.RoundedRectangle(borderRect, radius))
            {
                float borderWidth = UseLoginLightStyle ? 1f : (_isFocused && !_isInvalid ? 1.5f : 1f);
                using (var pen = new Pen(borderColor, borderWidth))
                {
                    pen.Alignment = PenAlignment.Inset;
                    pen.LineJoin = LineJoin.Round;
                    g.DrawPath(pen, borderPath);
                }
            }
        }

        private void SyncSurfaceBackColor()
        {
            Color backdrop = ResolveBackdropColor();
            if (BackColor != backdrop)
            {
                BackColor = backdrop;
            }
        }

        private Color ResolveBackdropColor()
        {
            if (UseLoginLightStyle)
            {
                return LoginFieldBackdrop;
            }

            return CardPanel.ResolveParentSurfaceColor(this, ClinicUiTheme.BgTop);
        }

        private void UpdateRoundedRegion()
        {
            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            int radius = UseLoginLightStyle ? LoginFieldCornerRadius : ClinicUiTheme.FieldRadius;
            RectangleF fillRect = RoundedControlPaint.FillBounds(Width, Height);
            using (GraphicsPath path = UiPaths.RoundedRectangle(fillRect, radius))
            {
                Region old = Region;
                Region = new Region(path);
                old?.Dispose();
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

            if (host is CardPanel card && (card.UseLoginLightStyle || card.UseRegisterLightStyle))
            {
                plate.UseLoginLightStyle = true;
            }

            host.Controls.Remove(tb);
            host.Controls.Add(plate);
            host.Controls.SetChildIndex(plate, z);

            ApplyTextBoxChrome(tb, plate.UseLoginLightStyle);
            tb.TabIndex = 0;
            plate.Controls.Add(tb);
            plate.SyncSurfaceBackColor();
            plate.UpdateRoundedRegion();
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

            if (host is CardPanel card && (card.UseLoginLightStyle || card.UseRegisterLightStyle))
            {
                plate.UseLoginLightStyle = true;
            }

            host.Controls.Remove(combo);
            host.Controls.Add(plate);
            host.Controls.SetChildIndex(plate, z);

            ApplyComboBoxChrome(combo, plate.UseLoginLightStyle);
            combo.TabIndex = 0;
            plate.Controls.Add(combo);
            plate.SyncSurfaceBackColor();
            plate.UpdateRoundedRegion();
            FitInnerControl(combo, plate);
            combo.Anchor = anchor;
            WireFocusHandlers(plate, combo);
        }

        private static void ApplyTextBoxChrome(TextBox textBox, bool lightStyle)
        {
            textBox.BorderStyle = BorderStyle.None;
            if (lightStyle)
            {
                textBox.BackColor = LightInputSurface;
                textBox.ForeColor = LightInputText;
            }
            else
            {
                textBox.BackColor = DarkInputSurface;
                textBox.ForeColor = ClinicUiTheme.TitleText;
            }
        }

        private static void ApplyComboBoxChrome(ComboBox combo, bool lightStyle)
        {
            combo.FlatStyle = FlatStyle.Flat;
            if (lightStyle)
            {
                combo.BackColor = LightInputSurface;
                combo.ForeColor = LightInputText;
            }
            else
            {
                combo.BackColor = DarkInputSurface;
                combo.ForeColor = ClinicUiTheme.TitleText;
            }
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
