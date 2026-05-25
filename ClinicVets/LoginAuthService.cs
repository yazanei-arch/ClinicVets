using System;
using System.Linq;

namespace ClinicVets
{
    internal static class LoginAuthService
    {
        private static readonly ExcelHelper Excel = new ExcelHelper();

        internal static Employee Authenticate(string username, string password)
        {
            username = (username ?? string.Empty).Trim();
            password = password ?? string.Empty;
            if (username.Length == 0 || password.Length == 0)
            {
                return null;
            }

            Employee fromExcel = null;
            try
            {
                fromExcel = Excel.TryAuthenticateEmployee(username, password);
            }
            catch
            {
                fromExcel = null;
            }

            if (fromExcel != null)
            {
                return fromExcel;
            }

            return EmployeeCredentialStore.GetInMemoryEmployees().FirstOrDefault(employee =>
                string.Equals(employee.Username?.Trim(), username, StringComparison.OrdinalIgnoreCase)
                && string.Equals(employee.Password ?? string.Empty, password, StringComparison.Ordinal));
        }

        internal static void RegisterEmployee(Employee employee)
        {
            if (employee == null)
            {
                return;
            }

            string username = (employee.Username ?? string.Empty).Trim();
            string email = (employee.Email ?? string.Empty).Trim();
            string employeeNumber = (employee.EmployeeID ?? string.Empty).Trim();
            string nationalId = (employee.NationalID ?? string.Empty).Trim();

            if (username.Length == 0)
            {
                throw new InvalidOperationException("Username is required.");
            }

            if (email.Length == 0)
            {
                throw new InvalidOperationException("Email is required.");
            }

            if (employeeNumber.Length == 0)
            {
                throw new InvalidOperationException("Employee number is required.");
            }

            EnsureEmployeeIsUnique(username, email, employeeNumber, nationalId);

            Excel.AppendEmployee(employee);
            EmployeeCredentialStore.Add(employee);
        }

        private static void EnsureEmployeeIsUnique(string username, string email, string employeeNumber, string nationalId)
        {
            foreach (Employee existing in Excel.ReadEmployees())
            {
                ThrowIfDuplicate(existing, username, email, employeeNumber, nationalId);
            }

            foreach (Employee existing in EmployeeCredentialStore.GetInMemoryEmployees())
            {
                ThrowIfDuplicate(existing, username, email, employeeNumber, nationalId);
            }
        }

        private static void ThrowIfDuplicate(Employee existing, string username, string email, string employeeNumber, string nationalId)
        {
            if (existing == null)
            {
                return;
            }

            if (string.Equals(existing.Username?.Trim(), username, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Username already exists.");
            }

            if (string.Equals(existing.Email?.Trim(), email, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("An employee with this email already exists.");
            }

            if (string.Equals(existing.EmployeeID?.Trim(), employeeNumber, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("An employee with this ID already exists.");
            }

            if (nationalId.Length > 0
                && string.Equals(existing.NationalID?.Trim(), nationalId, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("An employee with this ID already exists.");
            }
        }
    }
}
