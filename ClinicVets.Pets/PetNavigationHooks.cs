using System;
using System.Windows.Forms;

namespace ClinicVets.UI
{
    /// <summary>Set by ClinicVets at startup to open visits without circular project references.</summary>
    public static class PetNavigationHooks
    {
        public static Action<IWin32Window, string> OpenVisitForPet { get; set; }
    }
}
