using System;
using System.IO;
using System.Windows.Forms;
using ClinicVets;

namespace ClinicVets.UI
{
    /// <summary>Safe Excel workbook checks for pet forms — no loading during constructors.</summary>
    internal static class PetExcelSupport
    {
        internal static string GetWorkbookPath()
        {
            try
            {
                return ExcelFileManager.GetWorkbookFullPath();
            }
            catch
            {
                return ExcelFileManager.FilePath;
            }
        }

        internal static bool TryEnsureWorkbookReady(IWin32Window owner, out string workbookPath)
        {
            workbookPath = GetWorkbookPath();

            try
            {
                if (string.IsNullOrWhiteSpace(workbookPath) || !File.Exists(workbookPath))
                {
                    MessageBox.Show(
                        owner,
                        ExcelFileManager.WorkbookNotFoundMessage + Environment.NewLine + Environment.NewLine
                        + "Expected file:" + Environment.NewLine + workbookPath,
                        "ClinicVets — Data file",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    owner,
                    "Could not access the clinic data file." + Environment.NewLine + Environment.NewLine + ex.Message,
                    "ClinicVets — Data file",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }
        }
    }
}
