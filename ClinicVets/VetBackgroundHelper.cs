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
        private static Image _cachedLoginBackground;
        private static Image _cachedRegisterBackground;

        private static readonly Dictionary<string, Image> _imageCacheByFile =
            new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
        private static readonly object _imageCacheLock = new object();

        /// <summary>
        /// Returns a shared <see cref="Image"/> for the given file name (from any standard
        /// background-search root). The image is read from disk only once per app run;
        /// callers must clone before assigning to a Form/PictureBox they will dispose.
        /// </summary>
        internal static Image GetCachedImage(string imageFileName)
        {
            if (string.IsNullOrWhiteSpace(imageFileName))
            {
                return null;
            }

            lock (_imageCacheLock)
            {
                if (_imageCacheByFile.TryGetValue(imageFileName, out Image cached))
                {
                    return cached;
                }

                string path = FindImagePath(imageFileName);
                if (path == null)
                {
                    return null;
                }

                try
                {
                    using (Image fromFile = Image.FromFile(path))
                    {
                        Image inMemory = new Bitmap(fromFile);
                        _imageCacheByFile[imageFileName] = inMemory;
                        return inMemory;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

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

        internal static void ApplyLoginBackground(Form form)
        {
            if (form == null)
            {
                return;
            }

            ClearBackgroundImage(form);
            Image source = GetLoginBackgroundImage();
            if (source == null)
            {
                form.BackColor = Color.FromArgb(232, 244, 252);
                form.Invalidate(true);
                return;
            }

            form.BackgroundImageLayout = ImageLayout.Zoom;
            form.BackgroundImage = (Image)source.Clone();
            form.BackColor = Color.FromArgb(232, 244, 252);
            form.Invalidate(true);
        }

        internal static void ApplyRegisterBackground(Form form)
        {
            if (form == null)
            {
                return;
            }

            ClearBackgroundImage(form);
            Image source = GetRegisterBackgroundImage();
            if (source == null)
            {
                form.BackColor = Color.FromArgb(232, 244, 252);
                form.Invalidate(true);
                return;
            }

            form.BackgroundImageLayout = ImageLayout.Stretch;
            form.BackgroundImage = (Image)source.Clone();
            form.BackColor = Color.FromArgb(232, 244, 252);
            form.Invalidate(true);
        }

        internal static Image GetRegisterBackgroundImage()
        {
            if (_cachedRegisterBackground != null)
            {
                return _cachedRegisterBackground;
            }

            string path = FindRegisterBackgroundImagePath();
            if (path == null)
            {
                return null;
            }

            try
            {
                _cachedRegisterBackground = Image.FromFile(path);
            }
            catch
            {
                _cachedRegisterBackground = null;
            }

            return _cachedRegisterBackground;
        }

        internal static Image GetLoginBackgroundImage()
        {
            if (_cachedLoginBackground != null)
            {
                return _cachedLoginBackground;
            }

            string path = FindLoginBackgroundImagePath();
            if (path == null)
            {
                return null;
            }

            try
            {
                _cachedLoginBackground = Image.FromFile(path);
            }
            catch
            {
                _cachedLoginBackground = null;
            }

            return _cachedLoginBackground;
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

        private static string FindRegisterBackgroundImagePath()
        {
            return FindImagePath("register-bg.png");
        }

        private static string FindLoginBackgroundImagePath()
        {
            return FindImagePath("login_background.png");
        }

        private static string FindVetBackgroundImagePath()
        {
            return FindImagePath("vet_background.png");
        }

        private static string FindImagePath(string fileName)
        {
            string relative = Path.Combine("images", fileName);
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
