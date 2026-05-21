using System;
using System.Windows.Forms;
using ClinicVets.UI;

namespace ClinicVets
{
    /// <summary>Central navigation for login, secretary, pets, and vet flows.</summary>
    public static class NavigationHelper
    {
        public static void RegisterFlows()
        {
            PetFlowBootstrap.Register();
        }

        public static bool TryNavigateAfterLogin(Employee employee, Form1 loginForm)
        {
            if (employee == null)
            {
                return false;
            }

            string role = (employee.Role ?? string.Empty).Trim();
            if (role.Equals("Secretary", StringComparison.OrdinalIgnoreCase))
            {
                SessionManager.CurrentUser = employee;
                using (var menu = new SecretaryMenuForm(loginForm))
                {
                    menu.ShowDialog(loginForm);
                }

                return true;
            }

            if (IsVetRole(role))
            {
                SessionManager.CurrentUser = employee;
                loginForm.Hide();
                try
                {
                    OpenVetPetManagement(loginForm);
                }
                finally
                {
                    if (!loginForm.IsDisposed)
                    {
                        loginForm.Show();
                    }
                }

                return true;
            }

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
            using (var form = new PetManagementForm(ownerId, vetWorkflow: false))
            {
                form.ShowDialog(owner);
            }
        }

        public static void OpenVetPetManagement(IWin32Window owner)
        {
            using (var form = new PetManagementForm(ownerId: null, vetWorkflow: true))
            {
                form.ShowDialog(owner);
            }
        }

        private static bool IsVetRole(string role)
        {
            return role.Equals("Vet", StringComparison.OrdinalIgnoreCase) ||
                   role.Equals("Veterinarian", StringComparison.OrdinalIgnoreCase);
        }
    }
}
