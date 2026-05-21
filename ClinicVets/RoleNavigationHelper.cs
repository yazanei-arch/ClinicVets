using System.Windows.Forms;

namespace ClinicVets
{
    /// <summary>Role routing after login only (not registration success).</summary>
    internal static class RoleNavigationHelper
    {
        internal static bool TryNavigateAfterLogin(Employee employee, Form1 loginForm)
        {
            return NavigationHelper.TryNavigateAfterLogin(employee, loginForm);
        }
    }
}
