using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ClinicVets.UI
{
    /// <summary>Loads pet screen backgrounds from disk — avoids .resx serialized images.</summary>
    internal static class PetBackgroundHelper
    {
        private const string BackgroundFileName = "background.png";

        private static Image _cachedBackground;
        private static readonly object _cachedBackgroundLock = new object();

        internal static void ApplyToPictureBox(PictureBox pictureBox)
        {
            if (pictureBox == null)
            {
                return;
            }

            DisposePictureBoxImage(pictureBox);

            try
            {
                Image source = GetCachedBackgroundImage();
                if (source == null)
                {
                    return;
                }

                pictureBox.Image = new Bitmap(source);
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            catch
            {
                pictureBox.Image = null;
            }
        }

        internal static void ApplyToForm(Form form)
        {
            if (form == null)
            {
                return;
            }

            try
            {
                Image source = GetCachedBackgroundImage();
                if (source == null)
                {
                    return;
                }

                form.BackgroundImageLayout = ImageLayout.Stretch;
                form.BackgroundImage = new Bitmap(source);
            }
            catch
            {
                form.BackgroundImage = null;
            }
        }

        private static Image GetCachedBackgroundImage()
        {
            if (_cachedBackground != null)
            {
                return _cachedBackground;
            }

            lock (_cachedBackgroundLock)
            {
                if (_cachedBackground != null)
                {
                    return _cachedBackground;
                }

                string path = FindBackgroundImagePath();
                if (path == null)
                {
                    return null;
                }

                try
                {
                    using (Image fromFile = Image.FromFile(path))
                    {
                        _cachedBackground = new Bitmap(fromFile);
                    }

                    return _cachedBackground;
                }
                catch
                {
                    _cachedBackground = null;
                    return null;
                }
            }
        }

        internal static void DisposePictureBoxImage(PictureBox pictureBox)
        {
            if (pictureBox?.Image == null)
            {
                return;
            }

            Image previous = pictureBox.Image;
            pictureBox.Image = null;
            previous.Dispose();
        }

        internal static void ReleaseFormBackground(Form form)
        {
            if (form?.BackgroundImage == null)
            {
                return;
            }

            Image previous = form.BackgroundImage;
            form.BackgroundImage = null;
            previous.Dispose();
        }

        internal static string FindBackgroundImagePath()
        {
            foreach (string root in GetSearchRoots())
            {
                if (string.IsNullOrWhiteSpace(root))
                {
                    continue;
                }

                string resourcesPath = Path.Combine(root, "Resources", BackgroundFileName);
                if (File.Exists(resourcesPath))
                {
                    return resourcesPath;
                }

                string imagesPath = Path.Combine(root, "images", BackgroundFileName);
                if (File.Exists(imagesPath))
                {
                    return imagesPath;
                }
            }

            return null;
        }

        private static System.Collections.Generic.IEnumerable<string> GetSearchRoots()
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

            yield return Environment.CurrentDirectory;
        }
    }
}
