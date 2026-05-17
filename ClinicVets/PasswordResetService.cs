using System;
using System.Collections.Generic;
using System.Linq;

namespace ClinicVets
{
    internal static class PasswordResetService
    {
        private static readonly ExcelHelper Excel = new ExcelHelper();
        private static readonly object Sync = new object();
        private static readonly Dictionary<string, string> PendingCodes =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        internal static bool TrySendVerificationCode(string email, out string codeForTesting, out string errorMessage)
        {
            codeForTesting = null;
            email = (email ?? string.Empty).Trim();

            errorMessage = ValidationHelper.ValidateEmail(email);
            if (errorMessage != null)
            {
                return false;
            }

            if (!EmployeeExistsByEmail(email))
            {
                errorMessage = "Email not found. Please enter the registered email.";
                return false;
            }

            string code = GenerateSixDigitCode();
            lock (Sync)
            {
                PendingCodes[email] = code;
            }

            codeForTesting = code;
            errorMessage = null;
            return true;
        }

        internal static bool TryResetPassword(
            string email,
            string verificationCode,
            string newPassword,
            string confirmPassword,
            out string errorMessage)
        {
            email = (email ?? string.Empty).Trim();
            verificationCode = (verificationCode ?? string.Empty).Trim();
            newPassword = newPassword ?? string.Empty;
            confirmPassword = confirmPassword ?? string.Empty;

            errorMessage = ValidationHelper.ValidateEmail(email);
            if (errorMessage != null)
            {
                return false;
            }

            errorMessage = ValidationHelper.ValidateVerificationCode(verificationCode);
            if (errorMessage != null)
            {
                return false;
            }

            errorMessage = ValidationHelper.ValidatePassword(newPassword);
            if (errorMessage != null)
            {
                return false;
            }

            errorMessage = ValidationHelper.ValidateConfirmPassword(newPassword, confirmPassword);
            if (errorMessage != null)
            {
                return false;
            }

            lock (Sync)
            {
                if (!PendingCodes.TryGetValue(email, out string expectedCode)
                    || !string.Equals(expectedCode, verificationCode, StringComparison.Ordinal))
                {
                    errorMessage = "Verification code is incorrect.";
                    return false;
                }
            }

            EmployeeCredentialStore.UpdatePasswordByEmail(email, newPassword);
            try
            {
                Excel.UpdateEmployeePasswordByEmail(email, newPassword);
            }
            catch
            {
            }

            lock (Sync)
            {
                PendingCodes.Remove(email);
            }

            errorMessage = null;
            return true;
        }

        private static bool EmployeeExistsByEmail(string email)
        {
            try
            {
                if (Excel.TryFindEmployeeByEmail(email) != null)
                {
                    return true;
                }
            }
            catch
            {
            }

            return EmployeeCredentialStore.GetInMemoryEmployees().Any(employee =>
                string.Equals(employee.Email?.Trim(), email, StringComparison.OrdinalIgnoreCase));
        }

        private static string GenerateSixDigitCode()
        {
            int value = new Random().Next(100000, 1000000);
            return value.ToString();
        }
    }
}
