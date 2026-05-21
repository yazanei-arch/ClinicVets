using System;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>Central navigation for login, secretary, pets, and vet flows.</summary>
    public static class NavigationHelper
    {
        public static void RegisterFlows()
        {
            PetFlowBootstrap.Register();
            PetAssemblyBootstrap.TryRegister();
        }

        public static bool TryNavigateAfterLogin(Employee employee, Form1 loginForm)
        {
            if (employee == null)
            {
                return false;
            }

            string role = (employee.Role ?? string.Empty).Trim();
            SessionManager.SetUser(employee);

            if (role.Equals("Secretary", StringComparison.OrdinalIgnoreCase))
            {
                using (var menu = new SecretaryMenuForm(loginForm))
                {
                    menu.ShowDialog(loginForm);
                }

                return true;
            }

            if (IsVetRole(role))
            {
                using (var menu = new VeterinarianMenuForm(loginForm))
                {
                    menu.ShowDialog(loginForm);
                }

                return true;
            }

            SessionManager.Clear();
            MessageBox.Show(
                loginForm,
                "Unknown employee role. Please contact an administrator.",
                "ClinicVets",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return false;
        }

        public static void OpenSecretaryPetManagement(IWin32Window owner, string ownerId)
        {
            if (!RolePermissions.CanAccessPets())
            {
                return;
            }

            if (PetFlowGateway.OpenPetManagementForOwner != null)
            {
                PetFlowGateway.OpenPetManagementForOwner(owner, ownerId);
                return;
            }

            ShowPetModuleUnavailable(owner);
        }

        public static void OpenVetPetManagement(IWin32Window owner)
        {
            if (!RolePermissions.CanAccessPets())
            {
                return;
            }

            if (PetFlowGateway.OpenVetPetManagement != null)
            {
                PetFlowGateway.OpenVetPetManagement(owner);
                return;
            }

            ShowPetModuleUnavailable(owner);
        }

        private static void ShowPetModuleUnavailable(IWin32Window owner)
        {
            MessageBox.Show(
                owner,
                "Pet module is not available. Rebuild the solution and ensure ClinicVets.Pets is deployed.",
                "ClinicVets",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private static bool IsVetRole(string role)
        {
            return RolePermissions.IsVeterinarian(role);
        }
    }
}
