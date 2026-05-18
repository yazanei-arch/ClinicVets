using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ClinicVets.VisitsMedicines
{
    public partial class VisitForm : Form
    {
        public VisitForm()
        {
            InitializeComponent();
        }

        string filePath = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\ClinicVetsData.xlsx";

        private void VisitForm_Load(object sender, EventArgs e)
        {
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
                        // Column 2 is "PetName" in your Excel
                        cmbPets.Items.Add(row.Cell(2).Value.ToString());
                    }



                    var medSheet = workbook.Worksheet("Medicines");
                    var medRows = medSheet.RangeUsed().RowsUsed().Skip(1);
                    clbMedicines.Items.Clear();

                    foreach (var row in medRows)
                    {
                        // Column 2 is the Medicine Name. 
                        clbMedicines.Items.Add(row.Cell(2).Value.ToString());
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading pets: " + ex.Message);
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
                string filePath = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\ClinicVetsData.xlsx";

                using (var workbook = new XLWorkbook(filePath))
                {
                    var visitSheet = workbook.Worksheet("Visits");
                    int lastRow = visitSheet.LastRowUsed().RowNumber() + 1;

                    // Combine checked medicines into one text string
                    string selectedMeds = string.Join(", ", clbMedicines.CheckedItems.Cast<string>());
                    string price = lblTotalCost.Text;
                    var words = price.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string result = string.Join(" ", words.TakeLast(2));
                
                    visitSheet.Cell(lastRow, 1).Value = new Random().Next(1000, 9999);   // VisitID (A)
                    visitSheet.Cell(lastRow, 2).Value = cmbPets.SelectedItem.ToString(); // PetID/Name (B)
                    visitSheet.Cell(lastRow, 3).Value = dtpVisitDate.Value.ToShortDateString(); // VisitDate (C)
                    visitSheet.Cell(lastRow, 4).Value = dtpVisitTime.Value.ToShortTimeString();
                    visitSheet.Cell(lastRow, 5).Value = txtReason.Text;                  // Reason (E)
                    visitSheet.Cell(lastRow, 6).Value = rtbSummary.Text;                 // Diagnosis (F)
                    visitSheet.Cell(lastRow, 7).Value = selectedMeds;                    // Medicines (G)
                    visitSheet.Cell(lastRow, 8).Value = txtVetName.Text; // VetName (H)
                    visitSheet.Cell(lastRow, 9).Value = result;               // TotalPrice (I)

                    var medSheet = workbook.Worksheet("Medicines");
                    var medRows = medSheet.RangeUsed().RowsUsed().Skip(1);

                    foreach (var item in clbMedicines.CheckedItems)
                    {
                        string selectedMed = item.ToString();
                        // Find the medicine in the sheet (Name is Column 7/G)
                        var row = medRows.FirstOrDefault(r => r.Cell(7).Value.ToString() == selectedMed);

                        if (row != null)
                        {

                            int currentStock = row.Cell(4).GetValue<int>();

                            if (currentStock > 0)
                            {
                                row.Cell(4).Value = currentStock - 1; // Subtract one
                            }
                            else
                            {
                                MessageBox.Show("Warning: " + selectedMed + " is out of stock!");
                            }
                        }
                    }

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
                }
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

        private void cmbPets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPets.SelectedItem == null) return;
            
            string selectedPet = cmbPets.SelectedItem.ToString();
            // Ensure filePath is defined (usually at the top of your class or here)
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\ClinicVetsData.xlsx";

            try
            {
                // Use FileStream to prevent "File in Use" errors
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var workbook = new XLWorkbook(fs))
                {
                    var worksheet = workbook.Worksheet("Pets");
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                    foreach (var row in rows)
                    {
                        // CORRECTED: Pet Name is in Column 2 (B)
                        if (row.Cell(2).Value.ToString() == selectedPet)
                        {
                            // CORRECTED: LastVaccineDate is in Column 8 (H)
                            DateTime lastVaccine = row.Cell(8).GetDateTime();

                            // Check if more than 365 days have passed
                            if ((DateTime.Now - lastVaccine).TotalDays > 365)
                            {
                                lblVaccineAlert.Text = "Annual Vaccine Required!";
                                lblVaccineAlert.Visible = true;

                                // OPTIONAL: Auto-check the vaccine in the checklist
                                for (int i = 0; i < clbMedicines.Items.Count; i++)
                                {
                                    if (clbMedicines.Items[i].ToString().Contains("Vaccine"))
                                    {
                                        clbMedicines.SetItemChecked(i, true);
                                    }
                                }
                            }
                            else
                            {
                                lblVaccineAlert.Visible = false;
                            }
                            break; // Found the pet, stop looking
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // This prevents the app from crashing if a date is empty in Excel
                lblVaccineAlert.Visible = false;
                Console.WriteLine("Error checking vaccine: " + ex.Message);
            }
        }

        private void clbMedicines_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string itemName = clbMedicines.Items[e.Index].ToString();
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\..\ClinicVetsData.xlsx";

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
                // ---> BASE VISIT FEE: Start at 100 NIS instead of 0
                double total = 100;

                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var workbook = new XLWorkbook(fs))
                {
                    var medSheet = workbook.Worksheet("Medicines");
                    foreach (var item in clbMedicines.CheckedItems)
                    {
                        var row = medSheet.RangeUsed().RowsUsed().FirstOrDefault(r => r.Cell(2).Value.ToString() == item.ToString());
                        if (row != null) total += row.Cell(3).GetDouble();
                    }
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
                    var rows = sheet.RangeUsed().RowsUsed().Skip(1);
                    foreach (var row in rows)
                    {
                        if (row.Cell(2).Value.ToString() == medName)
                            return row.Cell(3).GetDouble();
                    }
                }
            }
            catch { }
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