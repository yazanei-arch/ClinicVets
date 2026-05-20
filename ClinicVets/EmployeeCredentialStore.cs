using System;
using System.Collections.Generic;
using System.Linq;

namespace ClinicVets
{
    /// <summary>
    /// In-memory employee credentials used when Excel is empty or unavailable.
    /// </summary>
    internal static class EmployeeCredentialStore
    {
        private static readonly List<Employee> InMemoryEmployees = new List<Employee>();
        private static readonly object Sync = new object();

        internal static void Add(Employee employee)
        {
            if (employee == null)
            {
                return;
            }

            lock (Sync)
            {
                InMemoryEmployees.RemoveAll(e =>
                    string.Equals(e.Username, employee.Username, StringComparison.OrdinalIgnoreCase));
                InMemoryEmployees.Add(employee);
            }
        }

        internal static IReadOnlyList<Employee> GetInMemoryEmployees()
        {
            lock (Sync)
            {
                return InMemoryEmployees.ToList();
            }
        }

        internal static void UpdatePasswordByEmail(string email, string newPassword)
        {
            email = (email ?? string.Empty).Trim();
            if (email.Length == 0)
            {
                return;
            }

            lock (Sync)
            {
                foreach (Employee employee in InMemoryEmployees)
                {
                    if (string.Equals(employee.Email?.Trim(), email, StringComparison.OrdinalIgnoreCase))
                    {
                        employee.Password = newPassword ?? string.Empty;
                    }
                }
            }
        }
    }
}

