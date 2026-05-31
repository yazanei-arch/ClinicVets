using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ClosedXML.Excel;
using ClinicVets;
using DocumentFormat.OpenXml.Wordprocessing;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;


namespace ClinicVets.UI
{
    public partial class AllPetsForm : Form
    {
        private readonly string filePath = ExcelFileManager.FilePath;
        private bool isUpdateMode = false;
        private int editingRowIndex = -1;
        private string editingPetCode = "";

        public AllPetsForm()
        {
            InitializeComponent();
            ClinicFormLayout.ApplyStandard(this);
            dgvPets.EnableHeadersVisualStyles = false;

            dgvPets.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgvPets.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPets.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvPets.ColumnHeadersHeight = 45;

            dgvPets.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvPets.DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue;
            dgvPets.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvPets.BackgroundColor = Color.White;
            dgvPets.GridColor = Color.LightGray;
            dgvPets.BorderStyle = BorderStyle.FixedSingle;

            dgvPets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPets.ReadOnly = true;
            dgvPets.AllowUserToAddRows = false;
            dgvPets.AllowUserToDeleteRows = false;
            dgvPets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPets.RowHeadersVisible = false;

        }

        private void AllPetsForm_Load(object sender, EventArgs e)
        {
            PetBackgroundHelper.ApplyToPictureBox(pictureBox1);
            if (pictureBox1 != null)
            {
                pictureBox1.SendToBack();
            }

            ApplyAllPetsContentLayout();
            SafeLoadPetsFromExcel();
            Resize += AllPetsForm_Resize;
        }

        private void AllPetsForm_Resize(object sender, EventArgs e)
        {
            ApplyAllPetsContentLayout();
        }

        private void ApplyAllPetsContentLayout()
        {
            const int gridWidth = 756;
            const int gridHeight = 295;
            const int buttonWidth = 160;
            const int buttonHeight = 45;
            const int buttonGap = 28;
            const int contentShiftLeft = 48;

            int centerX = (ClientSize.Width / 2) - contentShiftLeft;
            int gridLeft = centerX - (gridWidth / 2);

            const int titleTop = 178;
            const int titleHeight = 28;
            const int gapTitleToSubtitle = 12;
            const int gapSubtitleToTable = 34;
            const int gapTableToButtons = 24;

            int subtitleTop = titleTop + titleHeight + gapTitleToSubtitle;
            const int subtitleHeight = 26;
            int gridTop = subtitleTop + subtitleHeight + gapSubtitleToTable;

            int buttonsRowWidth = (buttonWidth * 2) + buttonGap;
            int buttonsLeft = centerX - (buttonsRowWidth / 2);
            int buttonsTop = gridTop + gridHeight + gapTableToButtons;

            const int titleBlockWidth = 380;
            const int subtitleBlockWidth = 500;
            label1.AutoSize = false;
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.BackColor = Color.Transparent;
            label1.SetBounds(centerX - (titleBlockWidth / 2), titleTop, titleBlockWidth, titleHeight);

            label2.AutoSize = false;
            label2.TextAlign = ContentAlignment.TopCenter;
            label2.BackColor = Color.Transparent;
            label2.SetBounds(centerX - (subtitleBlockWidth / 2), subtitleTop, subtitleBlockWidth, subtitleHeight);

            dgvPets.Size = new Size(gridWidth, gridHeight);
            dgvPets.Location = new Point(gridLeft, gridTop);

            btnUpdatePet.Size = new Size(buttonWidth, buttonHeight);
            btnUpdatePet.Location = new Point(buttonsLeft, buttonsTop);

            btnDeletePet.Size = new Size(buttonWidth, buttonHeight);
            btnDeletePet.Location = new Point(buttonsLeft + buttonWidth + buttonGap, buttonsTop);

            PlaceBackButtonTopLeft();
            dgvPets.BringToFront();
            btnUpdatePet.BringToFront();
            btnDeletePet.BringToFront();
            label1.BringToFront();
            label2.BringToFront();
        }

        private void PlaceBackButtonTopLeft()
        {
            const int margin = 16;
            const int top = 12;
            int x = RightToLeftLayout && RightToLeft == RightToLeft.Yes
                ? ClientSize.Width - btnBack.Width - margin
                : margin;
            btnBack.Location = new Point(x, top);
            btnBack.BringToFront();
        }

        private void SafeLoadPetsFromExcel()
        {
            dgvPets.Rows.Clear();

            if (!PetExcelSupport.TryEnsureWorkbookReady(this, out string workbookPath))
            {
                return;
            }

            try
            {
                LoadPetsFromExcel(workbookPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not load pets from Excel." + Environment.NewLine + Environment.NewLine + ex.Message,
                    "ClinicVets",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void LoadPetsFromExcel(string workbookPath)
        {
            if (!File.Exists(workbookPath))
            {
                MessageBox.Show("Excel file not found:\n" + workbookPath, "ClinicVets", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var workbook = new XLWorkbook(workbookPath))
            {
                var sheet = workbook.Worksheet("Pets");

                if (sheet == null)
                {
                    MessageBox.Show("Sheet Pets not found");
                    return;
                }

                var usedRange = sheet.RangeUsed();

                if (usedRange == null)
                {
                    MessageBox.Show("No data found in Pets sheet");
                    return;
                }

                foreach (var row in usedRange.RowsUsed())
                {
                    if (row.RowNumber() == 1)
                        continue;

                    dgvPets.Rows.Add(
                        row.Cell(1).GetValue<string>(), // PetID
                        row.Cell(2).GetValue<string>(), // PetName
                        row.Cell(3).GetValue<string>(), // AnimalType
                        row.Cell(4).GetValue<string>(), // Weight
                        row.Cell(5).GetValue<string>(), // BirthDate
                        row.Cell(6).GetValue<string>(), // Owner
                        row.Cell(7).GetValue<string>(), // ChipNumber
                        row.Cell(8).GetValue<string>()  // LastVaccineDate
                    );
                }
            }

            dgvPets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnBack_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void btnDeletePet_Click(object sender, EventArgs e)
        {
            if (dgvPets.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a pet to delete");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this pet?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            int selectedIndex = dgvPets.SelectedRows[0].Index;

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Excel file not found:\n" + filePath);
                return;
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                var sheet = workbook.Worksheet("Pets");

                if (sheet == null)
                {
                    MessageBox.Show("Sheet Pets not found");
                    return;
                }

                int excelRowNumber = selectedIndex + 2;

                sheet.Row(excelRowNumber).Delete();

                workbook.Save();
            }

            SafeLoadPetsFromExcel();

            MessageBox.Show("Pet deleted successfully");
        }

        private void btnUpdatePet_Click(object sender, EventArgs e)
        {
            if (!isUpdateMode)
            {
                if (dgvPets.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Select a pet to update");
                    return;
                }

                editingRowIndex = dgvPets.SelectedRows[0].Index;
                editingPetCode = dgvPets.Rows[editingRowIndex].Cells[0].Value.ToString();

                dgvPets.ReadOnly = false;

                foreach (DataGridViewRow row in dgvPets.Rows)
                {
                    row.ReadOnly = true;
                }

                dgvPets.Rows[editingRowIndex].ReadOnly = false;

                // Pet Code should not be changed
                dgvPets.Rows[editingRowIndex].Cells[0].ReadOnly = true;
                dgvPets.Rows[editingRowIndex].Cells[2].ReadOnly = true;

                isUpdateMode = true;
                btnUpdatePet.Text = "Save Update";

                MessageBox.Show("You can now edit the selected pet row");
            }
            else
            {

                string message = "";
                bool name = false, fWeight = false, owner = false, chip = false, birth = false, vaccine = false;

                if (string.IsNullOrWhiteSpace(dgvPets.Rows[editingRowIndex].Cells[1].Value?.ToString()))
                {
                    name = true;
                    dgvPets.Rows[editingRowIndex].Cells[1].Style.BackColor = Color.FromArgb(255, 200, 200);
                    message += "Pet name is required\n";
                }
                else if (!string.IsNullOrWhiteSpace(dgvPets.Rows[editingRowIndex].Cells[1].Value?.ToString()))
                {
                    foreach (char c in dgvPets.Rows[editingRowIndex].Cells[1].Value.ToString().Trim())
                    {
                        if (!char.IsLetter(c) && c != ' ')
                        {
                            name = true;
                            dgvPets.Rows[editingRowIndex].Cells[1].Style.BackColor = Color.FromArgb(255, 200, 200);
                            message += "Pet name must be only letters\n";                       
                            break;
                        }
                    }
                }
                else
                {
                    dgvPets.Rows[editingRowIndex].Cells[1].Style.BackColor = Color.Empty;
                }

                double weight = 0;
                string weightText = dgvPets.Rows[editingRowIndex].Cells[3].Value?.ToString().Trim();
                if (string.IsNullOrWhiteSpace(weightText))
                {
                    fWeight = true;
                    message += "Weight is required\n";
                    dgvPets.Rows[editingRowIndex].Cells[3].Style.BackColor = Color.FromArgb(255, 200, 200);
                }
                else if (!double.TryParse(weightText, out weight)) 
                {
                    fWeight = true;
                    message += "Weight must be a number\n";
                    dgvPets.Rows[editingRowIndex].Cells[3].Style.BackColor = Color.FromArgb(255, 200, 200);
                }
                else if (weight < 0.1 || weight > 100)
                {
                    fWeight = true;
                    message += "Weight must be 0.1 - 100\n";
                    dgvPets.Rows[editingRowIndex].Cells[3].Style.BackColor = Color.FromArgb(255, 200, 200);
                }
                else
                {
                    dgvPets.Rows[editingRowIndex].Cells[3].Style.BackColor = Color.Empty;
                }

                if (Convert.ToDateTime(dgvPets.Rows[editingRowIndex].Cells[4].Value) > DateTime.Today)
                {
                    birth = true;
                    dgvPets.Rows[editingRowIndex].Cells[4].Style.BackColor = Color.FromArgb(255, 200, 200);
                    message += "The birth date cant be in the future\n";
                }
                else
                {
                    dgvPets.Rows[editingRowIndex].Cells[4].Style.BackColor = Color.Empty;
                }

                string ownerName = dgvPets.Rows[editingRowIndex].Cells[5].Value?.ToString().Trim();

                if (string.IsNullOrWhiteSpace(ownerName))
                {
                    owner = true;
                    dgvPets.Rows[editingRowIndex].Cells[5].Style.BackColor = Color.FromArgb(255, 200, 200);
                    message = "Owner is required\n";
                }
                else if (!string.IsNullOrWhiteSpace(ownerName))
                {
                    foreach (char c in dgvPets.Rows[editingRowIndex].Cells[5].Value.ToString())
                    {
                        if (!char.IsLetter(c) && c != ' ')
                        {
                            owner = true;
                            dgvPets.Rows[editingRowIndex].Cells[5].Style.BackColor = Color.FromArgb(255, 200, 200);
                            message += "Owner name has to be only letters\n";
                            break;
                        }
                    }
                }
                else
                {
                    dgvPets.Rows[editingRowIndex].Cells[5].Style.BackColor = Color.Empty;
                }

                string chipNumber = dgvPets.Rows[editingRowIndex].Cells[6].Value?.ToString().Trim();

                if (string.IsNullOrWhiteSpace(chipNumber))
                {
                    chip = true;
                    dgvPets.Rows[editingRowIndex].Cells[6].Style.BackColor = Color.FromArgb(255, 200, 200);
                    message = "Chip number is required\n";
                }
                else if (!string.IsNullOrWhiteSpace(chipNumber))
                {
                    string cellText = dgvPets.Rows[editingRowIndex].Cells[6].Value?.ToString() ?? "";

                    foreach (char c in cellText)
                    {
                        if (!char.IsDigit(c) && c != ' ')
                        {
                            owner = true;
                            dgvPets.Rows[editingRowIndex].Cells[6].Style.BackColor = Color.FromArgb(255, 200, 200);
                            message += "Chip number must be only numbers\n";
                            break;
                        }
                    }
                }
                else
                {
                    dgvPets.Rows[editingRowIndex].Cells[6].Style.BackColor = Color.Empty;
                }

                if (Convert.ToDateTime(dgvPets.Rows[editingRowIndex].Cells[7].Value) > DateTime.Today)
                {
                    vaccine = true;
                    dgvPets.Rows[editingRowIndex].Cells[7].Style.BackColor = Color.FromArgb(255, 200, 200);
                    message += "The birth date cant be in the future\n";
                }
                else
                {
                    dgvPets.Rows[editingRowIndex].Cells[7].Style.BackColor = Color.Empty;
                }

                if (birth || vaccine || name || fWeight || owner || chip)
                {
                    MessageBox.Show(message);
                    return;
                }

                SaveUpdatedPetToExcel();

                dgvPets.ReadOnly = true;
                isUpdateMode = false;
                editingRowIndex = -1;
                editingPetCode = "";

                btnUpdatePet.Text = "Update Pet";

                SafeLoadPetsFromExcel();

                MessageBox.Show("Pet updated successfully");
            }
        }
        private void SaveUpdatedPetToExcel()
        {
            if (editingRowIndex == -1)
            {
                MessageBox.Show("No pet selected for update");
                return;
            }

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Excel file not found:\n" + filePath);
                return;
            }

            string petCode = dgvPets.Rows[editingRowIndex].Cells[0].Value.ToString();
            string petName = dgvPets.Rows[editingRowIndex].Cells[1].Value.ToString();
            string animalType = dgvPets.Rows[editingRowIndex].Cells[2].Value.ToString();
            string weight = dgvPets.Rows[editingRowIndex].Cells[3].Value.ToString();
            string birthDate = dgvPets.Rows[editingRowIndex].Cells[4].Value.ToString();
            string owner = dgvPets.Rows[editingRowIndex].Cells[5].Value.ToString();
            string chipNumber = dgvPets.Rows[editingRowIndex].Cells[6].Value.ToString();
            string lastVaccineDate = dgvPets.Rows[editingRowIndex].Cells[7].Value.ToString();

            using (var workbook = new XLWorkbook(filePath))
            {
                var sheet = workbook.Worksheet("Pets");

                if (sheet == null)
                {
                    MessageBox.Show("Sheet Pets not found");
                    return;
                }

                var usedRange = sheet.RangeUsed();

                if (usedRange == null)
                {
                    MessageBox.Show("No data found in Pets sheet");
                    return;
                }

                

                foreach (var row in usedRange.RowsUsed())
                {
                    if (row.RowNumber() == 1)
                        continue;

                    string currentPetCode = row.Cell(1).GetValue<string>();

                    if (currentPetCode == editingPetCode)
                    {
                        row.Cell(1).Value = petCode;
                        row.Cell(2).Value = petName;
                        row.Cell(3).Value = animalType;
                        row.Cell(4).Value = weight;
                        row.Cell(5).Value = birthDate;
                        row.Cell(6).Value = owner;
                        row.Cell(7).Value = chipNumber;
                        row.Cell(8).Value = lastVaccineDate;

                        workbook.Save();
                        return;
                    }
                }
            }

            MessageBox.Show("Pet not found in Excel file");
        }
    }
}