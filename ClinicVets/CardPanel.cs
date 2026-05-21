using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>Content card with dark glass or light login styling.</summary>
    public class CardPanel : Panel
    {
        private static readonly Color LoginPanelFill = Color.FromArgb(248, 252, 255);
        private static readonly Color LoginPanelBorder = Color.FromArgb(220, 235, 245);
        private static readonly Color LoginPanelBackdrop = Color.FromArgb(248, 252, 255);
        internal static readonly Color RegisterCardFill = Color.FromArgb(252, 253, 255);

        internal const int LoginCornerRadius = 10;
        internal const int RegisterCornerRadius = 8;

        public CardPanel()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer,
                true);
            DoubleBuffered = true;
            BackColor = LoginPanelBackdrop;
            CornerRadius = ClinicUiTheme.PanelRadius;
        }

        public int CornerRadius { get; set; }

        public bool ShowCornerDecorations { get; set; }

        /// <summary>Solid card fill to avoid transparency artifacts.</summary>
        public bool UseOpaqueFill { get; set; }

        /// <summary>Light login panel: no shadow, soft blue-gray border only.</summary>
        public bool UseLoginLightStyle { get; set; }

        /// <summary>Soft blue drop shadow under light menu cards (veterinarian menu).</summary>
        public bool ShowSoftDropShadow { get; set; }

        /// <summary>Semi-transparent white panel over a background image (register form).</summary>
        public bool UseRegisterLightStyle { get; set; }

        private bool UsesTransparentOverlay => UseRegisterLightStyle;

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
            if (UsesTransparentOverlay)
            {
                return;
            }

            Color backdrop = ResolveBackdropColor();
            using (var brush = new SolidBrush(backdrop))
            {
                pevent.Graphics.FillRectangle(brush, ClientRectangle);
            }
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
                if (control.Parent is CardPanel card && card.UseRegisterLightStyle)
                {
                    label.BackColor = RegisterCardFill;
                }
                else
                {
                    label.BackColor = Color.Transparent;
                }
            }
            else if (control is RoundedActionButton || control is DataGridView)
            {
                control.BackColor = ClinicUiTheme.SoftChromeSurface;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            RoundedControlPaint.Configure(g);

            RectangleF fillRect = RoundedControlPaint.FillBounds(Width, Height);
            int r = ResolveCornerRadius(fillRect);

            if (UseRegisterLightStyle)
            {
                PaintRegisterLightCard(g, fillRect, r);
                return;
            }

            if (UseLoginLightStyle)
            {
                PaintLoginLightCard(g, fillRect, r);
                return;
            }

            Color fillColor = RoundedControlPaint.OpaqueFill(
                UseOpaqueFill ? ClinicUiTheme.SolidCardFill : ClinicUiTheme.GlassFill);
            Color backdrop = ResolveBackdropColor();
            PaintRoundedSurface(g, Width, Height, fillRect, r, backdrop, fillColor, ClinicUiTheme.GlassBorder, drawGlassGlow: !UseOpaqueFill);

            if (ShowCornerDecorations)
            {
                var decorRect = Rectangle.Round(fillRect);
                ClinicDecorations.PaintCardAccent(g, decorRect, true);
            }
        }

        private void PaintLoginLightCard(Graphics g, RectangleF fillRect, int radius)
        {
            if (ShowSoftDropShadow)
            {
                RectangleF shadowRect = fillRect;
                shadowRect.Offset(0f, 4f);
                using (GraphicsPath shadowPath = UiPaths.RoundedRectangle(shadowRect, radius))
                using (var shadowBrush = new SolidBrush(Color.FromArgb(34, 140, 175, 205)))
                {
                    g.FillPath(shadowBrush, shadowPath);
                }
            }

            Color backdrop = ResolveBackdropColor();
            PaintRoundedSurface(g, Width, Height, fillRect, radius, backdrop, LoginPanelFill, LoginPanelBorder, drawGlassGlow: false);
        }

        private void PaintRegisterLightCard(Graphics g, RectangleF fillRect, int radius)
        {
            RectangleF shadowRect = fillRect;
            shadowRect.Offset(0f, 3f);
            using (GraphicsPath shadowPath = UiPaths.RoundedRectangle(shadowRect, radius))
            using (var shadowBrush = new SolidBrush(Color.FromArgb(32, 120, 150, 175)))
            {
                g.FillPath(shadowBrush, shadowPath);
            }

            using (GraphicsPath path = UiPaths.RoundedRectangle(fillRect, radius))
            using (var fillBrush = new SolidBrush(RegisterCardFill))
            {
                g.FillPath(fillBrush, path);
            }
        }

        private int ResolveCornerRadius(RectangleF fillRect)
        {
            if (UseRegisterLightStyle)
            {
                return RegisterCornerRadius;
            }

            if (UseLoginLightStyle)
            {
                return LoginCornerRadius;
            }

            return Math.Max(6, Math.Min(CornerRadius, (int)Math.Min(fillRect.Width, fillRect.Height) / 2));
        }

        private void SyncSurfaceBackColor()
        {
            Color backdrop = UsesTransparentOverlay ? RegisterCardFill : ResolveBackdropColor();
            if (BackColor != backdrop)
            {
                BackColor = backdrop;
            }
        }

        private Color ResolveBackdropColor()
        {
            if (UseLoginLightStyle || UseRegisterLightStyle)
            {
                return ResolveParentSurfaceColor(this, Color.FromArgb(232, 244, 252));
            }

            return ResolveParentSurfaceColor(this, ClinicUiTheme.BgTop);
        }

        internal static Color ResolveParentSurfaceColor(Control control, Color fallback)
        {
            for (Control parent = control?.Parent; parent != null; parent = parent.Parent)
            {
                if (parent.BackColor.A > 0 && parent.BackColor != Color.Transparent)
                {
                    return parent.BackColor;
                }
            }

            return fallback;
        }

        private void UpdateRoundedRegion()
        {
            if (Width <= 0 || Height <= 0)
            {
                return;
            }

            RectangleF fillRect = RoundedControlPaint.FillBounds(Width, Height);
            int radius = ResolveCornerRadius(fillRect);

            using (GraphicsPath path = UiPaths.RoundedRectangle(fillRect, radius))
            {
                Region old = Region;
                Region = new Region(path);
                old?.Dispose();
            }
        }

        private static void PaintRoundedSurface(
            Graphics g,
            int width,
            int height,
            RectangleF fillRect,
            int radius,
            Color backdrop,
            Color fill,
            Color borderColor,
            bool drawGlassGlow)
        {
            var client = new RectangleF(0, 0, width, height);

            using (var backdropBrush = new SolidBrush(backdrop))
            {
                g.FillRectangle(backdropBrush, client);
            }

            using (GraphicsPath path = UiPaths.RoundedRectangle(fillRect, radius))
            using (var fillBrush = new SolidBrush(fill))
            {
                g.FillPath(fillBrush, path);
            }

            RectangleF borderRect = RoundedControlPaint.BorderBounds(width, height);
            using (GraphicsPath borderPath = UiPaths.RoundedRectangle(borderRect, radius))
            {
                if (drawGlassGlow)
                {
                    using (var glowPen = new Pen(Color.FromArgb(48, borderColor), 1.5f))
                    {
                        glowPen.Alignment = PenAlignment.Inset;
                        glowPen.LineJoin = LineJoin.Round;
                        g.DrawPath(glowPen, borderPath);
                    }
                }

                using (var border = new Pen(borderColor, 1f))
                {
                    border.Alignment = PenAlignment.Inset;
                    border.LineJoin = LineJoin.Round;
                    g.DrawPath(border, borderPath);
                }
            }
        }
    }

    /// <summary>Form1 login shell — light CardPanel preset.</summary>
    public class LoginCardPanel : CardPanel
    {
        public LoginCardPanel()
        {
            UseLoginLightStyle = true;
            CornerRadius = CardPanel.LoginCornerRadius;
        }
    }
}
