using System;
using System.IO;

public static class ExcelFileManager
{
    public const string CustomerSheetName = "Customer";
    public const string EmployeeSheetName = "Employees";

    public const string WorkbookFileName = "ClinicVetsData.xlsx";
    public const string WorkbookRelativeToProject = @"Data\ClinicVetsData.xlsx";

    public const string WorkbookLockedMessage = "Please close the Excel file before saving.";

    public const string WorkbookNotFoundMessage =
        "Excel workbook not found at ClinicVets\\Data\\ClinicVetsData.xlsx";

    /// <summary>Preferred Customer header order when headers must be created.</summary>
    public static readonly string[] CustomerColumnHeaders = { "CustomerID", "FullName", "IDNumber", "Phone", "Email" };

    /// <summary>Preferred Employees header order when headers must be created.</summary>
    public static readonly string[] EmployeeColumnHeaders = { "Username", "Password", "EmployeeNumber", "Email", "ID", "Role" };

    public static string GetWorkbookFullPath()
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar);

        string projectDirectory = Path.GetFullPath(Path.Combine(baseDir, "..", ".."));
        return Path.GetFullPath(Path.Combine(projectDirectory, WorkbookRelativeToProject));
    }
}
