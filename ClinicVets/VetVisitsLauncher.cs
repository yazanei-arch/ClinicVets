using System;
using System.Windows.Forms;
using ClinicVets.VisitsMedicines;

namespace ClinicVets
{
    /// <summary>Opens visit and medicine screens inside the running ClinicVets application.</summary>
    public static class VetVisitsLauncher
    {
        public static bool TryLaunchVisitsApplication(IWin32Window owner, string petId = null)
        {
            if (!RolePermissions.CanAccessVisits())
            {
                return false;
            }

            try
            {
                VisitManagementForm form = string.IsNullOrWhiteSpace(petId)
                    ? new VisitManagementForm()
                    : new VisitManagementForm(petId.Trim());
                ShowForm(owner, form);
                return true;
            }
            catch (Exception ex)
            {
                ShowOpenError(owner, ex, "Visit Management could not open.");
                return false;
            }
        }

        public static bool TryLaunchMedicinesApplication(IWin32Window owner)
        {
            if (!RolePermissions.CanAccessMedicines())
            {
                return false;
            }

            try
            {
                var form = new MedicinesForm();
                ShowForm(owner, form);
                return true;
            }
            catch (Exception ex)
            {
                ShowOpenError(owner, ex, "Medicine Inventory could not open.");
                return false;
            }
        }

        private static void ShowForm(IWin32Window owner, Form form)
        {
            if (owner is Form ownerForm)
            {
                form.Owner = ownerForm;
            }

            form.StartPosition = FormStartPosition.CenterScreen;
            form.Show();
        }

        private static void ShowOpenError(IWin32Window owner, Exception ex, string title)
        {
            MessageBox.Show(
                owner,
                title + Environment.NewLine + Environment.NewLine + (ex?.Message ?? "Unknown error."),
                "ClinicVets",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}
