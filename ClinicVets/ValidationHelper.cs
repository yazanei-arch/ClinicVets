using System;
using System.Linq;

namespace ClinicVets
{
  public static class ValidationHelper
  {
    public const string RequiredMessage = "This field is required.";

    private static readonly string[] ValidPhonePrefixes = { "050", "052", "053", "054", "055", "058" };

    public static string ValidateRequired(string value)
    {
      return string.IsNullOrWhiteSpace(value) ? RequiredMessage : null;
    }

    public static string ValidateIdNumber(string value)
    {
      string required = ValidateRequired(value);
      if (required != null)
      {
        return required;
      }

      value = value.Trim();
      if (value.Length != 9)
      {
        return "ID must be exactly 9 digits.";
      }

      foreach (char c in value)
      {
        if (!char.IsDigit(c))
        {
          return "ID must contain numbers only.";
        }
      }

      return null;
    }

    public static string ValidatePhone(string value)
    {
      string required = ValidateRequired(value);
      if (required != null)
      {
        return required;
      }

      value = value.Trim();
      if (value.Length != 10)
      {
        return "Phone must be exactly 10 digits.";
      }

      foreach (char c in value)
      {
        if (!char.IsDigit(c))
        {
          return "Phone must contain numbers only.";
        }
      }

      bool validPrefix = false;
      foreach (string prefix in ValidPhonePrefixes)
      {
        if (value.StartsWith(prefix, StringComparison.Ordinal))
        {
          validPrefix = true;
          break;
        }
      }

      if (!validPrefix)
      {
        return "Phone must start with 050, 052, 053, 054, 055, or 058.";
      }

      return null;
    }

    public static string ValidateEmail(string value)
    {
      string required = ValidateRequired(value);
      if (required != null)
      {
        return required;
      }

      value = value.Trim();
      int atCount = value.Count(c => c == '@');
      if (atCount != 1)
      {
        return "Email must contain exactly one '@' character.";
      }

      string[] parts = value.Split('@');
      if (parts[0].Length == 0 || parts[1].Length == 0)
      {
        return "Email must include text before and after '@'.";
      }

      string domain = parts[1];
      if (!domain.Contains("."))
      {
        return "Email domain must contain a dot.";
      }

      string tld = domain.Substring(domain.LastIndexOf('.') + 1);
      if (tld.Length < 2 || !tld.All(IsEnglishLetter))
      {
        return "Email domain must end with at least 2 letters.";
      }

      return null;
    }

        public static string ValidateUsername(string value)
        {
            string required = ValidateRequired(value);
            if (required != null)
            {
                return required;
            }

            value = value.Trim();

            if (value.Length < 6 || value.Length > 8)
            {
                return "Username must be 6-8 characters.";
            }

            int digitCount = value.Count(char.IsDigit);

            if (digitCount > 2)
            {
                return "Username may contain up to 2 digits only.";
            }

            foreach (char c in value)
            {
                if (!IsEnglishLetter(c) && !char.IsDigit(c))
                {
                    return "Username may contain English letters and numbers only.";
                }
            }

            return null;
        }

        public static string ValidatePassword(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return RequiredMessage;
            }

            if (value.Length < 8 || value.Length > 10)
            {
                return "Password must be 8-10 characters.";
            }

            bool hasLetter = false;
            bool hasDigit = false;
            bool hasSpecial = false;

            foreach (char c in value)
            {
                if (IsEnglishLetter(c))
                {
                    hasLetter = true;
                }
                else if (char.IsDigit(c))
                {
                    hasDigit = true;
                }
                else if (c == '!' || c == '#' || c == '$')
                {
                    hasSpecial = true;
                }
            }

            if (!hasLetter)
            {
                return "Password must contain at least one letter.";
            }

            if (!hasDigit)
            {
                return "Password must contain at least one number.";
            }

            if (!hasSpecial)
            {
                return "Password must contain at least one special character (!,#,$).";
            }

            return null;
        }

        /// <summary>Password rules for forgot-password reset only (does not affect login/register).</summary>
        public static string ValidateResetPassword(string value)
    {
      if (string.IsNullOrEmpty(value))
      {
        return RequiredMessage;
      }

      if (value.Length < 8 || value.Length > 10)
      {
        return "Password must be 8-10 characters.";
      }

      bool hasLetter = false;
      bool hasDigit = false;
      bool hasSpecial = false;

      foreach (char c in value)
      {
        if (IsEnglishLetter(c))
        {
          hasLetter = true;
        }
        else if (char.IsDigit(c))
        {
          hasDigit = true;
        }
        else if ("!$#()".IndexOf(c) >= 0)
        {
          hasSpecial = true;
        }
        else
        {
          return "Password may only use letters, digits, and !$#() special characters.";
        }
      }

      if (!hasLetter)
      {
        return "Password must contain at least one letter.";
      }

      if (!hasDigit)
      {
        return "Password must contain at least one number.";
      }

      if (!hasSpecial)
      {
        return "Password must contain at least one special character (!$#()).";
      }

      return null;
    }

    public static string ValidateConfirmPassword(string password, string confirm)
    {
      string required = ValidateRequired(confirm);
      if (required != null)
      {
        return required;
      }

      if (!string.Equals(password ?? string.Empty, confirm, StringComparison.Ordinal))
      {
        return "Passwords do not match.";
      }

      return null;
    }

    public static string ValidateSearchDigitsOnly(string value)
    {
      string required = ValidateRequired(value);
      if (required != null)
      {
        return required;
      }

      value = value.Trim();
      foreach (char c in value)
      {
        if (!char.IsDigit(c))
        {
          return "Search value must contain numbers only.";
        }
      }

      return null;
    }

    public static string ValidateVerificationCode(string value)
    {
      string required = ValidateRequired(value);
      if (required != null)
      {
        return required;
      }

      value = value.Trim();
      if (value.Length != 6)
      {
        return "Verification code must be 6 digits.";
      }

      foreach (char c in value)
      {
        if (!char.IsDigit(c))
        {
          return "Verification code must contain numbers only.";
        }
      }

      return null;
    }

    public static string ValidateName(string value)
    {
      string required = ValidateRequired(value);
      if (required != null)
      {
        return required;
      }

      value = value.Trim();
      int letterCount = 0;

      foreach (char c in value)
      {
        if (c == ' ')
        {
          continue;
        }

        if (!IsEnglishLetter(c))
        {
          return "Name may contain letters only.";
        }

        letterCount++;
      }

      if (letterCount < 2)
      {
        return "Name must be at least 2 letters.";
      }

      return null;
    }

    public static string ValidateAddress(string value)
    {
      string required = ValidateRequired(value);
      if (required != null)
      {
        return required;
      }

      if (value.Trim().Length < 3)
      {
        return "Address must be at least 3 characters.";
      }

      return null;
    }

    public static string ValidateRole(string role)
    {
      if (string.IsNullOrWhiteSpace(role))
      {
        return RequiredMessage;
      }

      role = role.Trim();
      if (role != "Vet" && role != "Secretary")
      {
        return "Role must be Vet or Secretary.";
      }

      return null;
    }

    public static string ValidateEmployeeNumber(string value)
    {
      string required = ValidateRequired(value);
      if (required != null)
      {
        return required;
      }

      value = value.Trim();
      if (value.Length != 4)
      {
        return "Employee number must be exactly 4 digits.";
      }

      foreach (char c in value)
      {
        if (!char.IsDigit(c))
        {
          return "Employee number must contain numbers only.";
        }
      }

      return null;
    }

    private static bool IsEnglishLetter(char c)
    {
      return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }
  }
}
