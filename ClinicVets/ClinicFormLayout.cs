using System.Drawing;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>Shared WinForms size and placement for clinic screens.</summary>
    public static class ClinicFormLayout
    {
        public const int StandardWidth = 1100;
        public const int StandardHeight = 700;

        public static void ApplyStandard(Form form)
        {
            if (form == null)
            {
                return;
            }

            form.StartPosition = FormStartPosition.CenterScreen;
            form.ClientSize = new Size(StandardWidth, StandardHeight);
            form.MaximizeBox = false;
            if (form.FormBorderStyle == FormBorderStyle.Sizable)
            {
                form.FormBorderStyle = FormBorderStyle.FixedSingle;
            }
        }
    }
}
