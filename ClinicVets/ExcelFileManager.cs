using System;
using System.IO;

public static class ExcelFileManager
{
    public const string CustomerSheetName = "Customer";

    /// <summary>Column order in the Customer sheet: A–E.</summary>
    public static readonly string[] CustomerColumnHeaders = { "Email", "Phone", "IDNumber", "FullName", "CustomerID" };

    public static string FilePath = @"C:\Users\abdal\source\repos\ClinicVets\ClinicVetsData.xlsx";

    public static string GetWorkbookFullPath()
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        string relative = FilePath.Replace('/', Path.DirectorySeparatorChar);
        return Path.Combine(baseDir, relative);
    }
}
