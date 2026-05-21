public static class SessionManager
{
    public static Employee CurrentUser { get; set; }
    public static string CurrentUsername { get; private set; } = string.Empty;
    public static string CurrentRole { get; private set; } = string.Empty;

    public static void SetUser(Employee employee)
    {
        CurrentUser = employee;
        CurrentUsername = employee?.Username?.Trim() ?? string.Empty;
        CurrentRole = employee?.Role?.Trim() ?? string.Empty;
    }

    public static void Clear()
    {
        CurrentUser = null;
        CurrentUsername = string.Empty;
        CurrentRole = string.Empty;
    }
}
