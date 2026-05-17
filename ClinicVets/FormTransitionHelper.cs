using System;
using System.Windows.Forms;

namespace ClinicVets
{
    internal static class FormTransitionHelper
    {
        private const int FadeDurationMs = 300;
        private const int FadeIntervalMs = 16;

        internal static void FadeIn(Form form)
        {
            if (form == null || form.IsDisposed)
            {
                return;
            }

            form.Opacity = 0d;
            var timer = new Timer { Interval = FadeIntervalMs };
            int elapsed = 0;

            timer.Tick += (sender, args) =>
            {
                if (form.IsDisposed)
                {
                    timer.Stop();
                    timer.Dispose();
                    return;
                }

                elapsed += FadeIntervalMs;
                double progress = Math.Min(1d, elapsed / (double)FadeDurationMs);
                form.Opacity = progress;

                if (progress >= 1d)
                {
                    form.Opacity = 1d;
                    timer.Stop();
                    timer.Dispose();
                }
            };

            timer.Start();
        }
    }
}
