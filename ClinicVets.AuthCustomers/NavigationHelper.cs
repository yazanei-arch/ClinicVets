using ClinicVets;

namespace ClinicVets.AuthCustomers
{
    /// <summary>Delegates to ClinicVets.NavigationHelper (same entry flow).</summary>
    public static class NavigationHelper
    {
        public static void RegisterFlows()
        {
            ClinicVets.NavigationHelper.RegisterFlows();
        }
    }
}
