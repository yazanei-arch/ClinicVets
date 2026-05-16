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
            if (headerMap.Count == 0)
            {
                return list;
            }

            uint lastRow = GetLastUsedRowIndex(wsp, sstp, headerMap);
            for (uint r = 2u; r <= lastRow; r++)
            {
                string email = GetCellAtRow(wsp, sstp, headerMap, r, "Email");
                string phone = GetCellAtRow(wsp, sstp, headerMap, r, "Phone");
                string idNumber = GetCellAtRow(wsp, sstp, headerMap, r, "IDNumber");
                string fullName = GetCellAtRow(wsp, sstp, headerMap, r, "FullName");
                string customerId = GetCellAtRow(wsp, sstp, headerMap, r, "CustomerID");
                if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone) &&
                    string.IsNullOrWhiteSpace(idNumber) && string.IsNullOrWhiteSpace(fullName) &&
                    string.IsNullOrWhiteSpace(customerId))
                {
                    continue;
                }

                list.Add(new Customer
                {
                    Email = email,
                    Phone = phone,
                    IDNumber = idNumber,
                    FullName = fullName,
                    CustomerID = customerId
                });
            }
        }

        return list;
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
            row.AppendChild(NewInlineTextCell(nextRow, 1, customer.Email ?? string.Empty));
            row.AppendChild(NewInlineTextCell(nextRow, 2, customer.Phone ?? string.Empty));
            row.AppendChild(NewInlineTextCell(nextRow, 3, customer.IDNumber ?? string.Empty));
            row.AppendChild(NewInlineTextCell(nextRow, 4, customer.FullName ?? string.Empty));
            row.AppendChild(NewInlineTextCell(nextRow, 5, customer.CustomerID ?? string.Empty));
            sheetData.AppendChild(row);

            wsp.Worksheet.Save();
            doc.WorkbookPart.Workbook.Save();
        }
    }

    private void EnsureWorkbookAndCustomerLayout()
    {
        MessageBox.Show("Excel path: " + _workbookPath);
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
            else if (!map.ContainsKey("Email"))
            {
                InsertHeaderRowAndShiftExistingRows(wsp, sstp);
            }

            doc.WorkbookPart.Workbook.Save();
            wsp.Worksheet.Save();
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
