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

        internal static void ApplyToPictureBox(PictureBox pictureBox)
        {
            if (pictureBox == null)
            {
                return;
            }

            DisposePictureBoxImage(pictureBox);

            try
            {
                string path = FindBackgroundImagePath();
                if (path == null)
                {
                    return;
                }

                using (var loaded = Image.FromFile(path))
                {
                    pictureBox.Image = new Bitmap(loaded);
                }

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
                string path = FindBackgroundImagePath();
                if (path == null)
                {
                    return;
                }

                form.BackgroundImageLayout = ImageLayout.Stretch;
                using (var loaded = Image.FromFile(path))
                {
                    form.BackgroundImage = new Bitmap(loaded);
                }
            }
            catch
            {
                form.BackgroundImage = null;
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
