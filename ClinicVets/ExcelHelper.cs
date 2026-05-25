using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

/// <summary>
/// Minimal Excel read/write via Open XML SDK only (plain cells, no styles/fonts/tables).
/// </summary>
public class ExcelHelper
{
    public static event EventHandler CustomersChanged;

    private readonly string _workbookPath;

    public ExcelHelper()
        : this(ExcelFileManager.GetWorkbookFullPath())
    {
    }

    public ExcelHelper(string workbookFullPath)
    {
        _workbookPath = workbookFullPath ?? throw new ArgumentNullException(nameof(workbookFullPath));
    }

    public DataTable ReadSheet(string sheetName)
    {
        var table = new DataTable();
        if (!File.Exists(_workbookPath))
        {
            return table;
        }

        using (SpreadsheetDocument doc = SpreadsheetDocument.Open(_workbookPath, false))
        {
            WorksheetPart wsp = GetWorksheetPartByName(doc.WorkbookPart, sheetName);
            if (wsp == null)
            {
                return table;
            }

            SharedStringTablePart sstp = doc.WorkbookPart.SharedStringTablePart;
            SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>();
            if (sheetData == null)
            {
                return table;
            }

            Row firstRow = sheetData.Elements<Row>().OrderBy(r => r.RowIndex?.Value ?? uint.MaxValue).FirstOrDefault();
            if (firstRow == null)
            {
                return table;
            }

            foreach (Cell cell in firstRow.Elements<Cell>().OrderBy(c => ColumnIndexFromReference(c.CellReference)))
            {
                string colName = GetCellRawText(cell, sstp).Trim();
                if (string.IsNullOrEmpty(colName))
                {
                    colName = "Column" + ColumnIndexFromReference(cell.CellReference);
                }

                if (!table.Columns.Contains(colName))
                {
                    var col = new DataColumn(colName, typeof(string));
                    col.ExtendedProperties.Add("ExcelColumn", ColumnIndexFromReference(cell.CellReference));
                    table.Columns.Add(col);
                }
            }

            if (table.Columns.Count == 0)
            {
                return table;
            }

            uint headerRowIndex = firstRow.RowIndex?.Value ?? 1u;
            foreach (Row row in sheetData.Elements<Row>().OrderBy(r => r.RowIndex?.Value ?? 0))
            {
                uint ri = row.RowIndex?.Value ?? 0u;
                if (ri <= headerRowIndex)
                {
                    continue;
                }

                DataRow dataRow = table.NewRow();
                bool any = false;
                foreach (DataColumn col in table.Columns)
                {
                    int colNum = (int)col.ExtendedProperties["ExcelColumn"];
                    Cell cell = FindCellInRow(row, colNum);
                    string value = cell != null ? GetCellRawText(cell, sstp) : string.Empty;
                    dataRow[col.ColumnName] = value;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        any = true;
                    }
                }

                if (any)
                {
                    table.Rows.Add(dataRow);
                }
            }
        }

        return table;
    }

    public void WriteSheet(string sheetName, DataTable table)
    {
        if (table == null)
        {
            throw new ArgumentNullException(nameof(table));
        }

        string directory = Path.GetDirectoryName(_workbookPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(_workbookPath))
        {
            CreateWorkbookWithEmptySheet(sheetName);
        }

        using (SpreadsheetDocument doc = SpreadsheetDocument.Open(_workbookPath, true))
        {
            WorksheetPart wsp = GetOrCreateWorksheetPart(doc.WorkbookPart, sheetName);
            SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>() ?? wsp.Worksheet.AppendChild(new SheetData());
            foreach (Row r in sheetData.Elements<Row>().ToList())
            {
                r.Remove();
            }

            uint rowIndex = 1;
            var headerRow = new Row { RowIndex = rowIndex };
            for (int c = 0; c < table.Columns.Count; c++)
            {
                headerRow.AppendChild(NewInlineTextCell(rowIndex, c + 1, table.Columns[c].ColumnName ?? string.Empty));
            }

            sheetData.AppendChild(headerRow);

            for (int r = 0; r < table.Rows.Count; r++)
            {
                rowIndex++;
                var dataRow = new Row { RowIndex = rowIndex };
                for (int c = 0; c < table.Columns.Count; c++)
                {
                    object val = table.Rows[r][c];
                    string text = val != null && val != DBNull.Value ? Convert.ToString(val, CultureInfo.InvariantCulture) : string.Empty;
                    dataRow.AppendChild(NewInlineTextCell(rowIndex, c + 1, text ?? string.Empty));
                }

                sheetData.AppendChild(dataRow);
            }

            wsp.Worksheet.Save();
            doc.WorkbookPart.Workbook.Save();
        }
    }

    /// <summary>
    /// Reads the Pets sheet and groups pet names by their owner name (case-insensitive).
    /// Used by views that join customers to their pets without touching pet-management logic.
    /// </summary>
    public IReadOnlyDictionary<string, List<string>> ReadPetOwnerIndex()
    {
        var index = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        if (!File.Exists(_workbookPath))
        {
            return index;
        }

        using (SpreadsheetDocument doc = SpreadsheetDocument.Open(_workbookPath, false))
        {
            WorksheetPart wsp = GetWorksheetPartByName(doc.WorkbookPart, "Pets");
            if (wsp == null)
            {
                return index;
            }

            SharedStringTablePart sstp = doc.WorkbookPart.SharedStringTablePart;
            Dictionary<string, int> headerMap = BuildHeaderMap(wsp, sstp);

            int colPetName = ResolveFirstColumn(headerMap, "PetName", "Pet Name", "Name");
            int colOwner = ResolveFirstColumn(headerMap, "Owner", "OwnerName", "Owner Name");

            if (colPetName <= 0)
            {
                colPetName = 2;
            }

            if (colOwner <= 0)
            {
                colOwner = 6;
            }

            SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>();
            if (sheetData == null)
            {
                return index;
            }

            foreach (Row row in sheetData.Elements<Row>())
            {
                uint ri = row.RowIndex?.Value ?? 0u;
                if (ri < 2u)
                {
                    continue;
                }

                Dictionary<int, Cell> cellsByCol = IndexRowCellsByColumn(row);
                string petName = ReadCellTrimmed(cellsByCol, colPetName, sstp);
                string owner = ReadCellTrimmed(cellsByCol, colOwner, sstp);

                if (string.IsNullOrWhiteSpace(petName) || string.IsNullOrWhiteSpace(owner))
                {
                    continue;
                }

                if (!index.TryGetValue(owner, out List<string> pets))
                {
                    pets = new List<string>();
                    index[owner] = pets;
                }

                pets.Add(petName);
            }
        }

        return index;
    }

    public IReadOnlyList<Customer> ReadCustomers()
    {
        var list = new List<Customer>();
        if (!File.Exists(_workbookPath))
        {
            return list;
        }

        using (SpreadsheetDocument doc = SpreadsheetDocument.Open(_workbookPath, false))
        {
            WorksheetPart wsp = GetWorksheetPartByName(doc.WorkbookPart, ExcelFileManager.CustomerSheetName);
            if (wsp == null)
            {
                return list;
            }

            SharedStringTablePart sstp = doc.WorkbookPart.SharedStringTablePart;
            Dictionary<string, int> headerMap = BuildHeaderMap(wsp, sstp);
            return ReadCustomersCore(wsp, sstp, headerMap);
        }
    }

    public void AppendCustomer(Customer customer)
    {
        if (customer == null)
        {
            throw new ArgumentNullException(nameof(customer));
        }

        try
        {
            RequireWorkbookForWrite();
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(_workbookPath, true))
            {
                WorksheetPart wsp = GetWorksheetPartByName(doc.WorkbookPart, ExcelFileManager.CustomerSheetName);
                if (wsp == null)
                {
                    throw new InvalidOperationException(
                        "The Customer worksheet was not found in " + ExcelFileManager.WorkbookFileName + ".");
                }

                SharedStringTablePart sstp = doc.WorkbookPart.SharedStringTablePart;
                Dictionary<string, int> headerMap = BuildHeaderMap(wsp, sstp);
                if (headerMap.Count == 0)
                {
                    throw new InvalidOperationException("The Customer worksheet has no header row.");
                }

                uint nextRow = GetLastUsedRowIndex(wsp, sstp, headerMap) + 1u;
                if (nextRow < 2u)
                {
                    nextRow = 2u;
                }

                SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>() ?? wsp.Worksheet.AppendChild(new SheetData());
                var row = new Row { RowIndex = nextRow };
                AppendCustomerCells(row, nextRow, headerMap, customer);
                sheetData.AppendChild(row);

                wsp.Worksheet.Save();
                doc.WorkbookPart.Workbook.Save();
            }

            NotifyCustomersChanged();
        }
        catch (Exception ex) when (IsWorkbookLockedException(ex))
        {
            throw new InvalidOperationException(ExcelFileManager.WorkbookLockedMessage, ex);
        }
    }

    public static bool IsWorkbookLockedException(Exception ex)
    {
        while (ex != null)
        {
            if (ex is IOException || ex is UnauthorizedAccessException)
            {
                return true;
            }

            ex = ex.InnerException;
        }

        return false;
    }

    public void UpdateCustomer(Customer customer)
    {
        if (customer == null)
        {
            throw new ArgumentNullException(nameof(customer));
        }

        if (string.IsNullOrWhiteSpace(customer.CustomerID))
        {
            throw new ArgumentException("Customer ID is required.", nameof(customer));
        }

        RequireWorkbookForWrite();
        using (SpreadsheetDocument doc = SpreadsheetDocument.Open(_workbookPath, true))
        {
            WorksheetPart wsp = GetWorksheetPartByName(doc.WorkbookPart, ExcelFileManager.CustomerSheetName);
            if (wsp == null)
            {
                throw new InvalidOperationException("Customer worksheet is missing.");
            }

            SharedStringTablePart sstp = doc.WorkbookPart.SharedStringTablePart;
            Dictionary<string, int> headerMap = BuildHeaderMap(wsp, sstp);
            uint? rowIndex = FindCustomerRowIndex(wsp, sstp, headerMap, customer.CustomerID.Trim());
            if (!rowIndex.HasValue)
            {
                throw new InvalidOperationException("Customer was not found in the workbook.");
            }

            ReplaceCustomerRow(wsp, sstp, headerMap, rowIndex.Value, customer);
            wsp.Worksheet.Save();
            doc.WorkbookPart.Workbook.Save();
        }
    }

    public void DeleteCustomer(string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            throw new ArgumentException("Customer ID is required.", nameof(customerId));
        }

        RequireWorkbookForWrite();
        using (SpreadsheetDocument doc = SpreadsheetDocument.Open(_workbookPath, true))
        {
            WorksheetPart wsp = GetWorksheetPartByName(doc.WorkbookPart, ExcelFileManager.CustomerSheetName);
            if (wsp == null)
            {
                throw new InvalidOperationException("Customer worksheet is missing.");
            }

            SharedStringTablePart sstp = doc.WorkbookPart.SharedStringTablePart;
            Dictionary<string, int> headerMap = BuildHeaderMap(wsp, sstp);
            uint? rowIndex = FindCustomerRowIndex(wsp, sstp, headerMap, customerId.Trim());
            if (!rowIndex.HasValue)
            {
                throw new InvalidOperationException("Customer was not found in the workbook.");
            }

            RemoveRow(wsp, rowIndex.Value);
            wsp.Worksheet.Save();
            doc.WorkbookPart.Workbook.Save();
        }
    }

    public IReadOnlyList<Employee> ReadEmployees()
    {
        var list = new List<Employee>();
        if (!File.Exists(_workbookPath))
        {
            return list;
        }

        using (SpreadsheetDocument doc = SpreadsheetDocument.Open(_workbookPath, false))
        {
            WorksheetPart wsp = GetWorksheetPartByName(doc.WorkbookPart, ExcelFileManager.EmployeeSheetName);
            if (wsp == null)
            {
                return list;
            }

            SharedStringTablePart sstp = doc.WorkbookPart.SharedStringTablePart;
            Dictionary<string, int> headerMap = BuildEmployeeHeaderMapFromSheet(wsp, sstp);
            if (EmployeeLogicalHeadersMissing(headerMap))
            {
                return list;
            }

            SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>();
            if (sheetData == null)
            {
                return list;
            }

            int colEmployeeId = ResolveFirstColumn(headerMap, "EmployeeNumber", "EmployeeNum", "EmployeeI", "EmployeeID");
            int colUsername = ResolveFirstColumn(headerMap, "Username");
            int colPassword = ResolveFirstColumn(headerMap, "Password");
            int colEmail = ResolveFirstColumn(headerMap, "Email");
            int colNationalId = ResolveFirstColumn(headerMap, "NationalID", "ID");
            int colRole = ResolveFirstColumn(headerMap, "Role");
            int colFullName = ResolveFirstColumn(headerMap, "FullName");

            foreach (Row row in sheetData.Elements<Row>())
            {
                uint ri = row.RowIndex?.Value ?? 0u;
                if (ri < 2u)
                {
                    continue;
                }

                Dictionary<int, Cell> cellsByCol = IndexRowCellsByColumn(row);

                string username = ReadCellTrimmed(cellsByCol, colUsername, sstp);
                string password = ReadCellTrimmed(cellsByCol, colPassword, sstp);

                if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
                {
                    continue;
                }

                list.Add(new Employee
                {
                    EmployeeID = ReadCellTrimmed(cellsByCol, colEmployeeId, sstp),
                    Username = username,
                    Password = password,
                    Email = ReadCellTrimmed(cellsByCol, colEmail, sstp),
                    NationalID = ReadCellTrimmed(cellsByCol, colNationalId, sstp),
                    Role = ReadCellTrimmed(cellsByCol, colRole, sstp),
                    FullName = ReadCellTrimmed(cellsByCol, colFullName, sstp)
                });
            }
        }

        return list;
    }

    public void AppendEmployee(Employee employee)
    {
        if (employee == null)
        {
            throw new ArgumentNullException(nameof(employee));
        }

        RequireWorkbookForWrite();

        try
        {
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(_workbookPath, true))
            {
                WorksheetPart wsp = GetWorksheetPartByName(doc.WorkbookPart, ExcelFileManager.EmployeeSheetName);
                if (wsp == null)
                {
                    throw new InvalidOperationException(
                        "The Employees worksheet was not found in " + ExcelFileManager.WorkbookFileName + ".");
                }

                SharedStringTablePart sstp = doc.WorkbookPart.SharedStringTablePart;
                Dictionary<string, int> headerMap = EnsureEmployeeHeaderRow(wsp, sstp);
                headerMap = LabelUnlabeledEmployeeColumns(wsp, sstp, headerMap);
                foreach (string canonicalHeader in ExcelFileManager.EmployeeColumnHeaders)
                {
                    headerMap = EnsureOptionalEmployeeHeaderColumn(wsp, headerMap, canonicalHeader);
                }

                uint nextRow = GetLastUsedRowIndex(wsp, sstp, headerMap) + 1u;
                if (nextRow < 2u)
                {
                    nextRow = 2u;
                }

                SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>() ?? wsp.Worksheet.AppendChild(new SheetData());
                var row = new Row { RowIndex = nextRow };
                AppendEmployeeCells(row, nextRow, headerMap, employee);
                sheetData.AppendChild(row);

                wsp.Worksheet.Save();
                doc.WorkbookPart.Workbook.Save();
            }
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            if (IsWorkbookLockedException(ex))
            {
                throw new InvalidOperationException(ExcelFileManager.WorkbookLockedMessage, ex);
            }

            throw;
        }
    }

    /// <summary>
    /// Login-only Excel read: resolves Username/Password/Role columns by header name
    /// (never by fixed column letters/indexes) and authenticates against trimmed values.
    /// Tolerant of other Employees columns being missing or reordered.
    /// </summary>
    public Employee TryAuthenticateEmployee(string username, string password)
    {
        username = (username ?? string.Empty).Trim();
        password = (password ?? string.Empty).Trim();
        if (username.Length == 0 || password.Length == 0)
        {
            return null;
        }

        if (!File.Exists(_workbookPath))
        {
            return null;
        }

        using (SpreadsheetDocument doc = SpreadsheetDocument.Open(_workbookPath, false))
        {
            WorksheetPart wsp = GetWorksheetPartByName(doc.WorkbookPart, ExcelFileManager.EmployeeSheetName);
            if (wsp == null)
            {
                return null;
            }

            SharedStringTablePart sstp = doc.WorkbookPart.SharedStringTablePart;
            Dictionary<string, int> headerMap = BuildEmployeeHeaderMapFromSheet(wsp, sstp);

            int colUsername = ResolveFirstColumn(headerMap, "Username");
            int colPassword = ResolveFirstColumn(headerMap, "Password");
            int colRole = ResolveFirstColumn(headerMap, "Role");
            if (colUsername <= 0 || colPassword <= 0)
            {
                return null;
            }

            int colEmployeeId = ResolveFirstColumn(headerMap, "EmployeeNumber", "EmployeeNum", "EmployeeI", "EmployeeID");
            int colEmail = ResolveFirstColumn(headerMap, "Email");
            int colNationalId = ResolveFirstColumn(headerMap, "NationalID", "ID");
            int colFullName = ResolveFirstColumn(headerMap, "FullName");

            SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>();
            if (sheetData == null)
            {
                return null;
            }

            foreach (Row row in sheetData.Elements<Row>())
            {
                uint ri = row.RowIndex?.Value ?? 0u;
                if (ri < 2u)
                {
                    continue;
                }

                Dictionary<int, Cell> cellsByCol = IndexRowCellsByColumn(row);
                string rowUsername = ReadCellTrimmed(cellsByCol, colUsername, sstp);
                string rowPassword = ReadCellTrimmed(cellsByCol, colPassword, sstp);

                if (rowUsername.Length == 0 || rowPassword.Length == 0)
                {
                    continue;
                }

                if (!string.Equals(rowUsername, username, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(rowPassword, password, StringComparison.Ordinal))
                {
                    continue;
                }

                return new Employee
                {
                    EmployeeID = ReadCellTrimmed(cellsByCol, colEmployeeId, sstp),
                    Username = rowUsername,
                    Password = rowPassword,
                    Email = ReadCellTrimmed(cellsByCol, colEmail, sstp),
                    NationalID = ReadCellTrimmed(cellsByCol, colNationalId, sstp),
                    Role = ReadCellTrimmed(cellsByCol, colRole, sstp),
                    FullName = ReadCellTrimmed(cellsByCol, colFullName, sstp)
                };
            }
        }

        return null;
    }

    public Employee TryFindEmployeeByEmail(string email)
    {
        email = (email ?? string.Empty).Trim();
        if (email.Length == 0)
        {
            return null;
        }

        foreach (Employee employee in ReadEmployees())
        {
            if (string.Equals(employee.Email?.Trim(), email, StringComparison.OrdinalIgnoreCase))
            {
                return employee;
            }
        }

        return null;
    }

    public bool UpdateEmployeePasswordByEmail(string email, string newPassword)
    {
        email = (email ?? string.Empty).Trim();
        newPassword = newPassword ?? string.Empty;
        if (email.Length == 0)
        {
            return false;
        }

        if (!File.Exists(_workbookPath))
        {
            return false;
        }

        using (SpreadsheetDocument doc = SpreadsheetDocument.Open(_workbookPath, true))
        {
            WorksheetPart wsp = GetWorksheetPartByName(doc.WorkbookPart, ExcelFileManager.EmployeeSheetName);
            if (wsp == null)
            {
                return false;
            }

            SharedStringTablePart sstp = doc.WorkbookPart.SharedStringTablePart;
            Dictionary<string, int> headerMap = BuildEmployeeHeaderMapFromSheet(wsp, sstp);
            uint? rowIndex = FindEmployeeRowIndexByEmail(wsp, sstp, headerMap, email);
            if (!rowIndex.HasValue)
            {
                return false;
            }

            SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>();
            Row row = sheetData?.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex.Value);
            if (row == null)
            {
                return false;
            }

            SetCellOnRow(row, rowIndex.Value, headerMap, "Password", newPassword);
            wsp.Worksheet.Save();
            doc.WorkbookPart.Workbook.Save();
            return true;
        }
    }

    private void RequireWorkbookForWrite()
    {
        if (!File.Exists(_workbookPath))
        {
            throw new InvalidOperationException(ExcelFileManager.WorkbookNotFoundMessage);
        }
    }

    private void WriteEmployeeHeaderRowOnly()
    {
        using (SpreadsheetDocument doc = SpreadsheetDocument.Open(_workbookPath, true))
        {
            WorksheetPart wsp = GetWorksheetPartByName(doc.WorkbookPart, ExcelFileManager.EmployeeSheetName);
            if (wsp != null)
            {
                ReplaceEmployeeHeaderRow(wsp);
                wsp.Worksheet.Save();
                doc.WorkbookPart.Workbook.Save();
            }
        }
    }

    private static void ReplaceEmployeeHeaderRow(WorksheetPart wsp)
    {
        SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>() ?? wsp.Worksheet.AppendChild(new SheetData());
        Row row1 = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == 1u);
        row1?.Remove();

        row1 = new Row { RowIndex = 1u };
        for (int i = 0; i < ExcelFileManager.EmployeeColumnHeaders.Length; i++)
        {
            row1.AppendChild(NewInlineTextCell(1u, i + 1, ExcelFileManager.EmployeeColumnHeaders[i]));
        }

        sheetData.InsertAt(row1, 0);
    }

    private static readonly string[] RequiredEmployeeLogicalColumns =
    {
        "EmployeeNumber", "Username", "Password", "Email", "NationalID", "Role"
    };

    private static bool EmployeeLogicalHeadersMissing(IReadOnlyDictionary<string, int> headerMap)
    {
        if (headerMap == null || headerMap.Count == 0)
        {
            return true;
        }

        foreach (string required in RequiredEmployeeLogicalColumns)
        {
            if (!headerMap.ContainsKey(required))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Maps legacy header labels (EmployeeI, NationalID, etc.) to canonical columns without moving data.
    /// </summary>
    private static Dictionary<string, int> BuildEmployeeHeaderMapFromSheet(WorksheetPart wsp, SharedStringTablePart sstp)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>();
        if (sheetData == null)
        {
            return map;
        }

        Row first = sheetData.Elements<Row>().OrderBy(r => r.RowIndex?.Value ?? uint.MaxValue).FirstOrDefault();
        if (first == null)
        {
            return map;
        }

        foreach (Cell cell in first.Elements<Cell>().OrderBy(c => ColumnIndexFromReference(c.CellReference)))
        {
            string raw = GetCellRawText(cell, sstp).Trim();
            if (raw.Length == 0)
            {
                continue;
            }

            int col = ColumnIndexFromReference(cell.CellReference);
            RegisterEmployeeHeaderAlias(map, raw, col);
        }

        return map;
    }

    private static void RegisterEmployeeHeaderAlias(Dictionary<string, int> map, string rawHeader, int columnIndex)
    {
        map[rawHeader] = columnIndex;

        string canonical = NormalizeEmployeeHeaderKey(rawHeader);
        if (canonical.Length > 0)
        {
            map[canonical] = columnIndex;
        }
    }

    private static string NormalizeEmployeeHeaderKey(string rawHeader)
    {
        string trimmed = (rawHeader ?? string.Empty).Trim();
        if (trimmed.Length == 0)
        {
            return string.Empty;
        }

        string compact = trimmed.Replace(" ", string.Empty);
        switch (compact.ToUpperInvariant())
        {
            case "EMPLOYEEI":
            case "EMPLOYEEID":
            case "EMPLOYEENUMBER":
            case "EMPLOYEENUM":
            case "EMPLOYEENO":
            case "EMPNUMBER":
                return "EmployeeNumber";
            case "NATIONALID":
            case "ID":
            case "IDNUMBER":
                return "NationalID";
            case "USERNAME":
                return "Username";
            case "PASSWORD":
                return "Password";
            case "EMAIL":
                return "Email";
            case "ROLE":
                return "Role";
            case "FULLNAME":
            case "FULL_NAME":
            case "NAME":
                return "FullName";
            default:
                return trimmed;
        }
    }

    private static bool IsLegacyEmployeeHeaderLabel(string rawHeader, string canonicalHeader)
    {
        return string.Equals(NormalizeEmployeeHeaderKey(rawHeader), canonicalHeader, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(rawHeader.Trim(), canonicalHeader, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Renames header labels in row 1 only (same columns) — does not recreate the workbook or move data.
    /// </summary>
    private static void RenameLegacyEmployeeHeaderLabels(WorksheetPart wsp, SharedStringTablePart sstp)
    {
        SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>();
        Row row1 = sheetData?.Elements<Row>().FirstOrDefault(r => (r.RowIndex?.Value ?? 1u) == 1u);
        if (row1 == null)
        {
            return;
        }

        foreach (Cell cell in row1.Elements<Cell>().ToList())
        {
            string raw = GetCellRawText(cell, sstp).Trim();
            if (raw.Length == 0)
            {
                continue;
            }

            string canonical = NormalizeEmployeeHeaderKey(raw);
            if (canonical.Length == 0 || !IsLegacyEmployeeHeaderLabel(raw, canonical))
            {
                continue;
            }

            int col = ColumnIndexFromReference(cell.CellReference);
            if (col <= 0)
            {
                continue;
            }

            cell.Remove();
            row1.AppendChild(NewInlineTextCell(1u, col, canonical));
        }
    }

    /// <summary>
    /// Uses existing column layout when possible; only creates row 1 when headers are truly missing.
    /// </summary>
    private static Dictionary<string, int> EnsureEmployeeHeaderRow(WorksheetPart wsp, SharedStringTablePart sstp)
    {
        Dictionary<string, int> headerMap = BuildEmployeeHeaderMapFromSheet(wsp, sstp);
        if (!EmployeeLogicalHeadersMissing(headerMap))
        {
            RenameLegacyEmployeeHeaderLabels(wsp, sstp);
            return BuildEmployeeHeaderMapFromSheet(wsp, sstp);
        }

        ReplaceEmployeeHeaderRow(wsp);
        return BuildStandardEmployeeHeaderMap();
    }

    private static Dictionary<string, int> BuildStandardEmployeeHeaderMap()
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < ExcelFileManager.EmployeeColumnHeaders.Length; i++)
        {
            string header = ExcelFileManager.EmployeeColumnHeaders[i];
            int col = i + 1;
            RegisterEmployeeHeaderAlias(map, header, col);
        }

        return map;
    }

    private static void AppendEmployeeCells(Row row, uint rowIndex, IReadOnlyDictionary<string, int> headerMap, Employee employee)
    {
        SetCellOnRowAny(row, rowIndex, headerMap, (employee.EmployeeID ?? string.Empty).Trim(), "EmployeeNumber", "EmployeeNum", "EmployeeI", "EmployeeID");
        SetCellOnRowAny(row, rowIndex, headerMap, (employee.Username ?? string.Empty).Trim(), "Username");
        SetCellOnRowAny(row, rowIndex, headerMap, (employee.FullName ?? string.Empty).Trim(), "FullName");
        SetCellOnRowAny(row, rowIndex, headerMap, employee.Password ?? string.Empty, "Password");
        SetCellOnRowAny(row, rowIndex, headerMap, (employee.Email ?? string.Empty).Trim(), "Email");
        SetCellOnRowAny(row, rowIndex, headerMap, (employee.NationalID ?? string.Empty).Trim(), "NationalID", "ID");
        SetCellOnRowAny(row, rowIndex, headerMap, (employee.Role ?? string.Empty).Trim(), "Role");
    }

    /// <summary>
    /// Labels columns that contain data but have a blank/missing header cell in row 1.
    /// If exactly one canonical Employees header is missing AND exactly one data column is unlabeled,
    /// the unlabeled column receives the missing canonical name without moving any data.
    /// </summary>
    private static Dictionary<string, int> LabelUnlabeledEmployeeColumns(
        WorksheetPart wsp,
        SharedStringTablePart sstp,
        Dictionary<string, int> headerMap)
    {
        SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>();
        if (sheetData == null)
        {
            return headerMap;
        }

        Row row1 = sheetData.Elements<Row>().FirstOrDefault(r => (r.RowIndex?.Value ?? 1u) == 1u);
        if (row1 == null)
        {
            return headerMap;
        }

        int maxDataColumn = 0;
        foreach (Row dataRow in sheetData.Elements<Row>())
        {
            uint rowIdx = dataRow.RowIndex?.Value ?? 0u;
            if (rowIdx <= 1u)
            {
                continue;
            }

            foreach (Cell cell in dataRow.Elements<Cell>())
            {
                int col = ColumnIndexFromReference(cell.CellReference);
                if (col > maxDataColumn)
                {
                    maxDataColumn = col;
                }
            }
        }

        if (maxDataColumn <= 0)
        {
            return headerMap;
        }

        var labeledColumns = new HashSet<int>(headerMap.Values);
        var unlabeledColumns = new List<int>();
        for (int col = 1; col <= maxDataColumn; col++)
        {
            if (!labeledColumns.Contains(col))
            {
                unlabeledColumns.Add(col);
            }
        }

        var missingCanonicalHeaders = new List<string>();
        foreach (string canonicalHeader in ExcelFileManager.EmployeeColumnHeaders)
        {
            if (!headerMap.ContainsKey(canonicalHeader))
            {
                missingCanonicalHeaders.Add(canonicalHeader);
            }
        }

        if (unlabeledColumns.Count != 1 || missingCanonicalHeaders.Count != 1)
        {
            return headerMap;
        }

        int targetColumn = unlabeledColumns[0];
        string headerName = missingCanonicalHeaders[0];

        InsertOrReplaceCellInRow(row1, 1u, targetColumn, headerName);

        var updated = new Dictionary<string, int>(headerMap, StringComparer.OrdinalIgnoreCase)
        {
            [headerName] = targetColumn
        };
        return updated;
    }

    /// <summary>
    /// Ensures an optional header column exists at the end of the existing header row
    /// without removing or moving any existing columns or data.
    /// </summary>
    private static Dictionary<string, int> EnsureOptionalEmployeeHeaderColumn(
        WorksheetPart wsp,
        Dictionary<string, int> headerMap,
        string canonicalHeaderName)
    {
        if (headerMap == null || string.IsNullOrEmpty(canonicalHeaderName))
        {
            return headerMap;
        }

        if (headerMap.ContainsKey(canonicalHeaderName))
        {
            return headerMap;
        }

        SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>();
        Row row1 = sheetData?.Elements<Row>().FirstOrDefault(r => (r.RowIndex?.Value ?? 1u) == 1u);
        if (row1 == null)
        {
            return headerMap;
        }

        int maxColumn = 0;
        foreach (Cell existing in row1.Elements<Cell>())
        {
            int col = ColumnIndexFromReference(existing.CellReference);
            if (col > maxColumn)
            {
                maxColumn = col;
            }
        }

        int newColumn = maxColumn + 1;
        row1.AppendChild(NewInlineTextCell(1u, newColumn, canonicalHeaderName));

        var updated = new Dictionary<string, int>(headerMap, StringComparer.OrdinalIgnoreCase)
        {
            [canonicalHeaderName] = newColumn
        };
        return updated;
    }

    private static void SetCellOnRowAny(
        Row row,
        uint rowIndex,
        IReadOnlyDictionary<string, int> headerMap,
        string value,
        params string[] columnNames)
    {
        if (columnNames == null)
        {
            return;
        }

        foreach (string columnName in columnNames)
        {
            if (headerMap.ContainsKey(columnName))
            {
                SetCellOnRow(row, rowIndex, headerMap, columnName, value);
                return;
            }
        }
    }

    private IReadOnlyList<Customer> ReadCustomersCore(
        WorksheetPart wsp,
        SharedStringTablePart sstp,
        Dictionary<string, int> headerMap)
    {
        var list = new List<Customer>();
        if (headerMap.Count == 0)
        {
            return list;
        }

        SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>();
        if (sheetData == null)
        {
            return list;
        }

        int colEmail = ResolveFirstColumn(headerMap, "Email");
        int colPhone = ResolveFirstColumn(headerMap, "Phone");
        int colAddress = ResolveFirstColumn(headerMap, "Address");
        int colLastName = ResolveFirstColumn(headerMap, "LastName");
        int colFirstName = ResolveFirstColumn(headerMap, "FirstName");
        int colCustomerId = ResolveFirstColumn(headerMap, "CustomerID");
        int colIdNumber = ResolveFirstColumn(headerMap, "IDNumber");
        int colFullName = ResolveFirstColumn(headerMap, "FullName");

        foreach (Row row in sheetData.Elements<Row>())
        {
            uint ri = row.RowIndex?.Value ?? 0u;
            if (ri < 2u)
            {
                continue;
            }

            Dictionary<int, Cell> cellsByCol = IndexRowCellsByColumn(row);

            string email = ReadCellTrimmed(cellsByCol, colEmail, sstp);
            string phone = ReadCellTrimmed(cellsByCol, colPhone, sstp);
            string address = ReadCellTrimmed(cellsByCol, colAddress, sstp);
            string lastName = ReadCellTrimmed(cellsByCol, colLastName, sstp);
            string firstName = ReadCellTrimmed(cellsByCol, colFirstName, sstp);
            string customerId = ReadCellTrimmed(cellsByCol, colCustomerId, sstp);
            string idNumber = ReadCellTrimmed(cellsByCol, colIdNumber, sstp);
            string fullName = ReadCellTrimmed(cellsByCol, colFullName, sstp);

            if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName) && !string.IsNullOrWhiteSpace(fullName))
            {
                SplitLegacyFullName(fullName, out firstName, out lastName);
            }

            if (string.IsNullOrWhiteSpace(customerId) && !string.IsNullOrWhiteSpace(idNumber))
            {
                customerId = idNumber;
            }

            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone) &&
                string.IsNullOrWhiteSpace(address) && string.IsNullOrWhiteSpace(lastName) &&
                string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(customerId) &&
                string.IsNullOrWhiteSpace(idNumber) && string.IsNullOrWhiteSpace(fullName))
            {
                continue;
            }

            list.Add(new Customer
            {
                Email = email,
                Phone = phone,
                Address = address,
                LastName = lastName,
                FirstName = firstName,
                CustomerID = customerId,
                IDNumber = idNumber,
                FullName = fullName
            });
        }

        return list;
    }

    private static int ResolveFirstColumn(IReadOnlyDictionary<string, int> headerMap, params string[] candidateNames)
    {
        if (headerMap == null || candidateNames == null)
        {
            return 0;
        }

        foreach (string name in candidateNames)
        {
            if (string.IsNullOrEmpty(name))
            {
                continue;
            }

            if (headerMap.TryGetValue(name, out int col) && col > 0)
            {
                return col;
            }
        }

        return 0;
    }

    private static Dictionary<int, Cell> IndexRowCellsByColumn(Row row)
    {
        var cellsByCol = new Dictionary<int, Cell>();
        if (row == null)
        {
            return cellsByCol;
        }

        foreach (Cell cell in row.Elements<Cell>())
        {
            int col = ColumnIndexFromReference(cell.CellReference);
            if (col > 0)
            {
                cellsByCol[col] = cell;
            }
        }

        return cellsByCol;
    }

    private static string ReadCellTrimmed(IReadOnlyDictionary<int, Cell> cellsByCol, int columnIndex1Based, SharedStringTablePart sstp)
    {
        if (cellsByCol == null || columnIndex1Based <= 0)
        {
            return string.Empty;
        }

        if (!cellsByCol.TryGetValue(columnIndex1Based, out Cell cell) || cell == null)
        {
            return string.Empty;
        }

        return GetCellRawText(cell, sstp).Trim();
    }

    private static void RewriteCustomerSheet(WorksheetPart wsp, IReadOnlyList<Customer> customers)
    {
        SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>() ?? wsp.Worksheet.AppendChild(new SheetData());
        foreach (Row r in sheetData.Elements<Row>().ToList())
        {
            r.Remove();
        }

        ReplaceFirstRowWithHeaders(wsp);
        Dictionary<string, int> headerMap = BuildHeaderMap(wsp, null);
        uint rowIndex = 2u;
        foreach (Customer customer in customers)
        {
            var row = new Row { RowIndex = rowIndex };
            AppendCustomerCells(row, rowIndex, headerMap, customer);
            sheetData.AppendChild(row);
            rowIndex++;
        }
    }

    private void WriteCustomerHeaderRowOnly()
    {
        using (SpreadsheetDocument doc = SpreadsheetDocument.Open(_workbookPath, true))
        {
            WorksheetPart wsp = GetWorksheetPartByName(doc.WorkbookPart, ExcelFileManager.CustomerSheetName);
            if (wsp != null)
            {
                ReplaceFirstRowWithHeaders(wsp);
                wsp.Worksheet.Save();
                doc.WorkbookPart.Workbook.Save();
            }
        }
    }

    private static void ReplaceFirstRowWithHeaders(WorksheetPart wsp)
    {
        SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>() ?? wsp.Worksheet.AppendChild(new SheetData());
        Row row1 = sheetData.Elements<Row>().FirstOrDefault(r => r.RowIndex == 1u);
        if (row1 != null)
        {
            row1.Remove();
        }

        row1 = new Row { RowIndex = 1u };
        for (int i = 0; i < ExcelFileManager.CustomerColumnHeaders.Length; i++)
        {
            row1.AppendChild(NewInlineTextCell(1u, i + 1, ExcelFileManager.CustomerColumnHeaders[i]));
        }

        sheetData.InsertAt(row1, 0);
    }

    private static void InsertHeaderRowAndShiftExistingRows(WorksheetPart wsp, SharedStringTablePart sstp)
    {
        SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>() ?? wsp.Worksheet.AppendChild(new SheetData());
        var existing = new List<Dictionary<int, string>>();
        foreach (Row row in sheetData.Elements<Row>().OrderBy(r => r.RowIndex?.Value ?? 0u))
        {
            uint ri = row.RowIndex?.Value ?? 0u;
            if (ri == 0u)
            {
                continue;
            }

            var cols = new Dictionary<int, string>();
            foreach (Cell cell in row.Elements<Cell>())
            {
                int col = ColumnIndexFromReference(cell.CellReference);
                cols[col] = GetCellRawText(cell, sstp);
            }

            existing.Add(cols);
        }

        foreach (Row r in sheetData.Elements<Row>().ToList())
        {
            r.Remove();
        }

        var headerRow = new Row { RowIndex = 1u };
        for (int i = 0; i < ExcelFileManager.CustomerColumnHeaders.Length; i++)
        {
            headerRow.AppendChild(NewInlineTextCell(1u, i + 1, ExcelFileManager.CustomerColumnHeaders[i]));
        }

        sheetData.AppendChild(headerRow);

        uint newRowIndex = 2u;
        foreach (Dictionary<int, string> cols in existing)
        {
            var dataRow = new Row { RowIndex = newRowIndex };
            foreach (KeyValuePair<int, string> kv in cols.OrderBy(k => k.Key))
            {
                dataRow.AppendChild(NewInlineTextCell(newRowIndex, kv.Key, kv.Value ?? string.Empty));
            }

            sheetData.AppendChild(dataRow);
            newRowIndex++;
        }
    }

    private void CreateWorkbookWithEmptySheet(string sheetName)
    {
        using (SpreadsheetDocument doc = SpreadsheetDocument.Create(_workbookPath, SpreadsheetDocumentType.Workbook))
        {
            WorkbookPart wbPart = doc.AddWorkbookPart();
            wbPart.Workbook = new Workbook();
            WorksheetPart wsp = wbPart.AddNewPart<WorksheetPart>();
            wsp.Worksheet = new Worksheet(new SheetData());
            uint sheetId = 1u;
            Sheet sheet = new Sheet
            {
                Id = wbPart.GetIdOfPart(wsp),
                SheetId = sheetId,
                Name = sheetName
            };
            wbPart.Workbook.AppendChild(new Sheets(sheet));
            wbPart.Workbook.Save();
        }
    }

    private static WorksheetPart GetWorksheetPartByName(WorkbookPart wbPart, string sheetName)
    {
        if (wbPart?.Workbook?.Sheets == null)
        {
            return null;
        }

        foreach (Sheet s in wbPart.Workbook.Sheets.Elements<Sheet>())
        {
            if (string.Equals(s.Name, sheetName, StringComparison.OrdinalIgnoreCase))
            {
                return (WorksheetPart)wbPart.GetPartById(s.Id);
            }
        }

        return null;
    }

    private static WorksheetPart GetOrCreateWorksheetPart(WorkbookPart wbPart, string sheetName)
    {
        WorksheetPart existing = GetWorksheetPartByName(wbPart, sheetName);
        if (existing != null)
        {
            return existing;
        }

        WorksheetPart wsp = wbPart.AddNewPart<WorksheetPart>();
        wsp.Worksheet = new Worksheet(new SheetData());
        uint maxId = 1u;
        if (wbPart.Workbook.Sheets != null)
        {
            foreach (Sheet s in wbPart.Workbook.Sheets.Elements<Sheet>())
            {
                if (s.SheetId != null && s.SheetId.Value >= maxId)
                {
                    maxId = s.SheetId.Value + 1u;
                }
            }
        }
        else
        {
            wbPart.Workbook.AppendChild(new Sheets());
        }

        var newSheet = new Sheet
        {
            Id = wbPart.GetIdOfPart(wsp),
            SheetId = maxId,
            Name = sheetName
        };
        wbPart.Workbook.Sheets.Append(newSheet);
        return wsp;
    }

    private static Dictionary<string, int> BuildHeaderMap(WorksheetPart wsp, SharedStringTablePart sstp)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>();
        if (sheetData == null)
        {
            return map;
        }

        Row first = sheetData.Elements<Row>().OrderBy(r => r.RowIndex?.Value ?? uint.MaxValue).FirstOrDefault();
        if (first == null)
        {
            return map;
        }

        foreach (Cell cell in first.Elements<Cell>().OrderBy(c => ColumnIndexFromReference(c.CellReference)))
        {
            string name = GetCellRawText(cell, sstp).Trim();
            if (name.Length > 0)
            {
                map[name] = ColumnIndexFromReference(cell.CellReference);
            }
        }

        return map;
    }

    private static string GetCellAtRow(WorksheetPart wsp, SharedStringTablePart sstp, IReadOnlyDictionary<string, int> headerMap, uint rowIndex, string columnName)
    {
        if (!headerMap.TryGetValue(columnName, out int col))
        {
            return string.Empty;
        }

        SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>();
        Row row = sheetData?.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
        if (row == null)
        {
            return string.Empty;
        }

        Cell cell = FindCellInRow(row, col);
        return cell != null ? GetCellRawText(cell, sstp).Trim() : string.Empty;
    }

    private static string GetCellAtRowAny(
        WorksheetPart wsp,
        SharedStringTablePart sstp,
        IReadOnlyDictionary<string, int> headerMap,
        uint rowIndex,
        params string[] columnNames)
    {
        if (columnNames == null)
        {
            return string.Empty;
        }

        foreach (string columnName in columnNames)
        {
            string value = GetCellAtRow(wsp, sstp, headerMap, rowIndex, columnName);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return string.Empty;
    }

    private static void SetCellPrefer(
        Row row,
        uint rowIndex,
        IReadOnlyDictionary<string, int> headerMap,
        string columnName,
        string value)
    {
        if (headerMap.ContainsKey(columnName))
        {
            SetCellOnRow(row, rowIndex, headerMap, columnName, value);
        }
    }

    private static void SetCellPrefer(
        Row row,
        uint rowIndex,
        IReadOnlyDictionary<string, int> headerMap,
        string primaryColumn,
        string alternateColumn,
        string value)
    {
        if (headerMap.ContainsKey(primaryColumn))
        {
            SetCellOnRow(row, rowIndex, headerMap, primaryColumn, value);
            return;
        }

        if (headerMap.ContainsKey(alternateColumn))
        {
            SetCellOnRow(row, rowIndex, headerMap, alternateColumn, value);
        }
    }

    private static uint GetLastUsedRowIndex(WorksheetPart wsp, SharedStringTablePart sstp, IReadOnlyDictionary<string, int> headerMap)
    {
        if (headerMap.Count == 0)
        {
            return 1u;
        }

        IEnumerable<int> cols = headerMap.Values;
        uint max = 1u;
        SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>();
        if (sheetData == null)
        {
            return max;
        }

        foreach (Row row in sheetData.Elements<Row>())
        {
            uint ri = row.RowIndex?.Value ?? 0u;
            if (ri < 2u)
            {
                continue;
            }

            foreach (int col in cols)
            {
                Cell cell = FindCellInRow(row, col);
                string text = cell != null ? GetCellRawText(cell, sstp) : string.Empty;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    if (ri > max)
                    {
                        max = ri;
                    }

                    break;
                }
            }
        }

        return max;
    }

    private static Cell FindCellInRow(Row row, int columnIndex1Based)
    {
        foreach (Cell cell in row.Elements<Cell>())
        {
            if (ColumnIndexFromReference(cell.CellReference) == columnIndex1Based)
            {
                return cell;
            }
        }

        return null;
    }

    /// <summary>Plain string cell (no styles, shared strings, or inline formatting).</summary>
    private static Cell NewInlineTextCell(uint rowIndex, int columnIndex1Based, string text)
    {
        string reference = GetColumnName(columnIndex1Based) + rowIndex.ToString(CultureInfo.InvariantCulture);
        return new Cell
        {
            CellReference = reference,
            DataType = CellValues.String,
            CellValue = new CellValue(text ?? string.Empty)
        };
    }

    /// <summary>Plain cell text only (shared string index, inline string, or raw value). No formatting evaluation.</summary>
    private static string GetCellRawText(Cell cell, SharedStringTablePart sstp)
    {
        if (cell == null)
        {
            return string.Empty;
        }

        if (cell.DataType == null)
        {
            return cell.CellValue?.Text ?? string.Empty;
        }

        if (cell.DataType == CellValues.SharedString)
        {
            if (cell.CellValue == null || sstp?.SharedStringTable == null)
            {
                return string.Empty;
            }

            if (!int.TryParse(cell.CellValue.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int idx))
            {
                return string.Empty;
            }

            SharedStringItem[] items = sstp.SharedStringTable.Elements<SharedStringItem>().ToArray();
            if (idx < 0 || idx >= items.Length)
            {
                return string.Empty;
            }

            return SharedStringItemText(items[idx]);
        }

        if (cell.DataType == CellValues.InlineString)
        {
            if (cell.InlineString == null)
            {
                return string.Empty;
            }

            if (cell.InlineString.Text != null)
            {
                return cell.InlineString.Text.Text ?? string.Empty;
            }

            return string.Concat(cell.InlineString.Descendants<Text>().Select(t => t.Text ?? string.Empty));
        }

        if (cell.DataType == CellValues.String || cell.DataType == CellValues.Number)
        {
            return cell.CellValue?.Text ?? string.Empty;
        }

        if (cell.DataType == CellValues.Boolean)
        {
            return cell.CellValue?.Text == "1" ? "TRUE" : "FALSE";
        }

        return cell.CellValue?.Text ?? string.Empty;
    }

    private static string SharedStringItemText(SharedStringItem item)
    {
        if (item == null)
        {
            return string.Empty;
        }

        if (item.Text != null)
        {
            return item.Text.Text ?? string.Empty;
        }

        return string.Concat(item.Descendants<Text>().Select(t => t.Text ?? string.Empty));
    }

    private static int ColumnIndexFromReference(StringValue reference)
    {
        string r = reference?.Value;
        if (string.IsNullOrEmpty(r))
        {
            return 0;
        }

        int col = 0;
        foreach (char c in r)
        {
            if (c >= 'A' && c <= 'Z')
            {
                col = col * 26 + (c - 'A' + 1);
            }
            else if (c >= 'a' && c <= 'z')
            {
                col = col * 26 + (c - 'a' + 1);
            }
            else
            {
                break;
            }
        }

        return col;
    }

    private static void AppendCustomerCells(Row row, uint rowIndex, IReadOnlyDictionary<string, int> headerMap, Customer customer)
    {
        string fullName = (customer.FullName ?? string.Empty).Trim();
        if (fullName.Length == 0)
        {
            fullName = customer.DisplayName;
        }

        string idNumber = (customer.IDNumber ?? string.Empty).Trim();
        if (idNumber.Length == 0)
        {
            idNumber = customer.CustomerID;
        }

        SetCellOnRow(row, rowIndex, headerMap, "CustomerID", customer.CustomerID);
        SetCellOnRow(row, rowIndex, headerMap, "FullName", fullName);
        SetCellOnRow(row, rowIndex, headerMap, "IDNumber", idNumber);
        SetCellOnRow(row, rowIndex, headerMap, "Phone", customer.Phone);
        SetCellOnRow(row, rowIndex, headerMap, "Email", customer.Email);
    }

    private static void NotifyCustomersChanged()
    {
        CustomersChanged?.Invoke(null, EventArgs.Empty);
    }

    private static void ReplaceCustomerRow(
        WorksheetPart wsp,
        SharedStringTablePart sstp,
        Dictionary<string, int> headerMap,
        uint rowIndex,
        Customer customer)
    {
        SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>();
        Row row = sheetData?.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
        if (row == null)
        {
            throw new InvalidOperationException("Customer row is missing.");
        }

        foreach (Cell cell in row.Elements<Cell>().ToList())
        {
            cell.Remove();
        }

        AppendCustomerCells(row, rowIndex, headerMap, customer);
    }

    private static void SetCellOnRow(Row row, uint rowIndex, IReadOnlyDictionary<string, int> headerMap, string columnName, string value)
    {
        if (!headerMap.TryGetValue(columnName, out int col) || col <= 0)
        {
            return;
        }

        InsertOrReplaceCellInRow(row, rowIndex, col, value);
    }

    /// <summary>
    /// Inserts a cell into a row while keeping the row's cells in ascending column order.
    /// Existing cell at the same column is replaced. Required by OpenXML / Excel readers.
    /// </summary>
    private static void InsertOrReplaceCellInRow(Row row, uint rowIndex, int col, string value)
    {
        Cell newCell = NewInlineTextCell(rowIndex, col, value ?? string.Empty);

        foreach (Cell existing in row.Elements<Cell>().ToList())
        {
            int existingCol = ColumnIndexFromReference(existing.CellReference);
            if (existingCol == col)
            {
                row.ReplaceChild(newCell, existing);
                return;
            }

            if (existingCol > col)
            {
                row.InsertBefore(newCell, existing);
                return;
            }
        }

        row.AppendChild(newCell);
    }

    private static uint? FindEmployeeRowIndexByEmail(
        WorksheetPart wsp,
        SharedStringTablePart sstp,
        IReadOnlyDictionary<string, int> headerMap,
        string email)
    {
        uint lastRow = GetLastUsedRowIndex(wsp, sstp, headerMap);
        for (uint r = 2u; r <= lastRow; r++)
        {
            string rowEmail = GetCellAtRow(wsp, sstp, headerMap, r, "Email");
            if (string.Equals(rowEmail?.Trim(), email, StringComparison.OrdinalIgnoreCase))
            {
                return r;
            }
        }

        return null;
    }

    private static uint? FindCustomerRowIndex(
        WorksheetPart wsp,
        SharedStringTablePart sstp,
        IReadOnlyDictionary<string, int> headerMap,
        string customerId)
    {
        uint lastRow = GetLastUsedRowIndex(wsp, sstp, headerMap);
        for (uint r = 2u; r <= lastRow; r++)
        {
            string id = GetCellAtRow(wsp, sstp, headerMap, r, "CustomerID");
            if (string.IsNullOrWhiteSpace(id))
            {
                id = GetCellAtRow(wsp, sstp, headerMap, r, "IDNumber");
            }

            if (string.Equals(id?.Trim(), customerId, StringComparison.OrdinalIgnoreCase))
            {
                return r;
            }
        }

        return null;
    }

    private static void RemoveRow(WorksheetPart wsp, uint rowIndex)
    {
        SheetData sheetData = wsp.Worksheet.GetFirstChild<SheetData>();
        Row row = sheetData?.Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
        row?.Remove();
    }

    private static void SplitLegacyFullName(string fullName, out string firstName, out string lastName)
    {
        fullName = (fullName ?? string.Empty).Trim();
        int space = fullName.LastIndexOf(' ');
        if (space > 0)
        {
            firstName = fullName.Substring(0, space).Trim();
            lastName = fullName.Substring(space + 1).Trim();
            return;
        }

        firstName = fullName;
        lastName = string.Empty;
    }

    private static string GetColumnName(int columnNumber1Based)
    {
        int dividend = columnNumber1Based;
        string columnName = string.Empty;
        while (dividend > 0)
        {
            int modulo = (dividend - 1) % 26;
            columnName = Convert.ToChar('A' + modulo) + columnName;
            dividend = (dividend - modulo) / 26;
        }

        return columnName;
    }
}
