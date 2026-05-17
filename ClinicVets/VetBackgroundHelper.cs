using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ClinicVets
{
    internal static class VetBackgroundHelper
    {
        private static Image _cachedHeroImage;

        internal static void ApplyVetBackground(Form form)
        {
            ApplyPremiumBackground(form);
        }

        internal static void ApplySoftFallbackBackground(Form form)
        {
            ApplyPremiumBackground(form);
        }

        internal static void ApplyPremiumBackground(Form form)
        {
            if (form == null)
            {
                return;
            }

            ClearBackgroundImage(form);
            form.BackColor = ClinicUiTheme.BgTop;
            form.Invalidate(true);
        }

        internal static void ClearBackgroundImage(Form form)
        {
            if (form == null)
            {
                return;
            }

            Image previous = form.BackgroundImage;
            form.BackgroundImage = null;
            form.BackgroundImageLayout = ImageLayout.None;
            previous?.Dispose();
        }

        internal static Image GetLoginHeroImage()
        {
            if (_cachedHeroImage != null)
            {
                return _cachedHeroImage;
            }

            string path = FindVetBackgroundImagePath();
            if (path == null)
            {
                return null;
            }

            try
            {
                _cachedHeroImage = Image.FromFile(path);
            }
            catch
            {
                _cachedHeroImage = null;
            }

            return _cachedHeroImage;
        }

        private static string FindVetBackgroundImagePath()
        {
            string relative = Path.Combine("images", "vet_background.png");
            var tried = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string root in GetBackgroundImageSearchRoots())
            {
                if (string.IsNullOrWhiteSpace(root))
                {
                    continue;
                }

                string candidate = Path.GetFullPath(Path.Combine(root, relative));
                if (tried.Add(candidate) && File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static IEnumerable<string> GetBackgroundImageSearchRoots()
        {
            yield return Application.StartupPath;
            yield return AppDomain.CurrentDomain.BaseDirectory;

            string location = Assembly.GetExecutingAssembly().Location;
            if (!string.IsNullOrEmpty(location))
            {
                string dir = Path.GetDirectoryName(location);
                if (!string.IsNullOrEmpty(dir))
                {
                    yield return dir;
                }
            }

            foreach (string start in new[] { Application.StartupPath, AppDomain.CurrentDomain.BaseDirectory })
            {
                if (string.IsNullOrEmpty(start))
                {
                    continue;
                }

                yield return Path.GetFullPath(Path.Combine(start, "..", ".."));
                yield return Path.GetFullPath(Path.Combine(start, "..", "..", ".."));
            }

            yield return Environment.CurrentDirectory;
        }
    }
}
