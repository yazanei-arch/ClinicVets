using System;
using System.IO;

public static class ExcelFileManager
{
    public const string CustomerSheetName = "Customer";
    public const string EmployeeSheetName = "Employees";

    public const string WorkbookFileName = "ClinicVetsData.xlsx";

    /// <summary>Single canonical Excel file used by every form in the app.</summary>
    public const string UnifiedWorkbookPath =
        @"C:\Users\abdal\source\repos\ClinicVets\ClinicVetsData.xlsx";

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
        "EmployeeNumber", "Username", "FullName", "Password", "Email", "NationalID", "Role"
    };

    public static string GetWorkbookFullPath()
    {
        if (File.Exists(UnifiedWorkbookPath))
        {
            return UnifiedWorkbookPath;
        }

        string currentDir = AppDomain.CurrentDomain.BaseDirectory;
        DirectoryInfo dir = new DirectoryInfo(currentDir);

        while (dir != null)
        {
            string possiblePath = Path.Combine(dir.FullName, WorkbookFileName);

            if (File.Exists(possiblePath)
                && !IsInsideDataFolder(possiblePath))
            {
                return possiblePath;
            }

            dir = dir.Parent;
        }

        return UnifiedWorkbookPath;
    }

    private static bool IsInsideDataFolder(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        string parent = Path.GetDirectoryName(path);
        if (string.IsNullOrEmpty(parent))
        {
            return false;
        }

        string folderName = new DirectoryInfo(parent).Name;
        return string.Equals(folderName, "Data", StringComparison.OrdinalIgnoreCase);
    }
}