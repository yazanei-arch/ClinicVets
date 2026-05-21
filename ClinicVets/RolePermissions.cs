using System;

namespace ClinicVets
{
    /// <summary>Role-based access for navigation and forms.</summary>
    public static class RolePermissions
    {
        public static bool IsSecretary(string role)
        {
            return string.Equals(role, "Secretary", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsVeterinarian(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return false;
            }

            return role.Equals("Vet", StringComparison.OrdinalIgnoreCase) ||
                   role.Equals("Veterinarian", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsSecretary()
        {
            return IsSecretary(SessionManager.CurrentRole);
        }

        public static bool IsVeterinarian()
        {
            return IsVeterinarian(SessionManager.CurrentRole);
        }

        public static bool CanAccessCustomers()
        {
            return IsSecretary();
        }

        public static bool CanAccessPets()
        {
            return IsSecretary() || IsVeterinarian();
        }

        public static bool CanAccessVisits()
        {
            return IsVeterinarian();
        }

        public static bool CanAccessMedicines()
        {
            return IsVeterinarian();
        }
    }
}
