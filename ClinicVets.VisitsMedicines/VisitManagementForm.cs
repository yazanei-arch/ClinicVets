using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using ClosedXML.Excel;

namespace ClinicVets.VisitsMedicines
{
    public partial class VisitManagementForm : Form
    {
        private const string BackgroundFileName = "background.png";

        private readonly string _initialPetId;
        private Button _btnBack;

        public VisitManagementForm()
            : this(null)
        {
        }

        public VisitManagementForm(string petId)
        {
            _initialPetId = string.IsNullOrWhiteSpace(petId) ? null : petId.Trim();
            InitializeComponent();
            FormClosed += VisitManagementForm_FormClosed;
        }

        private string filePath => ExcelFileManager.FilePath;

        private void VisitManagementForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClosed -= VisitManagementForm_FormClosed;
            ReleasePictureBoxBackground();
        }

        private void VisitManagementForm_Load(object sender, EventArgs e)
        {
            ApplyBackgroundFromImagesFolder();
            ClinicFormLayout.ApplyStandard(this);
            ApplyBackgroundFromImagesFolder();
            if (pictureBox1 != null)
            {
                pictureBox1.SendToBack();
            }

            EnsureBackButton();

            SafeLoadVeterinarians();

            // Guarantee the vaccine reminder handler is wired up even if the Designer
            // file is regenerated. Detach first to avoid double-invocation.
            cmbPets.SelectedIndexChanged -= cmbPets_SelectedIndexChanged;
            cmbPets.SelectedIndexChanged += cmbPets_SelectedIndexChanged;

            int initialIndexToSelect = -1;

            try
            {
                dtpVisitDate.MaxDate = DateTime.Today;

                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet("Pets");
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                    cmbPets.Items.Clear(); // Clear any old items first
                    foreach (var row in rows)
                    {
                        string petId = row.Cell(1).GetString().Trim();
                        string petName = row.Cell(2).GetString().Trim();
                        string display = string.IsNullOrEmpty(petId)
                            ? petName
                            : petId + " — " + petName;
                        cmbPets.Items.Add(display);
                    }

                    if (!string.IsNullOrEmpty(_initialPetId))
                    {
                        for (int i = 0; i < cmbPets.Items.Count; i++)
                        {
                            string item = cmbPets.Items[i]?.ToString() ?? string.Empty;
                            if (item.StartsWith(_initialPetId, StringComparison.OrdinalIgnoreCase))
                            {
                                initialIndexToSelect = i;
                                break;
                            }
                        }
                    }

                    PopulateMedicinesCheckList(workbook);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading pets: " + ex.Message);
            }

            // Apply the initial selection AFTER the workbook is closed so the vaccine
            // check handler can re-open the file without contending with this Load.
            if (initialIndexToSelect >= 0)
            {
                cmbPets.SelectedIndex = initialIndexToSelect;
            }
        }

        private void btnSaveVisit_Click(object sender, EventArgs e)
        {
            lblPetError.Visible = false;
            lblVetEmpty.Visible = false;
            lblVetInvalid.Visible = false;
            lblReasonError.Visible = false;
            lblTimeError.Visible = false;

            bool isValid = true;

            if (cmbPets.SelectedItem == null)
            {
                lblPetError.Visible = true;
                isValid = false;
            }

            string vetName = txtVetName.Text.Trim();

            if (string.IsNullOrWhiteSpace(vetName))
            {
                lblVetEmpty.Visible = true;
                isValid = false;
            }
            else if (!vetName.All(c => char.IsLetter(c) || c == ' '))
            {
                lblVetInvalid.Visible = true;
                isValid = false;
            }

            if (dtpVisitDate.Value.Date == DateTime.Today && dtpVisitTime.Value.TimeOfDay > DateTime.Now.TimeOfDay)
            {
                lblTimeError.Visible = true;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtReason.Text))
            {
                lblReasonError.Visible = true;
                isValid = false;
            }

            if (!isValid)
            {
                return;
            }

            try
            {
                string filePath = ExcelFileManager.FilePath;

                using (var workbook = new XLWorkbook(filePath))
                {
                    var visitSheet = workbook.Worksheet("Visits");
                    int lastRow = visitSheet.LastRowUsed().RowNumber() + 1;

                    // Combine checked medicines into one text string
                    string selectedMeds = string.Join(", ", clbMedicines.CheckedItems.Cast<string>());
                    string price = lblTotalCost.Text;
                    var words = price.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string result = words.Length <= 2
                        ? string.Join(" ", words)
                        : string.Join(" ", words.Skip(words.Length - 2));
                
                    visitSheet.Cell(lastRow, 1).Value = new Random().Next(1000, 9999);   // VisitID (A)
                    visitSheet.Cell(lastRow, 2).Value = cmbPets.SelectedItem.ToString(); // PetID/Name (B)
                    visitSheet.Cell(lastRow, 3).Value = dtpVisitDate.Value.ToShortDateString(); // VisitDate (C)
                    visitSheet.Cell(lastRow, 4).Value = dtpVisitTime.Value.ToShortTimeString();
                    visitSheet.Cell(lastRow, 5).Value = txtReason.Text;                  // Reason (E)
                    visitSheet.Cell(lastRow, 6).Value = rtbSummary.Text;                 // Diagnosis (F)
                    visitSheet.Cell(lastRow, 7).Value = selectedMeds;                    // Medicines (G)
                    visitSheet.Cell(lastRow, 8).Value = txtVetName.Text; // VetName (H)
                    visitSheet.Cell(lastRow, 9).Value = result;               // TotalPrice (I)

                    var inventoryResult = DecrementMedicineInventory(workbook);

                    if (lblVaccineAlert.Visible)
                    {
                        var petSheet = workbook.Worksheet("Pets");
                        string selectedPet = cmbPets.SelectedItem.ToString();
                        var petRow = petSheet.RangeUsed().RowsUsed().FirstOrDefault(r => r.Cell(2).Value.ToString() == selectedPet);
                        if (petRow != null)
                        {
                            petRow.Cell(8).Value = DateTime.Now.ToShortDateString(); // Set Column A to today
                        }
                    }

                    workbook.Save();

                    foreach (string missed in inventoryResult.NotFound)
                    {
                        MessageBox.Show(
                            "Medicine not found in inventory: " + missed,
                            "Inventory",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }

                    if (inventoryResult.Decremented > 0)
                    {
                        MessageBox.Show(
                            "Medicine quantities updated.",
                            "Inventory",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }

                ReloadMedicinesList();

                MessageBox.Show("Saved successfully!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Close the Excel file before saving!\n" + ex.Message);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.ActiveControl = null;
            clbMedicines.ClearSelected();
        }

        private enum VaccineCheckResult
        {
            NoMatch,
            Invalid,
            UpToDate,
            Overdue
        }

        private void cmbPets_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reset the visual flag first; the visit-save logic depends on this label's
            // Visible state to decide whether to refresh LastVaccineDate on save, so it
            // must always reflect the current pet selection.
            if (lblVaccineAlert != null)
            {
                lblVaccineAlert.Visible = false;
            }

            if (cmbPets.SelectedItem == null)
            {
                return;
            }

            string selectedDisplay = cmbPets.SelectedItem.ToString() ?? string.Empty;
            SplitPetDisplay(selectedDisplay, out string selectedCode, out string selectedName);

            VaccineCheckResult status = VaccineCheckResult.NoMatch;

            try
            {
                string path = ExcelFileManager.FilePath;
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var workbook = new XLWorkbook(fs))
                {
                    if (!workbook.Worksheets.Contains("Pets"))
                    {
                        return;
                    }

                    var sheet = workbook.Worksheet("Pets");
                    var range = sheet.RangeUsed();
                    if (range == null)
                    {
                        return;
                    }

                    var header = range.FirstRowUsed();
                    if (header == null)
                    {
                        return;
                    }

                    int nameCol = FindHeaderColumn(header, "Pet Name", "PetName");
                    int codeCol = FindHeaderColumn(header, "Pet Code", "PetCode", "PetID");
                    int dateCol = FindHeaderColumn(header, "Last Vaccine Date", "LastVaccineDate", "Last Vaccine");

                    if (dateCol <= 0)
                    {
                        return;
                    }

                    foreach (var row in range.RowsUsed())
                    {
                        if (row.RowNumber() == header.RowNumber())
                        {
                            continue;
                        }

                        string rowCode = codeCol > 0 ? row.Cell(codeCol).GetString().Trim() : string.Empty;
                        string rowName = nameCol > 0 ? row.Cell(nameCol).GetString().Trim() : string.Empty;

                        bool matches =
                            (!string.IsNullOrEmpty(selectedCode)
                                && string.Equals(rowCode, selectedCode, StringComparison.OrdinalIgnoreCase))
                            || (!string.IsNullOrEmpty(selectedName)
                                && string.Equals(rowName, selectedName, StringComparison.OrdinalIgnoreCase))
                            || string.Equals(rowName, selectedDisplay.Trim(), StringComparison.OrdinalIgnoreCase);

                        if (!matches)
                        {
                            continue;
                        }

                        if (TryParsePetDate(row.Cell(dateCol), out DateTime lastVaccine))
                        {
                            status = lastVaccine.Date.AddYears(1) <= DateTime.Today
                                ? VaccineCheckResult.Overdue
                                : VaccineCheckResult.UpToDate;
                        }
                        else
                        {
                            status = VaccineCheckResult.Invalid;
                        }

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking vaccine: " + ex.Message);
                status = VaccineCheckResult.Invalid;
            }

            if (status == VaccineCheckResult.Overdue)
            {
                lblVaccineAlert.Text = "Annual Vaccine Required!";
                lblVaccineAlert.Visible = true;

                for (int i = 0; i < clbMedicines.Items.Count; i++)
                {
                    string item = clbMedicines.Items[i]?.ToString() ?? string.Empty;
                    if (item.IndexOf("Vaccine", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        clbMedicines.SetItemChecked(i, true);
                    }
                }

                ShowVaccineMessage("\u26A0 This pet needs an annual vaccine.", MessageBoxIcon.Warning);
            }
            else if (status == VaccineCheckResult.UpToDate)
            {
                ShowVaccineMessage("\u2713 Vaccination is up to date.", MessageBoxIcon.Information);
            }
            else if (status == VaccineCheckResult.Invalid)
            {
                ShowVaccineMessage("No valid vaccine date found for this pet.", MessageBoxIcon.Warning);
            }
        }

        private void ShowVaccineMessage(string message, MessageBoxIcon icon)
        {
            // Show synchronously: BeginInvoke can be skipped on rare edge cases where
            // the dropdown is closing, but the alert must always reach the user.
            MessageBox.Show(
                this,
                message,
                "Vaccine Reminder",
                MessageBoxButtons.OK,
                icon);
        }

        private static void SplitPetDisplay(string display, out string code, out string name)
        {
            code = string.Empty;
            name = string.Empty;
            if (string.IsNullOrWhiteSpace(display))
            {
                return;
            }

            int sep = display.IndexOf('\u2014'); // em dash used when building items
            if (sep < 0)
            {
                sep = display.IndexOf(" - ", StringComparison.Ordinal);
            }

            if (sep >= 0)
            {
                code = display.Substring(0, sep).Trim();
                string rest = display.Substring(sep + 1);
                name = rest.TrimStart('\u2014', '-', ' ').Trim();
            }
            else
            {
                name = display.Trim();
            }
        }

        private static int FindHeaderColumn(IXLRangeRow headerRow, params string[] candidates)
        {
            if (headerRow == null || candidates == null || candidates.Length == 0)
            {
                return 0;
            }

            foreach (var cell in headerRow.CellsUsed())
            {
                string raw = (cell.Value.ToString() ?? string.Empty).Trim();
                foreach (var candidate in candidates)
                {
                    if (string.Equals(raw, candidate, StringComparison.OrdinalIgnoreCase))
                    {
                        return cell.Address.ColumnNumber;
                    }
                }
            }

            return 0;
        }

        private static bool TryParsePetDate(IXLCell cell, out DateTime result)
        {
            result = default;
            if (cell == null)
            {
                return false;
            }

            try
            {
                // GetDateTime() throws for empty cells or text values that aren't dates;
                // we catch and fall through to string parsing below.
                result = cell.GetDateTime();
                if (result != default)
                {
                    return true;
                }
            }
            catch
            {
            }

            string raw = (cell.Value.ToString() ?? string.Empty).Trim();
            if (raw.Length == 0)
            {
                return false;
            }

            if (DateTime.TryParse(raw, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out result))
            {
                return true;
            }
            if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
            {
                return true;
            }
            if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out double oa))
            {
                try
                {
                    result = DateTime.FromOADate(oa);
                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        private void clbMedicines_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string itemName = clbMedicines.Items[e.Index].ToString();
            string filePath = ExcelFileManager.FilePath;

            // 1. STOCK CHECK (We keep this so they can't buy empty stock)
            if (e.NewValue == CheckState.Checked)
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var workbook = new XLWorkbook(fs))
                {
                    var medSheet = workbook.Worksheet("Medicines");
                    var row = medSheet.RangeUsed().RowsUsed().FirstOrDefault(r => r.Cell(2).Value.ToString() == itemName);

                    if (row != null)
                    {
                        int stock = 0;
                        int.TryParse(row.Cell(1).Value.ToString(), out stock);

                        if (stock <= 0)
                        {
                            MessageBox.Show(itemName + " is out of stock!");
                            e.NewValue = CheckState.Unchecked;
                            return;
                        }
                    }
                }
            }

            // 2. PRICE CALCULATION (With Base Fee)
            this.BeginInvoke(new Action(() =>
            {
                // ---> BASE VISIT FEE: Start at 100 NIS regardless of medicine state
                double total = 100;

                try
                {
                    if (File.Exists(filePath))
                    {
                        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var workbook = new XLWorkbook(fs))
                        {
                            if (workbook.Worksheets.Contains("Medicines"))
                            {
                                var medSheet = workbook.Worksheet("Medicines");
                                var range = medSheet.RangeUsed();
                                if (range != null)
                                {
                                    int priceCol = FindMedicinePriceColumn(range);
                                    if (priceCol > 0)
                                    {
                                        var dataRows = range.RowsUsed().Skip(1).ToList();
                                        foreach (var item in clbMedicines.CheckedItems)
                                        {
                                            string medText = (item?.ToString() ?? string.Empty).Trim();
                                            if (medText.Length == 0) continue;

                                            var matched = FindMedicineRowByCheckboxText(dataRows, medText);
                                            if (matched != null)
                                            {
                                                total += SafeParseMedicinePrice(matched.Cell(priceCol));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // Never crash the UI; total remains the base fee plus whatever has been parsed.
                }

                lblTotalCost.Text = "Total Cost: " + total + " NIS";
            }));
        }

        private double GetPriceFromExcel(string medName)
        {
            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var sheet = workbook.Worksheet("Medicines");
                    var range = sheet.RangeUsed();
                    if (range == null) return 0;

                    int priceCol = FindMedicinePriceColumn(range);
                    if (priceCol <= 0) return 0;

                    string needle = (medName ?? string.Empty).Trim();
                    var dataRows = range.RowsUsed().Skip(1).ToList();
                    var matched = FindMedicineRowByCheckboxText(dataRows, needle);
                    if (matched != null)
                    {
                        return SafeParseMedicinePrice(matched.Cell(priceCol));
                    }
                }
            }
            catch { }
            return 0;
        }

        /// <summary>
        /// Locates the Medicines-sheet Price column by header name (case-insensitive).
        /// Returns 0 if no Price header is present.
        /// </summary>
        private static int FindMedicinePriceColumn(IXLRange range)
        {
            var header = range?.FirstRowUsed();
            if (header == null) return 0;

            foreach (var cell in header.CellsUsed())
            {
                string raw = (cell.Value.ToString() ?? string.Empty).Trim();
                if (string.Equals(raw, "Price", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(raw, "MedicinePrice", StringComparison.OrdinalIgnoreCase))
                {
                    return cell.Address.ColumnNumber;
                }
            }

            return 0;
        }

        /// <summary>
        /// Finds the medicines row whose any-column cell matches the checkbox text exactly
        /// (case-insensitive, trimmed) — works whether the checkbox shows the medicine name
        /// or its medicine code.
        /// </summary>
        private static IXLRangeRow FindMedicineRowByCheckboxText(IEnumerable<IXLRangeRow> dataRows, string text)
        {
            if (dataRows == null || string.IsNullOrEmpty(text)) return null;

            foreach (var row in dataRows)
            {
                foreach (var cell in row.Cells())
                {
                    string raw = (cell.Value.ToString() ?? string.Empty).Trim();
                    if (string.Equals(raw, text, StringComparison.OrdinalIgnoreCase))
                    {
                        return row;
                    }
                }
            }

            return null;
        }

        private void SafeLoadVeterinarians()
        {
            try
            {
                LoadVeterinarians();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not load veterinarians from Excel." + Environment.NewLine + Environment.NewLine + ex.Message,
                    "ClinicVets",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Populates the Vet Name dropdown from the Employees sheet of the unified workbook.
        /// Columns are resolved by header name (Role, FullName, Username) — never by fixed
        /// column letters. Only employees with Role == "Vet" or "Veterinarian" are listed,
        /// displaying FullName when present and falling back to Username.
        /// </summary>
        private void LoadVeterinarians()
        {
            txtVetName.Items.Clear();
            txtVetName.SelectedIndex = -1;
            txtVetName.DropDownStyle = ComboBoxStyle.DropDownList;

            string path = ExcelFileManager.FilePath;
            if (!File.Exists(path))
            {
                return;
            }

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var workbook = new XLWorkbook(fs))
            {
                if (!workbook.Worksheets.Contains("Employees"))
                {
                    return;
                }

                var sheet = workbook.Worksheet("Employees");
                var range = sheet.RangeUsed();
                if (range == null)
                {
                    return;
                }

                var headerRow = range.FirstRowUsed();
                if (headerRow == null)
                {
                    return;
                }

                int roleCol = FindEmployeeHeaderColumn(headerRow, "Role");
                int fullNameCol = FindEmployeeHeaderColumn(headerRow, "FullName");
                int usernameCol = FindEmployeeHeaderColumn(headerRow, "Username");
                if (roleCol <= 0)
                {
                    return;
                }

                var added = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var row in range.RowsUsed())
                {
                    if (row.RowNumber() == headerRow.RowNumber())
                    {
                        continue;
                    }

                    string role = (row.Cell(roleCol).Value.ToString() ?? string.Empty).Trim();
                    if (!IsVeterinarianRole(role))
                    {
                        continue;
                    }

                    string fullName = fullNameCol > 0
                        ? (row.Cell(fullNameCol).Value.ToString() ?? string.Empty).Trim()
                        : string.Empty;
                    string username = usernameCol > 0
                        ? (row.Cell(usernameCol).Value.ToString() ?? string.Empty).Trim()
                        : string.Empty;

                    string display = fullName.Length > 0 ? fullName : username;
                    if (display.Length == 0)
                    {
                        continue;
                    }

                    if (added.Add(display))
                    {
                        txtVetName.Items.Add(display);
                    }
                }
            }
        }

        private static bool IsVeterinarianRole(string role)
        {
            return string.Equals(role, "Vet", StringComparison.OrdinalIgnoreCase)
                || string.Equals(role, "Veterinarian", StringComparison.OrdinalIgnoreCase);
        }

        private static int FindEmployeeHeaderColumn(IXLRangeRow headerRow, string headerName)
        {
            if (headerRow == null || string.IsNullOrEmpty(headerName))
            {
                return 0;
            }

            foreach (var cell in headerRow.CellsUsed())
            {
                string raw = (cell.Value.ToString() ?? string.Empty).Trim();
                if (string.Equals(raw, headerName, StringComparison.OrdinalIgnoreCase))
                {
                    return cell.Address.ColumnNumber;
                }
            }

            return 0;
        }

        /// <summary>
        /// Decrements stock for every medicine the vet checked on the save form, looking
        /// up columns by header name (Name/MedicineName, Quantity/StockQuantity, Status,
        /// ExpiryDate). Persists "Out of Stock" or "Available" into the Status column —
        /// adding a Status header if the sheet doesn't already have one — and refuses to
        /// take any row below zero.
        /// </summary>
        private class InventoryUpdateResult
        {
            public int Decremented { get; set; }
            public List<string> NotFound { get; } = new List<string>();
        }

        private InventoryUpdateResult DecrementMedicineInventory(XLWorkbook workbook)
        {
            var result = new InventoryUpdateResult();

            if (workbook == null || clbMedicines.CheckedItems.Count == 0)
            {
                return result;
            }

            if (!workbook.Worksheets.Contains("Medicines"))
            {
                return result;
            }

            var medSheet = workbook.Worksheet("Medicines");
            var medRange = medSheet.RangeUsed();
            if (medRange == null)
            {
                return result;
            }

            var header = medRange.FirstRowUsed();
            if (header == null)
            {
                return result;
            }

            int nameCol = FindHeaderColumn(header, "Name", "MedicineName", "Medicine Name", "Medicine");
            int qtyCol = FindHeaderColumn(header, "Quantity", "StockQuantity", "Stock Quantity", "Stock", "Qty");
            int statusCol = FindHeaderColumn(header, "Status");
            int expiryCol = FindHeaderColumn(header, "ExpiryDate", "Expiry Date", "Expiry");

            // Fallbacks consistent with the MedicinesForm canonical schema:
            // col 1 = StockQuantity, col 2 = Price, col 3 = MedicineName, col 4 = MedicineID.
            if (nameCol <= 0)
            {
                nameCol = 3;
            }
            if (qtyCol <= 0)
            {
                qtyCol = 1;
            }

            if (statusCol <= 0)
            {
                var lastUsedHeader = header.LastCellUsed();
                int newCol = (lastUsedHeader?.Address.ColumnNumber ?? 0) + 1;
                medSheet.Cell(header.RowNumber(), newCol).Value = "Status";
                statusCol = newCol;
            }

            var dataRows = medRange.RowsUsed()
                .Where(r => r.RowNumber() != header.RowNumber())
                .ToList();

            foreach (var item in clbMedicines.CheckedItems)
            {
                string rawText = item?.ToString() ?? string.Empty;
                string selectedMed = ExtractMedicineName(rawText);
                if (selectedMed.Length == 0)
                {
                    continue;
                }

                IXLRangeRow targetRow = null;
                foreach (var row in dataRows)
                {
                    string rowName = (row.Cell(nameCol).Value.ToString() ?? string.Empty).Trim();
                    if (string.Equals(rowName, selectedMed, StringComparison.OrdinalIgnoreCase))
                    {
                        targetRow = row;
                        break;
                    }
                }

                if (targetRow == null)
                {
                    result.NotFound.Add(selectedMed);
                    continue;
                }

                int currentQty = SafeParseInt(targetRow.Cell(qtyCol));

                if (currentQty <= 0)
                {
                    MessageBox.Show(
                        this,
                        "Medicine is out of stock.",
                        "Inventory",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    targetRow.Cell(qtyCol).Value = 0; // never allow negatives
                    targetRow.Cell(statusCol).Value = "Out of Stock";
                    continue;
                }

                int newQty = currentQty - 1;
                targetRow.Cell(qtyCol).Value = newQty;
                result.Decremented++;

                if (newQty <= 0)
                {
                    targetRow.Cell(statusCol).Value = "Out of Stock";
                }
                else if (!IsMedicineExpired(targetRow, expiryCol))
                {
                    targetRow.Cell(statusCol).Value = "Available";
                }
            }

            return result;
        }

        /// <summary>
        /// Cleans up a checkbox item text so it represents only the medicine name.
        /// Accepts "Acamol", "1234 - Acamol", "1234 — Acamol" and similar.
        /// </summary>
        private static string ExtractMedicineName(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return string.Empty;
            }

            string text = raw.Trim();

            // Strip an "ID - Name" or "ID — Name" prefix when present.
            int sep = text.IndexOf(" - ", StringComparison.Ordinal);
            if (sep < 0)
            {
                sep = text.IndexOf(" \u2014 ", StringComparison.Ordinal);
            }
            if (sep < 0)
            {
                sep = text.IndexOf('\u2014');
            }
            if (sep > 0)
            {
                string rest = text.Substring(sep + 1).TrimStart('-', '\u2014', ' ');
                if (rest.Length > 0)
                {
                    text = rest.Trim();
                }
            }

            return text;
        }

        /// <summary>
        /// Reads the Medicines sheet and populates clbMedicines with the values from the
        /// Name column (or MedicineName/Medicine/etc., resolved by header). Falls back to
        /// column 3, which is the canonical Name column per MedicinesForm.SaveMedicines.
        /// </summary>
        private void PopulateMedicinesCheckList(XLWorkbook workbook)
        {
            clbMedicines.Items.Clear();
            if (workbook == null || !workbook.Worksheets.Contains("Medicines"))
            {
                return;
            }

            var sheet = workbook.Worksheet("Medicines");
            var range = sheet.RangeUsed();
            if (range == null)
            {
                return;
            }

            var header = range.FirstRowUsed();
            if (header == null)
            {
                return;
            }

            int nameCol = FindHeaderColumn(header, "Name", "MedicineName", "Medicine Name", "Medicine");
            if (nameCol <= 0)
            {
                nameCol = 3;
            }

            foreach (var row in range.RowsUsed())
            {
                if (row.RowNumber() == header.RowNumber())
                {
                    continue;
                }

                string name = (row.Cell(nameCol).Value.ToString() ?? string.Empty).Trim();
                if (name.Length > 0)
                {
                    clbMedicines.Items.Add(name);
                }
            }
        }

        /// <summary>
        /// Re-reads the Medicines sheet (read-only) and refreshes the checkbox list so
        /// the UI reflects the latest quantities. Best-effort: silently no-ops on any
        /// I/O issue since the visit has already been persisted at this point.
        /// </summary>
        private void ReloadMedicinesList()
        {
            try
            {
                string path = ExcelFileManager.FilePath;
                if (!File.Exists(path))
                {
                    return;
                }

                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var workbook = new XLWorkbook(fs))
                {
                    PopulateMedicinesCheckList(workbook);
                }
            }
            catch
            {
                // Best-effort refresh — the visit has already been saved successfully.
            }
        }

        private static bool IsMedicineExpired(IXLRangeRow row, int expiryCol)
        {
            if (row == null || expiryCol <= 0)
            {
                return false;
            }

            if (TryParsePetDate(row.Cell(expiryCol), out DateTime expiry))
            {
                return expiry.Date < DateTime.Today;
            }

            return false;
        }

        /// <summary>
        /// Reads an integer cell defensively: empty/text/invalid values become 0 instead
        /// of throwing (mirrors the price-parsing pattern used for medicines).
        /// </summary>
        private static int SafeParseInt(IXLCell cell)
        {
            if (cell == null)
            {
                return 0;
            }

            string raw = (cell.Value.ToString() ?? string.Empty).Trim();
            if (raw.Length == 0)
            {
                return 0;
            }

            if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.CurrentCulture, out int v))
            {
                return v;
            }
            if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out v))
            {
                return v;
            }
            if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
            {
                return (int)Math.Floor(d);
            }

            return 0;
        }

        /// <summary>
        /// Reads a medicine price cell defensively: never calls GetDouble() (which throws
        /// when the cell is empty or stored as text). Empty/invalid values are treated as 0.
        /// </summary>
        private static double SafeParseMedicinePrice(IXLCell cell)
        {
            if (cell == null)
            {
                return 0;
            }

            string raw = cell.Value.ToString();
            if (string.IsNullOrWhiteSpace(raw))
            {
                return 0;
            }

            if (double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out double price))
            {
                return price;
            }

            if (double.TryParse(raw, NumberStyles.Any, CultureInfo.CurrentCulture, out price))
            {
                return price;
            }

            return 0;
        }

        private void txtReason_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReason.Text))
            {
                lblReasonError.Visible = true;
            }
            else
            {
                lblReasonError.Visible = false;
            }
        }

        private void txtVetName_Leave(object sender, EventArgs e)
        {
            string vetName = txtVetName.Text.Trim();

            // Reset both first
            lblVetEmpty.Visible = false;
            lblVetInvalid.Visible = false;

            // Check 1: Is it empty?
            if (string.IsNullOrWhiteSpace(vetName))
            {
                lblVetEmpty.Visible = true;
            }
            // Check 2: Does it contain non-letters? 
            // (Only check this if it's NOT empty, to avoid double errors)
            else if (!vetName.All(c => char.IsLetter(c) || c == ' '))
            {
                lblVetInvalid.Visible = true;
            }
        }

        private void ApplyBackgroundFromImagesFolder()
        {
            if (pictureBox1 == null)
            {
                return;
            }

            ReleasePictureBoxBackground();

            try
            {
                string path = FindBackgroundImagePath();
                if (path == null)
                {
                    return;
                }

                using (Image loaded = Image.FromFile(path))
                {
                    pictureBox1.Image = new Bitmap(loaded);
                }

                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            catch
            {
                pictureBox1.Image = null;
            }
        }

        private void ReleasePictureBoxBackground()
        {
            if (pictureBox1?.Image == null)
            {
                return;
            }

            Image previous = pictureBox1.Image;
            pictureBox1.Image = null;
            previous.Dispose();
        }

        private static string FindBackgroundImagePath()
        {
            foreach (string root in GetBackgroundSearchRoots())
            {
                if (string.IsNullOrWhiteSpace(root))
                {
                    continue;
                }

                string resourcesPath = Path.Combine(root, "Resources", BackgroundFileName);
                if (File.Exists(resourcesPath))
                {
                    return resourcesPath;
                }

                string imagesPath = Path.Combine(root, "images", BackgroundFileName);
                if (File.Exists(imagesPath))
                {
                    return imagesPath;
                }
            }

            return null;
        }

        private static IEnumerable<string> GetBackgroundSearchRoots()
        {
            yield return Application.StartupPath;
            yield return AppDomain.CurrentDomain.BaseDirectory;

            string location = Assembly.GetExecutingAssembly().Location;
            if (!string.IsNullOrEmpty(location))
            {
                string dir = Path.GetDirectoryName(location);
                if (!string.IsNullOrEmpty(dir))
                {
                    yield return dir;
                }
            }

            yield return Environment.CurrentDirectory;
        }

        private void EnsureBackButton()
        {
            if (_btnBack != null)
            {
                return;
            }

            _btnBack = new Button
            {
                Text = "← Back",
                Size = new Size(100, 36),
                Location = new Point(16, 12),
                BackColor = System.Drawing.Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                ForeColor = System.Drawing.Color.SteelBlue,
                Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold)
            };
            _btnBack.Click += (sender, e) => Close();
            Controls.Add(_btnBack);
            _btnBack.BringToFront();
        }

        private void dtpVisitTime_Leave(object sender, EventArgs e)
        {
            // Only check if the selected date is Today
            if (dtpVisitDate.Value.Date == DateTime.Today)
            {
                // If the selected time is in the future compared to right now
                if (dtpVisitTime.Value.TimeOfDay > DateTime.Now.TimeOfDay)
                {
                    lblTimeError.Visible = true;
                }
                else
                {
                    lblTimeError.Visible = false;
                }
            }
            else
            {
                // If it's a past date, the time can't be "in the future" logically
                lblTimeError.Visible = false;
            }
        }
    }
}