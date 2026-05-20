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

            Excel.AppendEmployee(employee);
            EmployeeCredentialStore.Add(employee);
        }
    }
}
