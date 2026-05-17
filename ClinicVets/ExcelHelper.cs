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

    public IReadOnlyList<Customer> ReadCustomers()
    {
        EnsureWorkbookAndCustomerLayout();
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

        EnsureWorkbookAndCustomerLayout();
        using (SpreadsheetDocument doc = SpreadsheetDocument.Open(_workbookPath, true))
        {
            WorksheetPart wsp = GetWorksheetPartByName(doc.WorkbookPart, ExcelFileManager.CustomerSheetName);
            if (wsp == null)
            {
                throw new InvalidOperationException("Customer worksheet is missing.");
            }

            SharedStringTablePart sstp = doc.WorkbookPart.SharedStringTablePart;
            Dictionary<string, int> headerMap = BuildHeaderMap(wsp, sstp);
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

        EnsureWorkbookAndCustomerLayout();
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

        EnsureWorkbookAndCustomerLayout();
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
        EnsureWorkbookAndEmployeeLayout();
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
            Dictionary<string, int> headerMap = BuildHeaderMap(wsp, sstp);
            if (headerMap.Count == 0)
            {
                return list;
            }

            uint lastRow = GetLastUsedRowIndex(wsp, sstp, headerMap);
            for (uint r = 2u; r <= lastRow; r++)
            {
                string employeeId = GetCellAtRow(wsp, sstp, headerMap, r, "EmployeeID");
                string username = GetCellAtRow(wsp, sstp, headerMap, r, "Username");
                string password = GetCellAtRow(wsp, sstp, headerMap, r, "Password");
                string email = GetCellAtRow(wsp, sstp, headerMap, r, "Email");
                string nationalId = GetCellAtRow(wsp, sstp, headerMap, r, "NationalID");
                string role = GetCellAtRow(wsp, sstp, headerMap, r, "Role");

                if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
                {
                    continue;
                }

                list.Add(new Employee
                {
                    EmployeeID = employeeId,
                    Username = username,
                    Password = password,
                    Email = email,
                    NationalID = nationalId,
                    Role = role
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

        EnsureWorkbookAndEmployeeLayout();
        using (SpreadsheetDocument doc = SpreadsheetDocument.Open(_workbookPath, true))
        {
            WorksheetPart wsp = GetWorksheetPartByName(doc.WorkbookPart, ExcelFileManager.EmployeeSheetName);
            if (wsp == null)
            {
                throw new InvalidOperationException("Employees worksheet is missing.");
            }

            SharedStringTablePart sstp = doc.WorkbookPart.SharedStringTablePart;
            Dictionary<string, int> headerMap = BuildHeaderMap(wsp, sstp);
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

    public Employee TryAuthenticateEmployee(string username, string password)
    {
        username = (username ?? string.Empty).Trim();
        password = password ?? string.Empty;
        if (username.Length == 0 || password.Length == 0)
        {
            return null;
        }

        foreach (Employee employee in ReadEmployees())
        {
            if (string.Equals(employee.Username?.Trim(), username, StringComparison.OrdinalIgnoreCase)
                && string.Equals(employee.Password ?? string.Empty, password, StringComparison.Ordinal))
            {
                return employee;
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

        EnsureWorkbookAndEmployeeLayout();
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
            Dictionary<string, int> headerMap = BuildHeaderMap(wsp, sstp);
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

    private void EnsureWorkbookAndEmployeeLayout()
    {
        string directory = Path.GetDirectoryName(_workbookPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(_workbookPath))
        {
            CreateWorkbookWithEmptySheet(ExcelFileManager.EmployeeSheetName);
            WriteEmployeeHeaderRowOnly();
            return;
        }

        using (SpreadsheetDocument doc = SpreadsheetDocument.Open(_workbookPath, true))
        {
            WorksheetPart wsp = GetOrCreateWorksheetPart(doc.WorkbookPart, ExcelFileManager.EmployeeSheetName);
            Dictionary<string, int> map = BuildHeaderMap(wsp, doc.WorkbookPart.SharedStringTablePart);
            if (map.Count == 0)
            {
                ReplaceEmployeeHeaderRow(wsp);
            }

            doc.WorkbookPart.Workbook.Save();
            wsp.Worksheet.Save();
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

    private static void AppendEmployeeCells(Row row, uint rowIndex, IReadOnlyDictionary<string, int> headerMap, Employee employee)
    {
        SetCellOnRow(row, rowIndex, headerMap, "EmployeeID", employee.EmployeeID);
        SetCellOnRow(row, rowIndex, headerMap, "Username", employee.Username);
        SetCellOnRow(row, rowIndex, headerMap, "Password", employee.Password);
        SetCellOnRow(row, rowIndex, headerMap, "Email", employee.Email);
        SetCellOnRow(row, rowIndex, headerMap, "NationalID", employee.NationalID);
        SetCellOnRow(row, rowIndex, headerMap, "Role", employee.Role);
    }

    private void EnsureWorkbookAndCustomerLayout()
    {
        string directory = Path.GetDirectoryName(_workbookPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(_workbookPath))
        {
            CreateWorkbookWithEmptySheet(ExcelFileManager.CustomerSheetName);
            WriteCustomerHeaderRowOnly();
            return;
        }

        using (SpreadsheetDocument doc = SpreadsheetDocument.Open(_workbookPath, true))
        {
            WorksheetPart wsp = GetOrCreateWorksheetPart(doc.WorkbookPart, ExcelFileManager.CustomerSheetName);
            SharedStringTablePart sstp = doc.WorkbookPart.SharedStringTablePart;
            Dictionary<string, int> map = BuildHeaderMap(wsp, sstp);
            if (map.Count == 0)
            {
                ReplaceFirstRowWithHeaders(wsp);
            }
            else if (!map.ContainsKey("FirstName") || !map.ContainsKey("Address"))
            {
                IReadOnlyList<Customer> existing = ReadCustomersCore(wsp, sstp, map);
                RewriteCustomerSheet(wsp, existing);
            }

            doc.WorkbookPart.Workbook.Save();
            wsp.Worksheet.Save();
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

        uint lastRow = GetLastUsedRowIndex(wsp, sstp, headerMap);
        for (uint r = 2u; r <= lastRow; r++)
        {
            string email = GetCellAtRow(wsp, sstp, headerMap, r, "Email");
            string phone = GetCellAtRow(wsp, sstp, headerMap, r, "Phone");
            string address = GetCellAtRow(wsp, sstp, headerMap, r, "Address");
            string lastName = GetCellAtRow(wsp, sstp, headerMap, r, "LastName");
            string firstName = GetCellAtRow(wsp, sstp, headerMap, r, "FirstName");
            string customerId = GetCellAtRow(wsp, sstp, headerMap, r, "CustomerID");
            string idNumber = GetCellAtRow(wsp, sstp, headerMap, r, "IDNumber");
            string fullName = GetCellAtRow(wsp, sstp, headerMap, r, "FullName");

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
                string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(customerId))
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

    private static Cell NewInlineTextCell(uint rowIndex, int columnIndex1Based, string text)
    {
        string reference = GetColumnName(columnIndex1Based) + rowIndex.ToString(CultureInfo.InvariantCulture);
        return new Cell
        {
            CellReference = reference,
            DataType = CellValues.InlineString,
            InlineString = new InlineString(new Text(text ?? string.Empty))
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
        SetCellOnRow(row, rowIndex, headerMap, "Email", customer.Email);
        SetCellOnRow(row, rowIndex, headerMap, "Phone", customer.Phone);
        SetCellOnRow(row, rowIndex, headerMap, "Address", customer.Address);
        SetCellOnRow(row, rowIndex, headerMap, "LastName", customer.LastName);
        SetCellOnRow(row, rowIndex, headerMap, "FirstName", customer.FirstName);
        SetCellOnRow(row, rowIndex, headerMap, "CustomerID", customer.CustomerID);
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
        int col = headerMap.TryGetValue(columnName, out int mapped)
            ? mapped
            : Array.IndexOf(ExcelFileManager.CustomerColumnHeaders, columnName) + 1;
        if (col <= 0)
        {
            return;
        }

        row.AppendChild(NewInlineTextCell(rowIndex, col, value ?? string.Empty));
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
