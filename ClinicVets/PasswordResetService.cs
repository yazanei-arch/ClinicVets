using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClinicVets
{
    internal static class PasswordResetService
    {
        private static readonly ExcelHelper Excel = new ExcelHelper();
        private static readonly object Sync = new object();
        private static readonly Dictionary<string, string> PendingCodes =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly Random CodeRandom = new Random();

        internal static bool TrySendVerificationCode(string email, out string verificationCode, out string errorMessage)
        {
            verificationCode = null;
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

            verificationCode = GenerateSixDigitCode();

            lock (Sync)
            {
                PendingCodes[email] = verificationCode;
            }

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

            if (!EmployeeExistsByEmail(email))
            {
                errorMessage = "Email not found. Please enter the registered email.";
                return false;
            }

            errorMessage = ValidationHelper.ValidateVerificationCode(verificationCode);
            if (errorMessage != null)
            {
                return false;
            }

            errorMessage = ValidationHelper.ValidateResetPassword(newPassword);
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

            bool employeeInExcel = false;
            try
            {
                employeeInExcel = Excel.TryFindEmployeeByEmail(email) != null;
            }
            catch (Exception ex)
            {
                errorMessage = BuildExcelErrorMessage(ex);
                return false;
            }

            EmployeeCredentialStore.UpdatePasswordByEmail(email, newPassword);

            if (employeeInExcel)
            {
                try
                {
                    if (!File.Exists(ExcelFileManager.FilePath))
                    {
                        errorMessage = ExcelFileManager.WorkbookNotFoundMessage;
                        return false;
                    }

                    if (!Excel.UpdateEmployeePasswordByEmail(email, newPassword))
                    {
                        errorMessage = "Could not update the password in the Employees sheet.";
                        return false;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    errorMessage = ex.Message;
                    return false;
                }
                catch (Exception ex)
                {
                    errorMessage = BuildExcelErrorMessage(ex);
                    return false;
                }
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
            lock (Sync)
            {
                return CodeRandom.Next(100000, 1000000).ToString();
            }
        }

        private static string BuildExcelErrorMessage(Exception ex)
        {
            if (ExcelHelper.IsWorkbookLockedException(ex))
            {
                return ExcelFileManager.WorkbookLockedMessage;
            }

            return "Could not access the Excel file."
                + Environment.NewLine + Environment.NewLine
                + (ex.Message ?? "Unknown error.");
        }
    }
}
