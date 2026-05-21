using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClosedXML.Excel;
using ClinicVets;

namespace ClinicVets.UI
{
    public partial class SearchPetForm : Form
    {
        private readonly bool _vetWorkflow;
        private Button _btnOpenVisit;

        public SearchPetForm()
            : this(false)
        {
        }

        public SearchPetForm(bool vetWorkflow)
        {
            _vetWorkflow = vetWorkflow;
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Normal;
            Shown += SearchPetForm_Shown;
            SetupDataGridView();
            if (_vetWorkflow)
            {
                SetupVetOpenVisitButton();
            }
        }

        private void SearchPetForm_Shown(object sender, EventArgs e)
        {
            StartPosition = FormStartPosition.CenterScreen;
            CenterToScreen();
        }

        private void SearchPetForm_Load(object sender, EventArgs e)
        {
            ClinicFormLayout.ApplyStandard(this);
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Normal;
            PetBackgroundHelper.ApplyToPictureBox(pictureBox1);
            if (pictureBox1 != null)
            {
                pictureBox1.Dock = DockStyle.Fill;
                pictureBox1.SendToBack();
            }

            ApplySearchPetContentLayout();
            Resize += SearchPetForm_Resize;
        }

        private void SearchPetForm_Resize(object sender, EventArgs e)
        {
            ApplySearchPetContentLayout();
        }

        private void ApplySearchPetContentLayout()
        {
            const int contentShiftLeft = 48;
            const int gridWidth = 756;
            const int gridHeight = 295;
            const int fieldWidth = 225;
            const int fieldHeight = 36;
            const int buttonWidth = 180;
            const int buttonHeight = 56;
            const int buttonGap = 28;
            const int edgeMargin = 24;

            int centerX = (ClientSize.Width / 2) - contentShiftLeft;
            int gridLeft = centerX - (gridWidth / 2);

            const int titleTop = 178;
            const int subtitleTop = 212;
            const int searchTop = 248;
            const int gridTop = 302;
            const int buttonsTop = gridTop + gridHeight + 20;

            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            int titleWidth = TextRenderer.MeasureText(label1.Text, label1.Font).Width + 8;
            label1.SetBounds(centerX - (titleWidth / 2), titleTop, titleWidth, 32);

            label3.AutoSize = false;
            label3.BackColor = Color.Transparent;
            label3.TextAlign = ContentAlignment.TopCenter;
            const int subtitleWidth = 520;
            label3.SetBounds(centerX - (subtitleWidth / 2), subtitleTop, subtitleWidth, 30);

            int chipLabelWidth = 151;
            int nameLabelWidth = 114;
            int rowWidth = fieldWidth + 8 + chipLabelWidth + 24 + fieldWidth + 8 + nameLabelWidth;
            int rowLeft = centerX - (rowWidth / 2);

            txtChipNumber2.SetBounds(rowLeft, searchTop, fieldWidth, fieldHeight);
            label7.SetBounds(rowLeft + fieldWidth + 8, searchTop + 4, chipLabelWidth, 30);
            txtPetName2.SetBounds(rowLeft + fieldWidth + 8 + chipLabelWidth + 24, searchTop, fieldWidth, fieldHeight);
            label2.SetBounds(
                rowLeft + fieldWidth + 8 + chipLabelWidth + 24 + fieldWidth + 8,
                searchTop + 4,
                nameLabelWidth,
                30);

            dgvPets.SetBounds(gridLeft, gridTop, gridWidth, gridHeight);

            int buttonsRowWidth = (buttonWidth * 2) + buttonGap;
            int buttonsLeft = centerX - (buttonsRowWidth / 2);
            int bottomButtonsTop = Math.Min(buttonsTop, ClientSize.Height - buttonHeight - edgeMargin);
            btnSearch.SetBounds(buttonsLeft, bottomButtonsTop, buttonWidth, buttonHeight);
            btnClear2.SetBounds(buttonsLeft + buttonWidth + buttonGap, bottomButtonsTop, buttonWidth, buttonHeight);

            PlaceBackButtonTopLeft();
            PositionVetVisitButton(bottomButtonsTop);

            dgvPets.BringToFront();
            btnSearch.BringToFront();
            btnClear2.BringToFront();
            label1.BringToFront();
            label2.BringToFront();
            label3.BringToFront();
            label7.BringToFront();
            txtChipNumber2.BringToFront();
            txtPetName2.BringToFront();
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

        private void PositionVetVisitButton(int referenceButtonsTop)
        {
            if (_btnOpenVisit == null)
            {
                return;
            }

            const int edgeMargin = 24;
            int y = Math.Max(referenceButtonsTop - _btnOpenVisit.Height - 12, 520);
            int x = Math.Max(edgeMargin, ClientSize.Width - _btnOpenVisit.Width - edgeMargin);
            if (RightToLeftLayout && RightToLeft == RightToLeft.Yes)
            {
                x = edgeMargin;
            }

            _btnOpenVisit.Location = new Point(x, y);
            _btnOpenVisit.BringToFront();
        }

        private void SetupVetOpenVisitButton()
        {
            _btnOpenVisit = new Button
            {
                Text = "Open Visit / Treatment",
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 10.8F, FontStyle.Bold),
                ForeColor = Color.SteelBlue,
                Size = new Size(280, 56),
                Location = new Point(830, 645),
                UseVisualStyleBackColor = false
            };
            _btnOpenVisit.Click += BtnOpenVisit_Click;
            Controls.Add(_btnOpenVisit);
        }

        private void SetupDataGridView()
        {
            dgvPets.Columns.Clear();

            if (_vetWorkflow)
            {
                dgvPets.Columns.Add("PetID", "PetID");
            }

            dgvPets.Columns.Add("Pet Name", "Pet Name");
            dgvPets.Columns.Add("Animal Type", "Animal Type");
            dgvPets.Columns.Add("Weight", "Weight");
            dgvPets.Columns.Add("Birth Date", "Birth Date");
            dgvPets.Columns.Add("Owner", "Owner");
            dgvPets.Columns.Add("Chip Number", "Chip Number");
            dgvPets.Columns.Add("Last Vaccine Date", "Last Vaccine Date");

            dgvPets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPets.ReadOnly = true;
            dgvPets.AllowUserToAddRows = false;
            dgvPets.AllowUserToDeleteRows = false;
            dgvPets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPets.MultiSelect = false;
            dgvPets.RowHeadersVisible = false;

            dgvPets.EnableHeadersVisualStyles = false;
            dgvPets.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgvPets.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPets.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvPets.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgvPets.DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue;
            dgvPets.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        private void btnClear2_Click(object sender, EventArgs e)
        {
            txtPetName2.Clear();
            txtChipNumber2.Clear();
            dgvPets.Rows.Clear();
        }

        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            string petName = txtPetName2.Text.Trim();
            string chipNumber = txtChipNumber2.Text.Trim();

            if (string.IsNullOrWhiteSpace(petName) && string.IsNullOrWhiteSpace(chipNumber))
            {
                MessageBox.Show("Please enter Pet Name or Chip Number.");
                return;
            }

            dgvPets.Rows.Clear();

            string filePath = ExcelFileManager.FilePath;

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Excel file not found");
                return;
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                if (!workbook.Worksheets.Contains("Pets"))
                {
                    MessageBox.Show("Pets sheet not found.");
                    return;
                }

                var sheet = workbook.Worksheet("Pets");
                var range = sheet.RangeUsed();

                if (range == null)
                {
                    MessageBox.Show("No pets found.");
                    return;
                }

                int petIdCol = 1;
                int nameCol = 2;
                int typeCol = 3;
                int weightCol = 4;
                int birthCol = 5;
                int ownerCol = 6;
                int chipCol = 7;
                int vaccineCol = 8;

                var headerRow = sheet.Row(1);
                string h1 = headerRow.Cell(1).GetString().Trim();
                if (h1.Equals("PetName", StringComparison.OrdinalIgnoreCase) ||
                    h1.Equals("Pet Name", StringComparison.OrdinalIgnoreCase))
                {
                    petIdCol = 0;
                    nameCol = 1;
                    typeCol = 2;
                    weightCol = 3;
                    birthCol = 4;
                    ownerCol = 5;
                    chipCol = 6;
                    vaccineCol = 7;
                }

                bool found = false;

                foreach (var row in range.RowsUsed())
                {
                    if (row.RowNumber() == 1)
                    {
                        continue;
                    }

                    string excelPetId = petIdCol > 0 ? row.Cell(petIdCol).GetString().Trim() : string.Empty;
                    string excelPetName = row.Cell(nameCol).GetString().Trim();
                    string excelAnimalType = row.Cell(typeCol).GetString().Trim();
                    string excelWeight = row.Cell(weightCol).GetString().Trim();
                    string excelBirthDate = row.Cell(birthCol).GetString().Trim();
                    string excelOwner = row.Cell(ownerCol).GetString().Trim();
                    string excelChipNumber = row.Cell(chipCol).GetString().Trim();
                    string excelLastVaccineDate = row.Cell(vaccineCol).GetString().Trim();

                    bool matchByName =
                        !string.IsNullOrWhiteSpace(petName) &&
                        excelPetName.IndexOf(petName, StringComparison.OrdinalIgnoreCase) >= 0;

                    bool matchByChip =
                        !string.IsNullOrWhiteSpace(chipNumber) &&
                        excelChipNumber.IndexOf(chipNumber, StringComparison.OrdinalIgnoreCase) >= 0;

                    if (matchByName || matchByChip)
                    {
                        if (_vetWorkflow)
                        {
                            dgvPets.Rows.Add(
                                excelPetId,
                                excelPetName,
                                excelAnimalType,
                                excelWeight,
                                excelBirthDate,
                                excelOwner,
                                excelChipNumber,
                                excelLastVaccineDate);
                        }
                        else
                        {
                            dgvPets.Rows.Add(
                                excelPetName,
                                excelAnimalType,
                                excelWeight,
                                excelBirthDate,
                                excelOwner,
                                excelChipNumber,
                                excelLastVaccineDate);
                        }

                        found = true;
                    }
                }

                if (!found)
                {
                    MessageBox.Show("No pets found.");
                }
            }
        }

        private void BtnOpenVisit_Click(object sender, EventArgs e)
        {
            if (dgvPets.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a pet first.");
                return;
            }

            string petId = dgvPets.SelectedRows[0].Cells[0].Value?.ToString()?.Trim();
            if (string.IsNullOrEmpty(petId))
            {
                MessageBox.Show("Selected pet has no Pet ID.");
                return;
            }

            var openVisit = PetNavigationHooks.OpenVisitForPet;
            if (openVisit == null)
            {
                MessageBox.Show("Visit navigation is not configured. Start ClinicVets from the main application.");
                return;
            }

            openVisit(this, petId);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
