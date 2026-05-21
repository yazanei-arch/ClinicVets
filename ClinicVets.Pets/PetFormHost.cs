using System;
using System.Reflection;
using System.Windows.Forms;

namespace ClinicVets.UI
{
    /// <summary>Opens pet forms safely without crashing the host application.</summary>
    public static class PetFormHost
    {
        public static void ShowPetManagement(IWin32Window owner, string ownerId, bool vetWorkflow)
        {
            try
            {
                using (var form = new PetManagementForm(ownerId, vetWorkflow))
                {
                    form.ShowDialog(owner);
                }
            }
            catch (TargetInvocationException ex)
            {
                ShowFriendlyError(owner, ex.InnerException ?? ex, "Pet Management could not open.");
            }
            catch (Exception ex)
            {
                ShowFriendlyError(owner, ex, "Pet Management could not open.");
            }
        }

        public static void ShowAddPet(IWin32Window owner, string ownerId)
        {
            try
            {
                using (var form = new AddPetForm(ownerId))
                {
                    form.ShowDialog(owner);
                }
            }
            catch (Exception ex)
            {
                ShowFriendlyError(owner, Unwrap(ex), "Add Pet could not open.");
            }
        }

        public static void ShowSearchPet(IWin32Window owner, bool vetWorkflow)
        {
            try
            {
                using (var form = new SearchPetForm(vetWorkflow))
                {
                    form.ShowDialog(owner);
                }
            }
            catch (Exception ex)
            {
                ShowFriendlyError(owner, Unwrap(ex), "Search Pet could not open.");
            }
        }

        public static void ShowAllPets(IWin32Window owner)
        {
            try
            {
                using (var form = new AllPetsForm())
                {
                    form.ShowDialog(owner);
                }
            }
            catch (Exception ex)
            {
                ShowFriendlyError(owner, Unwrap(ex), "View All Pets could not open.");
            }
        }

        public static void ShowAnimalTypes(IWin32Window owner)
        {
            try
            {
                using (var form = new AnimalTypesForm())
                {
                    form.ShowDialog(owner);
                }
            }
            catch (Exception ex)
            {
                ShowFriendlyError(owner, Unwrap(ex), "Animal Types could not open.");
            }
        }

        private static Exception Unwrap(Exception ex)
        {
            if (ex is TargetInvocationException tie && tie.InnerException != null)
            {
                return tie.InnerException;
            }

            return ex;
        }

        private static void ShowFriendlyError(IWin32Window owner, Exception ex, string title)
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
