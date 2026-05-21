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

namespace ClinicVets.UI
{
    public partial class AnimalTypesForm : Form
    {
        private readonly string filePath = ExcelFileManager.FilePath;

        public AnimalTypesForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Normal;
            Shown += AnimalTypesForm_Shown;
        }

        private void AnimalTypesForm_Shown(object sender, EventArgs e)
        {
            StartPosition = FormStartPosition.CenterScreen;
            CenterToScreen();
        }

        private void AnimalTypesForm_Load(object sender, EventArgs e)
        {
            ClinicFormLayout.ApplyStandard(this);
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Normal;
            PetBackgroundHelper.ApplyToPictureBox(pictureBox1);
            if (pictureBox1 != null)
            {
                pictureBox1.SendToBack();
            }

            ApplyAnimalTypesContentLayout();
            SafeLoadTypesFromExcel();
            Resize += AnimalTypesForm_Resize;
        }

        private void AnimalTypesForm_Resize(object sender, EventArgs e)
        {
            ApplyAnimalTypesContentLayout();
        }

        private void ApplyAnimalTypesContentLayout()
        {
            const int listWidth = 548;
            const int fieldWidth = 247;
            const int fieldHeight = 36;
            const int fieldLabelWidth = 140;
            const int fieldLabelHeight = 41;
            const int actionButtonWidth = 180;
            const int actionButtonHeight = 56;
            const int actionButtonGap = 16;
            const int contentShiftLeft = 48;
            const int bottomMargin = 52;

            int centerX = (ClientSize.Width / 2) - contentShiftLeft;

            const int logoClearanceBottom = 168;
            const int titleTop = logoClearanceBottom;
            const int gapTitleToSubtitle = 10;
            const int gapSubtitleToInput = 26;
            const int gapInputToActions = 14;
            const int gapActionsToList = 22;

            const int captionPadH = 10;
            const int captionPadV = 4;

            LayoutFittedCaptionLabel(label1, centerX, titleTop, captionPadH, captionPadV);
            int subtitleTop = label1.Bottom + gapTitleToSubtitle;
            LayoutFittedCaptionLabel(label2, centerX, subtitleTop, captionPadH, captionPadV);

            int inputTop = label2.Bottom + gapSubtitleToInput;
            int inputRowHeight = Math.Max(fieldHeight, fieldLabelHeight);
            int actionsTop = inputTop + inputRowHeight + gapInputToActions;
            int listTop = actionsTop + actionButtonHeight + gapActionsToList;

            int inputRowWidth = fieldWidth + 8 + fieldLabelWidth;
            int inputLeft = centerX - (inputRowWidth / 2);
            int fieldTop = inputTop + ((inputRowHeight - fieldHeight) / 2);
            int fieldLabelTop = inputTop + ((inputRowHeight - fieldLabelHeight) / 2);

            txtType.Size = new Size(fieldWidth, fieldHeight);
            txtType.Location = new Point(inputLeft, fieldTop);

            button1.Size = new Size(fieldLabelWidth, fieldLabelHeight);
            button1.Location = new Point(inputLeft + fieldWidth + 8, fieldLabelTop);

            int actionsRowWidth = (actionButtonWidth * 3) + (actionButtonGap * 2);
            int actionsLeft = centerX - (actionsRowWidth / 2);
            btnDelete.Size = new Size(actionButtonWidth, actionButtonHeight);
            btnDelete.Location = new Point(actionsLeft, actionsTop);
            btnUpdate.Size = new Size(actionButtonWidth, actionButtonHeight);
            btnUpdate.Location = new Point(actionsLeft + actionButtonWidth + actionButtonGap, actionsTop);
            btnAdd.Size = new Size(actionButtonWidth, actionButtonHeight);
            btnAdd.Location = new Point(actionsLeft + (actionButtonWidth + actionButtonGap) * 2, actionsTop);

            int listHeight = Math.Min(228, ClientSize.Height - listTop - bottomMargin);
            listHeight = Math.Max(180, listHeight);

            lstTypes.Size = new Size(listWidth, listHeight);
            lstTypes.Location = new Point(centerX - (listWidth / 2), listTop);

            PlaceBackButtonTopLeft();
            lstTypes.BringToFront();
            txtType.BringToFront();
            button1.BringToFront();
            btnAdd.BringToFront();
            btnUpdate.BringToFront();
            btnDelete.BringToFront();
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

        private static void LayoutFittedCaptionLabel(Label label, int centerX, int top, int padH, int padV)
        {
            if (label == null)
            {
                return;
            }

            Size textSize = TextRenderer.MeasureText(
                label.Text,
                label.Font,
                new Size(int.MaxValue, int.MaxValue),
                TextFormatFlags.NoPadding | TextFormatFlags.SingleLine);

            int width = textSize.Width + (padH * 2);
            int height = textSize.Height + (padV * 2);
            label.AutoSize = false;
            label.TextAlign = ContentAlignment.MiddleCenter;
            label.SetBounds(centerX - (width / 2), top, width, height);
        }

        private void SafeLoadTypesFromExcel()
        {
            lstTypes.Items.Clear();

            if (!PetExcelSupport.TryEnsureWorkbookReady(this, out string workbookPath))
            {
                return;
            }

            LoadTypesFromExcel(workbookPath);
        }

        private void LoadTypesFromExcel(string workbookPath)
        {
            if (!File.Exists(workbookPath))
            {
                MessageBox.Show("Excel file not found", "ClinicVets", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var workbook = new XLWorkbook(workbookPath))
                {
                    if (!workbook.Worksheets.Contains("AnimalTypes"))
                    {
                        MessageBox.Show("AnimalTypes sheet not found", "ClinicVets", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                    string type = row.Cell(1).GetValue<string>();

                    if (!string.IsNullOrWhiteSpace(type))
                        lstTypes.Items.Add(type);
                }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not read animal types from Excel." + Environment.NewLine + Environment.NewLine + ex.Message,
                    "ClinicVets",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private bool TypeExists(string type)
        {
            foreach (var item in lstTypes.Items)
            {
                if (item.ToString().Trim().ToLower() == type.Trim().ToLower())
                    return true;
            }

            return false;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (lstTypes.SelectedItem == null)
            {
                MessageBox.Show("Please select animal type to update.");
                return;
            }

            string oldType = lstTypes.SelectedItem.ToString();
            string newType = txtType.Text.Trim();

            if (string.IsNullOrWhiteSpace(newType))
            {
                MessageBox.Show("Please enter new animal type.");
                return;
            }

            if (!File.Exists(filePath))
            {
                MessageBox.Show("Excel file not found");
                return;
            }

            try
            {
                using (var workbook = new XLWorkbook(filePath))
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

                        string currentType = row.Cell(1).GetValue<string>().Trim();

                        if (currentType.Equals(oldType, StringComparison.OrdinalIgnoreCase))
                        {
                            row.Cell(1).Value = newType;
                            break;
                        }
                    }

                    workbook.Save();
                }

                // כאן העדכון החשוב:
                UpdatePetsAnimalTypeInExcel(oldType, newType);

                txtType.Clear();
                SafeLoadTypesFromExcel();

                MessageBox.Show("Animal type updated successfully.");
            }
            catch (IOException)
            {
                MessageBox.Show("Please close the Excel file before updating.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while updating animal type: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstTypes.SelectedIndex == -1)
            {
                MessageBox.Show("Select type first");
                return;
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                var sheet = workbook.Worksheet("AnimalTypes");
                int rowNumber = lstTypes.SelectedIndex + 2;

                sheet.Row(rowNumber).Delete();
                workbook.Save();
            }

            txtType.Clear();
            SafeLoadTypesFromExcel();

            MessageBox.Show("Animal type deleted");
        }

        private void lstTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTypes.SelectedItem != null)
                txtType.Text = lstTypes.SelectedItem.ToString();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnDelete_Click_2(object sender, EventArgs e)
        {
            btnDelete_Click(sender, e);
        }

        private void btnUpdate_Click_2(object sender, EventArgs e)
        {
            btnUpdate_Click(sender, e);
        }

        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            string type = txtType.Text.Trim();

            if (type == "")
            {
                MessageBox.Show("Enter animal type");
                return;
            }

            if (TypeExists(type))
            {
                MessageBox.Show("Animal type already exists");
                return;
            }

            using (var workbook = new XLWorkbook(filePath))
            {
                var sheet = workbook.Worksheet("AnimalTypes");
                int lastRow = sheet.LastRowUsed().RowNumber() + 1;

                sheet.Cell(lastRow, 1).Value = type;
                workbook.Save();
            }

            txtType.Clear();
            SafeLoadTypesFromExcel();

            MessageBox.Show("Animal type added");
        }

        private void UpdatePetsAnimalTypeInExcel(string oldType, string newType)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Excel file not found");
                return;
            }

            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    if (!workbook.Worksheets.Contains("Pets"))
                        return;

                    var petsSheet = workbook.Worksheet("Pets");
                    var range = petsSheet.RangeUsed();

                    if (range == null)
                        return;

                    foreach (var row in range.RowsUsed())
                    {
                        if (row.RowNumber() == 1)
                            continue;

                        // לפי הסדר שלנו:
                        // A = PetID
                        // B = PetName
                        // C = AnimalType
                        string currentAnimalType = row.Cell(3).GetValue<string>().Trim();

                        if (currentAnimalType.Equals(oldType, StringComparison.OrdinalIgnoreCase))
                        {
                            row.Cell(3).Value = newType;
                        }
                    }

                    workbook.Save();
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Please close the Excel file before updating.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while updating pets animal type: " + ex.Message);
            }
        }
    }
}