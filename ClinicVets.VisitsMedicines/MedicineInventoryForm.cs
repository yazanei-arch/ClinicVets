using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ClinicVets.VisitsMedicines
{
    public partial class Form1 : Form
    {
        private List<Medicine> medicines = new List<Medicine>();
        private string filePath = @"C:\Users\sohel\Desktop\ClinicVets-main_2\ClinicVets-main\ClinicVetsData.xlsx";
        TextBox txtName = new TextBox();
        NumericUpDown numQuantity = new NumericUpDown();
        NumericUpDown numPrice = new NumericUpDown();
        DateTimePicker dtpExpiry = new DateTimePicker();
        DataGridView dgvMedicines = new DataGridView();

        public Form1()
        {
            InitializeComponent();
            BuildDesign();
            LoadMedicines();
            RefreshTable();
        }

        private void BuildDesign()
        {
            this.Text = "Medicine Inventory";
            this.Width = 1050;
            this.Height = 680;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new System.Drawing.Font("Segoe UI", 10);


            Label title = new Label()
            {
                Text = "Clinic Medicine Inventory",
                Left = 250,
                Top = 40,
                Width = 550,
                Height = 60,
                Font = new System.Drawing.Font("Segoe UI", 26, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.Transparent,
                ForeColor = System.Drawing.Color.FromArgb(10, 70, 120),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            Panel inputPanel = new Panel()
            {
                Left = 150,
                Top = 145,
                Width = 740,
                Height = 170,
                BackColor = System.Drawing.Color.FromArgb(230, 255, 255, 255)
            };

            Label lblName = new Label()
            {
                Text = "Medicine Name",
                Left = 40,
                Top = 30,
                Width = 130,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.Transparent,
                ForeColor = System.Drawing.Color.FromArgb(20, 70, 120)
            };

            txtName.Left = 180;
            txtName.Top = 25;
            txtName.Width = 210;
            txtName.Height = 30;

            Label lblQty = new Label()
            {
                Text = "Quantity",
                Left = 40,
                Top = 80,
                Width = 130,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.Transparent,
                ForeColor = System.Drawing.Color.FromArgb(20, 70, 120)
            };

            numQuantity.Left = 180;
            numQuantity.Top = 75;
            numQuantity.Width = 210;
            numQuantity.Minimum = 1;
            numQuantity.Maximum = 10000;

            Label lblPrice = new Label()
            {
                Text = "Price",
                Left = 430,
                Top = 30,
                Width = 100,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.Transparent,
                ForeColor = System.Drawing.Color.FromArgb(20, 70, 120)
            };

            numPrice.Left = 530;
            numPrice.Top = 25;
            numPrice.Width = 170;
            numPrice.DecimalPlaces = 2;

            Label lblExpiry = new Label()
            {
                Text = "Expiry Date",
                Left = 430,
                Top = 80,
                Width = 100,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.Transparent,
                ForeColor = System.Drawing.Color.FromArgb(20, 70, 120)
            };

            dtpExpiry.Left = 530;
            dtpExpiry.Top = 75;
            dtpExpiry.Width = 170;

            Button btnAdd = new Button()
            {
                Text = "Add / Update",
                Left = 180,
                Top = 120,
                Width = 210,
                Height = 38,
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(20, 100, 180),
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
            };

            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;

            Button btnRemove = new Button()
            {
                Text = "Remove Selected",
                Left = 530,
                Top = 120,
                Width = 170,
                Height = 38,
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(180, 50, 50),
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold)
            };

            btnRemove.FlatAppearance.BorderSize = 0;
            btnRemove.Click += BtnRemove_Click;

            inputPanel.Controls.Add(lblName);
            inputPanel.Controls.Add(txtName);

            inputPanel.Controls.Add(lblQty);
            inputPanel.Controls.Add(numQuantity);

            inputPanel.Controls.Add(lblPrice);
            inputPanel.Controls.Add(numPrice);

            inputPanel.Controls.Add(lblExpiry);
            inputPanel.Controls.Add(dtpExpiry);

            inputPanel.Controls.Add(btnAdd);
            inputPanel.Controls.Add(btnRemove);

            dgvMedicines.Left = 110;
            dgvMedicines.Top = 360;
            dgvMedicines.Width = 820;
            dgvMedicines.Height = 230;

            dgvMedicines.BackgroundColor = System.Drawing.Color.White;
            dgvMedicines.BorderStyle = BorderStyle.None;
            dgvMedicines.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMedicines.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMedicines.ReadOnly = true;
            dgvMedicines.AllowUserToAddRows = false;
            dgvMedicines.RowHeadersVisible = false;

            dgvMedicines.EnableHeadersVisualStyles = false;
            dgvMedicines.ColumnHeadersHeight = 35;

            dgvMedicines.ColumnHeadersDefaultCellStyle.BackColor =
                System.Drawing.Color.FromArgb(15, 90, 160);

            dgvMedicines.ColumnHeadersDefaultCellStyle.ForeColor =
                System.Drawing.Color.White;

            dgvMedicines.ColumnHeadersDefaultCellStyle.Font =
                new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);

            this.Controls.Add(title);
            this.Controls.Add(inputPanel);
            this.Controls.Add(dgvMedicines);
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();

            if (name == "")
            {
                MessageBox.Show("Please enter medicine name.");
                return;
            }

            Medicine existingMedicine =
                medicines.FirstOrDefault(m =>
                m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (existingMedicine != null)
            {
                existingMedicine.Quantity += (int)numQuantity.Value;
                existingMedicine.Price = numPrice.Value;
                existingMedicine.ExpiryDate = dtpExpiry.Value.Date;

                MessageBox.Show("Medicine updated.");
            }
            else
            {
                medicines.Add(new Medicine()
                {
                    Name = name,
                    Quantity = (int)numQuantity.Value,
                    Price = numPrice.Value,
                    ExpiryDate = dtpExpiry.Value.Date
                });

                MessageBox.Show("Medicine added.");
            }

            SaveMedicines();
            RefreshTable();
            ClearInputs();
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            if (dgvMedicines.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select medicine.");
                return;
            }

            string selectedName =
                dgvMedicines.SelectedRows[0]
                .Cells["Name"]
                .Value
                .ToString();

            Medicine selectedMedicine =
                medicines.FirstOrDefault(m => m.Name == selectedName);

            if (selectedMedicine == null)
                return;

            DialogResult result = MessageBox.Show(
                "Do you want to remove ALL stock?\n\n" +
                "YES = Remove everything\n" +
                "NO = Remove specific quantity",
                "Remove Medicine",
                MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Yes)
            {
                medicines.Remove(selectedMedicine);

                MessageBox.Show("Medicine removed.");
            }
            else if (result == DialogResult.No)
            {
                string input =
                    Microsoft.VisualBasic.Interaction.InputBox(
                        "Enter quantity to remove:",
                        "Remove Quantity",
                        "1");

                if (!int.TryParse(input, out int removeQty))
                {
                    MessageBox.Show("Invalid quantity.");
                    return;
                }

                if (removeQty <= 0)
                {
                    MessageBox.Show("Quantity must be greater than 0.");
                    return;
                }

                if (removeQty > selectedMedicine.Quantity)
                {
                    MessageBox.Show("Not enough stock.");
                    return;
                }

                selectedMedicine.Quantity -= removeQty;

                if (selectedMedicine.Quantity == 0)
                {
                    medicines.Remove(selectedMedicine);
                }

                MessageBox.Show("Stock updated.");
            }
            else
            {
                return;
            }

            SaveMedicines();
            RefreshTable();
        }

        private void RefreshTable()
        {
            dgvMedicines.DataSource = null;

            dgvMedicines.DataSource = medicines.Select(m => new
            {
                m.Name,
                m.Quantity,
                m.Price,
                ExpiryDate = m.ExpiryDate.ToShortDateString(),
                Status =
                    m.ExpiryDate < DateTime.Today
                    ? "Expired"
                    : m.Quantity <= 5
                    ? "Low Stock"
                    : "Available"
            }).ToList();
        }

        private void ClearInputs()
        {
            txtName.Clear();
            numQuantity.Value = 1;
            numPrice.Value = 0;
            dtpExpiry.Value = DateTime.Today;
        }

        private void SaveMedicines()
        {
            XLWorkbook workbook;

            if (File.Exists(filePath))
                workbook = new XLWorkbook(filePath);
            else
                workbook = new XLWorkbook();

            IXLWorksheet worksheet;

            if (workbook.Worksheets.Contains("Medicines"))
                worksheet = workbook.Worksheet("Medicines");
            else
                worksheet = workbook.Worksheets.Add("Medicines");

            worksheet.Range("A2:D1000").Clear();

            worksheet.Cell(1, 1).Value = "StockQuantity";
            worksheet.Cell(1, 2).Value = "Price";
            worksheet.Cell(1, 3).Value = "MedicineName";
            worksheet.Cell(1, 4).Value = "MedicineID";

            for (int i = 0; i < medicines.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = medicines[i].Quantity;
                worksheet.Cell(i + 2, 2).Value = medicines[i].Price;
                worksheet.Cell(i + 2, 3).Value = medicines[i].Name;
                worksheet.Cell(i + 2, 4).Value = i + 1;
            }

            worksheet.Columns().AdjustToContents();

            workbook.SaveAs(filePath);
        }

        private void LoadMedicines()
        {
            medicines.Clear();

            if (!File.Exists(filePath))
                return;

            XLWorkbook workbook = new XLWorkbook(filePath);

            if (!workbook.Worksheets.Contains("Medicines"))
                return;

            IXLWorksheet worksheet = workbook.Worksheet("Medicines");

            var rows = worksheet.RowsUsed().Skip(1);

            foreach (var row in rows)
            {
                medicines.Add(new Medicine()
                {
                    Quantity = row.Cell(1).GetValue<int>(),
                    Price = row.Cell(2).GetValue<decimal>(),
                    Name = row.Cell(3).GetString(),
                    ExpiryDate = DateTime.Today.AddMonths(6)
                });
            }
        }
    }

    public class Medicine
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}