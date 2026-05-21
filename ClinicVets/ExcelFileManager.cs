using System;
using System.IO;

public static class ExcelFileManager
{
    public const string CustomerSheetName = "Customer";
    public const string EmployeeSheetName = "Employees";

    public const string WorkbookFileName = "ClinicVetsData.xlsx";

    public static string FilePath => GetWorkbookFullPath();

    public const string WorkbookLockedMessage = "Please close the Excel file before saving.";

    public const string WorkbookNotFoundMessage =
        "Excel file not found";

    public static readonly string[] CustomerColumnHeaders =
    {
        "CustomerID", "FullName", "IDNumber", "Phone", "Email"
    };

    /// <summary>Canonical header row for new/repaired Employees sheets (login order).</summary>
    public static readonly string[] EmployeeColumnHeaders =
    {
        "EmployeeNumber", "Username", "Password", "Email", "ID", "Role"
    };

    public static string GetWorkbookFullPath()
    {
        string currentDir = AppDomain.CurrentDomain.BaseDirectory;

        DirectoryInfo dir = new DirectoryInfo(currentDir);

        while (dir != null)
        {
            string possiblePath = Path.Combine(dir.FullName, WorkbookFileName);

            if (File.Exists(possiblePath))
            {
                return possiblePath;
            }

            dir = dir.Parent;
        }

        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, WorkbookFileName);
    }
}