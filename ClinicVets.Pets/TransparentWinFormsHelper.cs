using System;
using System.Windows.Forms;

namespace ClinicVets.UI
{
    /// <summary>Prevents black erase rectangles on controls over a PictureBox background.</summary>
    internal static class TransparentWinFormsHelper
    {
        private const int WM_ERASEBKGND = 0x0014;

        public static bool TrySuppressEraseBackground(ref Message m)
        {
            if (m.Msg != WM_ERASEBKGND)
            {
                return false;
            }

            m.Result = IntPtr.Zero;
            return true;
        }
    }
}
