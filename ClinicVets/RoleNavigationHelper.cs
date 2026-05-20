using System;
using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>Role routing after login only (not registration success).</summary>
    internal static class RoleNavigationHelper
    {
        private const string VetRoleMessage = "Veterinarian pages are handled by another team.";

        internal static bool TryNavigateAfterLogin(Employee employee, Form1 loginForm)
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

            if (role.Equals("Vet", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    loginForm,
                    VetRoleMessage,
                    "ClinicVets",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return false;
            }

            MessageBox.Show(
                loginForm,
                "Unknown employee role. Please contact an administrator.",
                "ClinicVets",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return false;
        }
    }
}
