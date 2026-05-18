using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>Animated green success checkmark for registration confirmation.</summary>
    public sealed class SuccessCheckmarkControl : Control
    {
        private static readonly Color CircleFill = Color.FromArgb(76, 175, 80);
        private static readonly Color CircleShadow = Color.FromArgb(40, 56, 142, 60);

        private readonly Timer _animationTimer;
        private float _progress;

        public SuccessCheckmarkControl()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);
            DoubleBuffered = true;
            Size = new Size(120, 120);
            BackColor = CardPanel.RegisterCardFill;

            _animationTimer = new Timer { Interval = 16 };
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        public void PlayPopAnimation()
        {
            _progress = 0f;
            _animationTimer.Start();
            Invalidate();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!DesignMode)
            {
                PlayPopAnimation();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            float scale = EaseOutBack(_progress);
            float alpha = Math.Min(1f, _progress * 1.2f);
            int alphaByte = (int)(255 * alpha);

            var center = new PointF(Width / 2f, Height / 2f);
            float radius = Math.Min(Width, Height) * 0.38f * scale;

            g.TranslateTransform(center.X, center.Y);
            g.ScaleTransform(scale, scale);
            g.TranslateTransform(-center.X, -center.Y);

            if (radius > 2f)
            {
                RectangleF shadowRect = new RectangleF(
                    center.X - radius,
                    center.Y - radius + 3f,
                    radius * 2f,
                    radius * 2f);
                using (var shadowBrush = new SolidBrush(Color.FromArgb((int)(60 * alpha), CircleShadow)))
                {
                    g.FillEllipse(shadowBrush, shadowRect);
                }

                RectangleF circleRect = new RectangleF(
                    center.X - radius,
                    center.Y - radius,
                    radius * 2f,
                    radius * 2f);
                using (var fillBrush = new SolidBrush(Color.FromArgb(alphaByte, CircleFill)))
                {
                    g.FillEllipse(fillBrush, circleRect);
                }

                using (var pen = new Pen(Color.FromArgb(alphaByte, Color.White), Math.Max(3f, radius * 0.14f)))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    pen.LineJoin = LineJoin.Round;
                    float check = radius * 0.95f;
                    g.DrawLines(
                        pen,
                        new[]
                        {
                            new PointF(center.X - check * 0.42f, center.Y + check * 0.02f),
                            new PointF(center.X - check * 0.08f, center.Y + check * 0.36f),
                            new PointF(center.X + check * 0.46f, center.Y - check * 0.34f)
                        });
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            _progress += 0.07f;
            if (_progress >= 1f)
            {
                _progress = 1f;
                _animationTimer.Stop();
            }

            Invalidate();
        }

        private static float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            float x = t - 1f;
            return 1f + c3 * x * x * x + c1 * x * x;
        }
    }
}
