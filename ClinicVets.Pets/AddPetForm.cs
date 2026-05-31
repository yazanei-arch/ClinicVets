using System;
using System.IO;
using ClosedXML.Excel;
using System.Drawing;
using System.Windows.Forms;
using ClinicVets;

namespace ClinicVets.UI
{
    public partial class AddPetForm : Form
    {
        private readonly string filePath = ExcelFileManager.FilePath;

        private Label lblPetNameError = new Label();
        private Label lblAnimalTypeError = new Label();
        private Label lblWeightError = new Label();
        private Label lblBirthDateError = new Label();
        private Label lblOwnerError = new Label();
        private Label lblChipNumberError = new Label();
        private Label lblLastVaccineError = new Label();

        private readonly string _ownerId;

        public AddPetForm()
            : this(null)
        {
        }

        public AddPetForm(string ownerId)
        {
            _ownerId = string.IsNullOrWhiteSpace(ownerId) ? null : ownerId.Trim();
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Normal;
            Shown += AddPetForm_Shown;
        }

        private void AddPetForm_Shown(object sender, EventArgs e)
        {
            StartPosition = FormStartPosition.CenterScreen;
            CenterToScreen();
        }

        private void AddPetForm_Load(object sender, EventArgs e)
        {
            dtpBirthDate.MaxDate = DateTime.Now;
            dtpLastVaccineDate.MaxDate = DateTime.Today;

            ClinicFormLayout.ApplyStandard(this);
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Normal;
            PetBackgroundHelper.ApplyToPictureBox(pictureBox1);
            if (pictureBox1 != null)
            {
                pictureBox1.SendToBack();
            }

            EnsureErrorLabels();
            ApplyAddPetContentLayout();
            ApplyTransparentLabels();
            SafeLoadAnimalTypes();
            SafeLoadCustomerOwners();
            if (!string.IsNullOrEmpty(_ownerId))
            {
                int preselect = txtOwner.Items.IndexOf(_ownerId);
                if (preselect >= 0)
                {
                    txtOwner.SelectedIndex = preselect;
                }
            }

            Resize += AddPetForm_Resize;
        }

        private void AddPetForm_Resize(object sender, EventArgs e)
        {
            ApplyAddPetContentLayout();
        }

        private void ApplyAddPetContentLayout()
        {
            const int logoClearanceBottom = 182;
            const int columnWidth = 268;
            const int columnGap = 44;
            const int rowStride = 78;
            const int captionHeight = 24;
            const int captionToInputGap = 4;
            const int inputHeight = 36;
            const int subtitleWidth = 560;
            const int buttonGap = 28;
            const int bottomMargin = 48;

            int centerX = ClientSize.Width / 2;
            int blockWidth = (columnWidth * 2) + columnGap;
            int colStartX = centerX - (blockWidth / 2);
            int colEndX = colStartX + columnWidth + columnGap;
            int petColX = colStartX;
            int dateColX = colEndX;

            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            int titleWidth = TextRenderer.MeasureText(label1.Text, label1.Font).Width + 8;
            label1.Location = new Point(centerX - (titleWidth / 2), logoClearanceBottom);

            label9.AutoSize = false;
            label9.BackColor = Color.Transparent;
            label9.TextAlign = ContentAlignment.TopCenter;
            label9.SetBounds(centerX - (subtitleWidth / 2), label1.Bottom + 10, subtitleWidth, 28);

            int rowTop = label9.Bottom + 22;
            int petY = rowTop;
            LayoutStackedField(label2, txtPetName, petColX, petY, columnWidth, captionHeight, captionToInputGap, inputHeight);
            petY += rowStride;
            LayoutStackedField(label3, cmbAnimalType, petColX, petY, columnWidth, captionHeight, captionToInputGap, inputHeight);
            petY += rowStride;
            LayoutStackedField(label4, txtWeight, petColX, petY, columnWidth, captionHeight, captionToInputGap, inputHeight);
            petY += rowStride;
            LayoutStackedField(label7, txtChipNumber, petColX, petY, columnWidth, captionHeight, captionToInputGap, inputHeight);
            int petColBottom = petY + captionHeight + captionToInputGap + inputHeight;

            int dateY = rowTop;
            LayoutStackedField(label5, dtpBirthDate, dateColX, dateY, columnWidth, captionHeight, captionToInputGap, inputHeight);
            dateY += rowStride;
            LayoutStackedField(label8, dtpLastVaccineDate, dateColX, dateY, columnWidth, captionHeight, captionToInputGap, inputHeight);
            dateY += rowStride;
            LayoutStackedField(label6, txtOwner, dateColX, dateY, columnWidth, captionHeight, captionToInputGap, inputHeight);
            int dateColBottom = dateY + captionHeight + captionToInputGap + inputHeight;

            RepositionAllErrorLabels();

            int fieldsBottom = Math.Max(petColBottom, dateColBottom);
            int buttonsY = Math.Min(fieldsBottom + 32, ClientSize.Height - bottomMargin - btnSave.Height);
            int buttonsWidth = btnSave.Width + buttonGap + btnClear.Width;
            int buttonsLeft = centerX - (buttonsWidth / 2);
            btnSave.Location = new Point(buttonsLeft, buttonsY);
            btnClear.Location = new Point(buttonsLeft + btnSave.Width + buttonGap, buttonsY);

            PlaceAddPetBackButtonTopLeft();

            BringLayoutControlsToFront();
        }

        private void ApplyTransparentLabels()
        {
            if (pictureBox1 == null)
            {
                return;
            }

            foreach (Label label in new[] { label1, label9, label2, label3, label4, label5, label6, label7, label8 })
            {
                if (label == null)
                {
                    continue;
                }

                if (label.Parent != pictureBox1)
                {
                    label.Parent = pictureBox1;
                }

                label.BackColor = Color.Transparent;
            }
        }

        private void LayoutStackedField(
            Label caption,
            Control input,
            int left,
            int top,
            int width,
            int captionHeight,
            int captionGap,
            int inputHeight)
        {
            if (pictureBox1 != null)
            {
                caption.Parent = pictureBox1;
                input.Parent = pictureBox1;
            }

            caption.AutoSize = false;
            caption.BackColor = Color.Transparent;
            caption.TextAlign = ContentAlignment.MiddleLeft;
            caption.SetBounds(left, top, width, captionHeight);

            input.SetBounds(left, top + captionHeight + captionGap, width, inputHeight);
        }

        private void RepositionAllErrorLabels()
        {
            PositionErrorLabel(lblPetNameError, txtPetName);
            PositionErrorLabel(lblAnimalTypeError, cmbAnimalType);
            PositionErrorLabel(lblWeightError, txtWeight);
            PositionErrorLabel(lblChipNumberError, txtChipNumber);
            PositionErrorLabel(lblBirthDateError, dtpBirthDate);
            PositionErrorLabel(lblLastVaccineError, dtpLastVaccineDate);
            PositionErrorLabel(lblOwnerError, txtOwner);
        }

        private static void PositionErrorLabel(Label label, Control input)
        {
            if (label == null || input == null)
            {
                return;
            }

            Control host = input.Parent;
            if (host != null && label.Parent != host)
            {
                host.Controls.Add(label);
            }

            label.Location = new Point(input.Left, input.Bottom + 3);
        }

        private void BringLayoutControlsToFront()
        {
            btnSave.BringToFront();
            btnClear.BringToFront();
            btnBack.BringToFront();
            label1.BringToFront();
            label9.BringToFront();
            label2.BringToFront();
            label3.BringToFront();
            label4.BringToFront();
            label5.BringToFront();
            label6.BringToFront();
            label7.BringToFront();
            label8.BringToFront();
            txtPetName.BringToFront();
            cmbAnimalType.BringToFront();
            txtWeight.BringToFront();
            txtChipNumber.BringToFront();
            dtpBirthDate.BringToFront();
            dtpLastVaccineDate.BringToFront();
            txtOwner.BringToFront();
            lblPetNameError.BringToFront();
            lblAnimalTypeError.BringToFront();
            lblWeightError.BringToFront();
            lblChipNumberError.BringToFront();
            lblBirthDateError.BringToFront();
            lblLastVaccineError.BringToFront();
            lblOwnerError.BringToFront();
        }

        private void PlaceAddPetBackButtonTopLeft()
        {
            const int margin = 16;
            const int top = 12;
            int x = RightToLeftLayout && RightToLeft == RightToLeft.Yes
                ? ClientSize.Width - btnBack.Width - margin
                : margin;
            btnBack.Location = new Point(x, top);
            btnBack.BringToFront();
        }

        private void SafeLoadAnimalTypes()
        {
            try
            {
                LoadAnimalTypes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not load animal types from Excel." + Environment.NewLine + Environment.NewLine + ex.Message,
                    "ClinicVets",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void SafeLoadCustomerOwners()
        {
            try
            {
                LoadCustomerOwners();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not load customers from Excel." + Environment.NewLine + Environment.NewLine + ex.Message,
                    "ClinicVets",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void LoadCustomerOwners()
        {
            txtOwner.Items.Clear();
            txtOwner.DropDownStyle = ComboBoxStyle.DropDownList;

            if (!PetExcelSupport.TryEnsureWorkbookReady(this, out string workbookPath))
            {
                return;
            }

            using (var workbook = new XLWorkbook(workbookPath))
            {
                const string customerSheetName = "Customer";
                if (!workbook.Worksheets.Contains(customerSheetName))
                {
                    return;
                }

                var sheet = workbook.Worksheet(customerSheetName);
                var range = sheet.RangeUsed();
                if (range == null)
                {
                    return;
                }

                int fullNameColumn = -1;
                var headerRow = range.FirstRowUsed();
                if (headerRow == null)
                {
                    return;
                }

                foreach (var headerCell in headerRow.CellsUsed())
                {
                    string header = headerCell.GetValue<string>().Trim();
                    if (string.Equals(header, "FullName", StringComparison.OrdinalIgnoreCase))
                    {
                        fullNameColumn = headerCell.Address.ColumnNumber;
                        break;
                    }
                }

                if (fullNameColumn <= 0)
                {
                    return;
                }

                var added = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var row in range.RowsUsed())
                {
                    if (row.RowNumber() == headerRow.RowNumber())
                    {
                        continue;
                    }

                    string name = row.Cell(fullNameColumn).GetValue<string>().Trim();
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        continue;
                    }

                    if (added.Add(name))
                    {
                        txtOwner.Items.Add(name);
                    }
                }
            }
        }

        private void EnsureErrorLabels()
        {
            SetupErrorLabel(lblPetNameError, txtPetName);
            SetupErrorLabel(lblAnimalTypeError, cmbAnimalType);
            SetupErrorLabel(lblWeightError, txtWeight);
            SetupErrorLabel(lblBirthDateError, dtpBirthDate);
            SetupErrorLabel(lblOwnerError, txtOwner);
            SetupErrorLabel(lblChipNumberError, txtChipNumber);
            SetupErrorLabel(lblLastVaccineError, dtpLastVaccineDate);
        }

        private void SetupErrorLabel(Label label, Control control)
        {
            label.Text = "";
            label.ForeColor = Color.Red;
            label.BackColor = Color.Transparent;
            label.AutoSize = true;
            label.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            label.Location = new Point(control.Left, control.Bottom + 3);

            Control host = control.Parent ?? this;
            if (!host.Controls.Contains(label))
            {
                host.Controls.Add(label);
            }

            label.BringToFront();
        }

        private void LoadAnimalTypes()
        {
            cmbAnimalType.Items.Clear();

            if (!PetExcelSupport.TryEnsureWorkbookReady(this, out string workbookPath))
            {
                return;
            }

            using (var workbook = new XLWorkbook(workbookPath))
            {
                if (!workbook.Worksheets.Contains("AnimalTypes"))
                {
                    MessageBox.Show("AnimalTypes sheet not found");
                    return;
                }

                var sheet = workbook.Worksheet("AnimalTypes");
                var range = sheet.RangeUsed();

                if (range == null)
                    return;

                foreach (var row in range.RowsUsed())
                {
                    if (row.RowNumber() == 1)
                        continue;

                    string animalType = row.Cell(1).GetValue<string>().Trim();

                    if (!string.IsNullOrWhiteSpace(animalType))
                        cmbAnimalType.Items.Add(animalType);
                }
            }

            cmbAnimalType.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void ClearErrorLabels()
        {
            lblPetNameError.Text = "";
            lblAnimalTypeError.Text = "";
            lblWeightError.Text = "";
            lblBirthDateError.Text = "";
            lblOwnerError.Text = "";
            lblChipNumberError.Text = "";
            lblLastVaccineError.Text = "";

            txtPetName.BackColor = Color.White;
            cmbAnimalType.BackColor = Color.White;
            txtWeight.BackColor = Color.White;
            txtChipNumber.BackColor = Color.White;
            txtOwner.BackColor = Color.White;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtPetName.Clear();
            cmbAnimalType.SelectedIndex = -1;
            txtWeight.Clear();
            txtChipNumber.Clear();
            txtOwner.SelectedIndex = -1;

            dtpBirthDate.Value = DateTime.Now;
            dtpLastVaccineDate.Value = DateTime.Now;

            ClearErrorLabels();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ClearErrorLabels();

            bool isValid = true;

            string petName = txtPetName.Text.Trim();
            string animalType = cmbAnimalType.Text.Trim();
            string weightText = txtWeight.Text.Trim();
            string chipNumber = txtChipNumber.Text.Trim();
            string owner = txtOwner.Text.Trim();

            if (string.IsNullOrWhiteSpace(petName))
            {
                lblPetNameError.Text = "Pet name is required.";
                txtPetName.BackColor = Color.MistyRose;
                isValid = false;
            }
            else
            {
                foreach (char c in petName)
                {
                    if (!char.IsLetter(c) && c != ' ')
                    {
                        lblPetNameError.Text = "Only letters are allowed.";
                        txtPetName.BackColor = Color.MistyRose;
                        isValid = false;
                        break;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(animalType))
            {
                lblAnimalTypeError.Text = "Select animal type.";
                cmbAnimalType.BackColor = Color.MistyRose;
                isValid = false;
            }

            double weight = 0;

            if (string.IsNullOrWhiteSpace(weightText))
            {
                lblWeightError.Text = "Weight is required.";
                txtWeight.BackColor = Color.MistyRose;
                isValid = false;
            }
            else if (!double.TryParse(weightText, out weight))
            {
                lblWeightError.Text = "Weight must be a number.";
                txtWeight.BackColor = Color.MistyRose;
                isValid = false;
            }
            else if (weight < 0.1 || weight > 100)
            {
                lblWeightError.Text = "Weight must be 0.1 - 100.";
                txtWeight.BackColor = Color.MistyRose;
                isValid = false;
            }

            if (dtpBirthDate.Value.Date > DateTime.Now.Date)
            {
                lblBirthDateError.Text = "Birth date cannot be future.";
                isValid = false;
            }
            else if (dtpBirthDate.Value.Year < 2000)
            {
                lblBirthDateError.Text = "Birth date cannot be before 2000.";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(owner))
            {
                lblOwnerError.Text = "Owner is required.";
                txtOwner.BackColor = Color.MistyRose;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(chipNumber))
            {
                lblChipNumberError.Text = "Chip number is required.";
                txtChipNumber.BackColor = Color.MistyRose;
                isValid = false;
            }

            if (dtpLastVaccineDate.Value.Date > DateTime.Now.Date)
            {
                lblLastVaccineError.Text = "Vaccine date cannot be future.";
                isValid = false;
            }

            if (!isValid)
                return;

            PetRepository repo = new PetRepository();

            Pet pet = new Pet
            {
                PetID = repo.GeneratePetID(),
                PetName = petName,
                AnimalType = animalType,
                Weight = weight,
                BirthDate = dtpBirthDate.Value.Date,
                Owner = owner,
                ChipNumber = chipNumber,
                LastVaccineDate = dtpLastVaccineDate.Value.Date
            };

            repo.AddPet(pet);
            SavePetToExcel(pet);

            MessageBox.Show("Pet saved successfully.");
            btnClear_Click(sender, e);
        }

        private void SavePetToExcel(Pet pet)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Excel file not found");
                return;
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                IXLWorksheet sheet;

                if (workbook.Worksheets.Contains("Pets"))
                {
                    sheet = workbook.Worksheet("Pets");
                }
                else
                {
                    sheet = workbook.Worksheets.Add("Pets");

                    sheet.Cell(1, 1).Value = "PetID";
                    sheet.Cell(1, 2).Value = "PetName";
                    sheet.Cell(1, 3).Value = "AnimalType";
                    sheet.Cell(1, 4).Value = "Weight";
                    sheet.Cell(1, 5).Value = "BirthDate";
                    sheet.Cell(1, 6).Value = "Owner";
                    sheet.Cell(1, 7).Value = "ChipNumber";
                    sheet.Cell(1, 8).Value = "LastVaccineDate";
                }

                int lastRow;

                if (sheet.LastRowUsed() == null)
                    lastRow = 2;
                else
                    lastRow = sheet.LastRowUsed().RowNumber() + 1;

                sheet.Cell(lastRow, 1).Value = pet.PetID;
                sheet.Cell(lastRow, 2).Value = pet.PetName;
                sheet.Cell(lastRow, 3).Value = pet.AnimalType;
                sheet.Cell(lastRow, 4).Value = pet.Weight;
                sheet.Cell(lastRow, 5).Value = pet.BirthDate;
                sheet.Cell(lastRow, 6).Value = pet.Owner;
                sheet.Cell(lastRow, 7).Value = pet.ChipNumber;
                sheet.Cell(lastRow, 8).Value = pet.LastVaccineDate;

                workbook.Save();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        
    }
}