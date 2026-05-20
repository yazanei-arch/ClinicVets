using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace ClinicVets
{
    internal static class EmailService
    {
        private sealed class SmtpSettings
        {
            internal string Host { get; set; }
            internal int Port { get; set; }
            internal string User { get; set; }
            internal string Password { get; set; }
            internal string FromName { get; set; }
            internal bool EnableSsl { get; set; }
        }

        internal static bool TrySendVerificationCodeEmail(string toEmail, string verificationCode, out string errorMessage)
        {
            errorMessage = null;
            toEmail = (toEmail ?? string.Empty).Trim();
            verificationCode = (verificationCode ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(toEmail))
            {
                errorMessage = "Email is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(verificationCode))
            {
                errorMessage = "Verification code is missing.";
                return false;
            }

            if (!TryReadSmtpSettings(out SmtpSettings settings, out errorMessage))
            {
                return false;
            }

            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(settings.User, settings.FromName);
                    message.To.Add(toEmail);
                    message.Subject = "ClinicVets - Password Verification Code";
                    message.Body =
                        "Hello," + Environment.NewLine + Environment.NewLine
                        + "Your ClinicVets password reset verification code is: " + verificationCode + Environment.NewLine + Environment.NewLine
                        + "If you did not request this code, you can ignore this email." + Environment.NewLine + Environment.NewLine
                        + "ClinicVets";
                    message.IsBodyHtml = false;

                    using (var client = new SmtpClient(settings.Host, settings.Port))
                    {
                        client.EnableSsl = settings.EnableSsl;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(settings.User, settings.Password);
                        client.Send(message);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage =
                    "Could not send the verification email. Check SMTP settings in App.config and try again."
                    + Environment.NewLine + Environment.NewLine
                    + ex.Message;
                return false;
            }
        }

        private static bool TryReadSmtpSettings(out SmtpSettings settings, out string errorMessage)
        {
            settings = null;
            errorMessage = null;

            string host = ConfigurationManager.AppSettings["SmtpHost"];
            string portText = ConfigurationManager.AppSettings["SmtpPort"];
            string user = ConfigurationManager.AppSettings["SmtpUser"];
            string password = ConfigurationManager.AppSettings["SmtpPass"];
            string fromName = ConfigurationManager.AppSettings["SmtpFromName"];
            string enableSslText = ConfigurationManager.AppSettings["SmtpEnableSsl"];

            if (string.IsNullOrWhiteSpace(host))
            {
                errorMessage =
                    "SMTP is not configured. Add SmtpHost, SmtpPort, SmtpUser, and SmtpPass to App.config.";
                return false;
            }

            if (!int.TryParse(portText, out int port) || port <= 0)
            {
                errorMessage = "SMTP port is missing or invalid in App.config (SmtpPort).";
                return false;
            }

            if (IsPlaceholder(user))
            {
                errorMessage =
                    "SMTP username is not configured. Set SmtpUser in App.config to your sender email address.";
                return false;
            }

            if (IsPlaceholder(password))
            {
                errorMessage =
                    "SMTP password is not configured. Set SmtpPass in App.config to your email app password.";
                return false;
            }

            bool enableSsl = true;
            if (!string.IsNullOrWhiteSpace(enableSslText)
                && bool.TryParse(enableSslText, out bool parsedSsl))
            {
                enableSsl = parsedSsl;
            }

            settings = new SmtpSettings
            {
                Host = host.Trim(),
                Port = port,
                User = user.Trim(),
                Password = password,
                FromName = string.IsNullOrWhiteSpace(fromName) ? "ClinicVets" : fromName.Trim(),
                EnableSsl = enableSsl
            };
            return true;
        }

        private static bool IsPlaceholder(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            string trimmed = value.Trim();
            return trimmed.StartsWith("YOUR_", StringComparison.OrdinalIgnoreCase)
                || trimmed.Contains("YOUR_EMAIL")
                || trimmed.Equals("CHANGE_ME", StringComparison.OrdinalIgnoreCase);
        }
    }
}
