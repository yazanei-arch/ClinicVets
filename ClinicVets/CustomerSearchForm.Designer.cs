
namespace ClinicVets
{
    public partial class CustomerSearchForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlCard = new ClinicVets.CardPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblSearchType = new System.Windows.Forms.Label();
            this.cmbSearchType = new System.Windows.Forms.ComboBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearchError = new System.Windows.Forms.Label();
            this.lblNoResults = new System.Windows.Forms.Label();
            this.btnSearch = new ClinicVets.RoundedActionButton();
            this.btnClear = new ClinicVets.RoundedActionButton();
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.btnBack = new ClinicVets.RoundedActionButton();
            this.pnlCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pnlCard.AutoScroll = true;
            this.pnlCard.BackColor = System.Drawing.Color.Transparent;
            this.pnlCard.Controls.Add(this.lblTitle);
            this.pnlCard.Controls.Add(this.lblSubtitle);
            this.pnlCard.Controls.Add(this.lblSearchType);
            this.pnlCard.Controls.Add(this.cmbSearchType);
            this.pnlCard.Controls.Add(this.lblSearch);
            this.pnlCard.Controls.Add(this.txtSearch);
            this.pnlCard.Controls.Add(this.lblSearchError);
            this.pnlCard.Controls.Add(this.lblNoResults);
            this.pnlCard.Controls.Add(this.btnSearch);
            this.pnlCard.Controls.Add(this.btnClear);
            this.pnlCard.Controls.Add(this.dgvResults);
            this.pnlCard.Controls.Add(this.btnBack);
            this.pnlCard.Location = new System.Drawing.Point(274, 40);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Padding = new System.Windows.Forms.Padding(44, 24, 44, 28);
            this.pnlCard.Size = new System.Drawing.Size(540, 580);
            this.pnlCard.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(101)))), ((int)(((byte)(192)))));
            this.lblTitle.Location = new System.Drawing.Point(44, 24);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(452, 36);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Customer Search";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(78)))), ((int)(((byte)(96)))));
            this.lblSubtitle.Location = new System.Drawing.Point(44, 62);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(452, 48);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Search by Customer ID or Phone. Enter the full value for the selected type.";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSearchType
            // 
            this.lblSearchType.AutoSize = true;
            this.lblSearchType.Location = new System.Drawing.Point(44, 124);
            this.lblSearchType.Name = "lblSearchType";
            this.lblSearchType.Size = new System.Drawing.Size(94, 23);
            this.lblSearchType.TabIndex = 2;
            this.lblSearchType.Text = "Search Type";
            // 
            // cmbSearchType
            // 
            this.cmbSearchType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSearchType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbSearchType.FormattingEnabled = true;
            this.cmbSearchType.Location = new System.Drawing.Point(44, 150);
            this.cmbSearchType.Name = "cmbSearchType";
            this.cmbSearchType.Size = new System.Drawing.Size(452, 31);
            this.cmbSearchType.TabIndex = 3;
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(44, 192);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(61, 23);
            this.lblSearch.TabIndex = 4;
            this.lblSearch.Text = "Search";
            // 
            // txtSearch
            // 
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Enabled = true;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSearch.Location = new System.Drawing.Point(44, 216);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.ReadOnly = false;
            this.txtSearch.Size = new System.Drawing.Size(452, 30);
            this.txtSearch.TabIndex = 5;
            this.txtSearch.TabStop = true;
            // 
            // lblSearchError
            // 
            this.lblSearchError.AutoSize = false;
            this.lblSearchError.BackColor = System.Drawing.Color.Transparent;
            this.lblSearchError.Location = new System.Drawing.Point(44, 254);
            this.lblSearchError.Name = "lblSearchError";
            this.lblSearchError.Size = new System.Drawing.Size(452, 18);
            this.lblSearchError.TabIndex = 6;
            this.lblSearchError.TabStop = false;
            this.lblSearchError.Visible = false;
            // 
            // lblNoResults
            // 
            this.lblNoResults.AutoSize = false;
            this.lblNoResults.Location = new System.Drawing.Point(44, 318);
            this.lblNoResults.Name = "lblNoResults";
            this.lblNoResults.Size = new System.Drawing.Size(452, 18);
            this.lblNoResults.TabIndex = 7;
            this.lblNoResults.Visible = false;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(44, 278);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(220, 40);
            this.btnSearch.TabIndex = 8;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnClear
            // 
            this.btnClear.IsOutlineStyle = true;
            this.btnClear.Location = new System.Drawing.Point(276, 278);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(220, 40);
            this.btnClear.TabIndex = 9;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // dgvResults
            // 
            this.dgvResults.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(255)))));
            this.dgvResults.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Location = new System.Drawing.Point(44, 342);
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.RowHeadersVisible = false;
            this.dgvResults.Size = new System.Drawing.Size(452, 200);
            this.dgvResults.TabIndex = 10;
            // 
            // btnBack
            // 
            this.btnBack.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnBack.IsOutlineStyle = true;
            this.btnBack.Location = new System.Drawing.Point(44, 554);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(452, 40);
            this.btnBack.TabIndex = 11;
            this.btnBack.Text = "Back";
            // 
            // CustomerSearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.CancelButton = this.btnBack;
            this.ClientSize = new System.Drawing.Size(1024, 680);
            this.Controls.Add(this.pnlCard);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "CustomerSearchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ClinicVets — Customer Search";
            this.Load += new System.EventHandler(this.CustomerSearchForm_Load);
            this.pnlCard.ResumeLayout(false);
            this.pnlCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private CardPanel pnlCard;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblSearchType;
        private System.Windows.Forms.ComboBox cmbSearchType;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSearchError;
        private System.Windows.Forms.Label lblNoResults;
        private RoundedActionButton btnSearch;
        private RoundedActionButton btnClear;
        private System.Windows.Forms.DataGridView dgvResults;
        private RoundedActionButton btnBack;
    }
}
