using System;
using System.Windows.Forms;

namespace ClinicVets.UI
{
    /// <summary>Set by ClinicVets at startup for visits and pharmacy without circular references.</summary>
    public static class PetNavigationHooks
    {
        public static Action<IWin32Window, string> OpenVisitForPet { get; set; }
        public static Action<IWin32Window> OpenMedicinesPharmacy { get; set; }
    }
}
