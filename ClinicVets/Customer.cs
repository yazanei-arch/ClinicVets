public class Customer
{
    public string CustomerID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }

    /// <summary>Legacy Excel column; mapped when reading older workbooks.</summary>
    public string FullName { get; set; }

    /// <summary>Legacy Excel column; mapped when reading older workbooks.</summary>
    public string IDNumber { get; set; }

    public string DisplayName
    {
        get
        {
            string first = (FirstName ?? string.Empty).Trim();
            string last = (LastName ?? string.Empty).Trim();
            if (first.Length > 0 || last.Length > 0)
            {
                return (first + " " + last).Trim();
            }

            return (FullName ?? string.Empty).Trim();
        }
    }
}