using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>Launches ClinicVets.VisitsMedicines as a separate process.</summary>
    public static class VetVisitsLauncher
    {
        public const string BuildFirstMessage = "Please build ClinicVets.VisitsMedicines first.";

        public static bool TryLaunchVisitsApplication(IWin32Window owner, string petId = null)
        {
            string exePath = FindVisitsMedicinesExecutable();
            if (string.IsNullOrEmpty(exePath))
            {
                MessageBox.Show(
                    owner,
                    BuildFirstMessage,
                    "ClinicVets",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                string arguments = string.IsNullOrWhiteSpace(petId) ? string.Empty : petId.Trim();
                var startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(exePath) ?? string.Empty
                };
                Process.Start(startInfo);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    owner,
                    BuildFirstMessage + Environment.NewLine + Environment.NewLine + ex.Message,
                    "ClinicVets",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }
        }

        internal static string FindVisitsMedicinesExecutable()
        {
            const string exeName = "ClinicVets.VisitsMedicines.exe";
            var tried = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string root in GetSearchRoots())
            {
                if (string.IsNullOrWhiteSpace(root))
                {
                    continue;
                }

                string[] relativeFolders =
                {
                    Path.Combine("ClinicVets.VisitsMedicines", "bin", "Debug", "net5.0-windows", exeName),
                    Path.Combine("ClinicVets.VisitsMedicines", "bin", "Debug", "net472", exeName),
                    Path.Combine("ClinicVets.VisitsMedicines", "bin", "Release", "net5.0-windows", exeName),
                    Path.Combine("ClinicVets.VisitsMedicines", "bin", "Release", "net472", exeName)
                };

                foreach (string relative in relativeFolders)
                {
                    string candidate = Path.GetFullPath(Path.Combine(root, relative));
                    if (tried.Add(candidate) && File.Exists(candidate))
                    {
                        return candidate;
                    }
                }
            }

            return null;
        }

        private static System.Collections.Generic.IEnumerable<string> GetSearchRoots()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);

            yield return baseDir;
            yield return Path.GetFullPath(Path.Combine(baseDir, ".."));
            yield return Path.GetFullPath(Path.Combine(baseDir, "..", ".."));
            yield return Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
            yield return Environment.CurrentDirectory;

            string location = typeof(VetVisitsLauncher).Assembly.Location;
            if (!string.IsNullOrEmpty(location))
            {
                string dir = Path.GetDirectoryName(location);
                if (!string.IsNullOrEmpty(dir))
                {
                    yield return dir;
                }
            }
        }
    }
}
