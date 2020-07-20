namespace Trace.UI.UIs
{
    partial class TigtheningRepairsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TigtheningRepairsForm));
            this.dgvList = new System.Windows.Forms.DataGridView();
            this.tighteningRepairModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.noDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.minDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.minDescDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.maxDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.maxDescDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.targetDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.targetDescDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultDescDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.testResultDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.jointMinDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.jointMinDescDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.jointMaxDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.jointMaxDescDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.jointTargetDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.jointTargetDescDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.jointResultDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.jointResultDescDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.jointTestResultDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tighteningResultIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tighteningResultDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.idDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastUpdateDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastUpdatedByDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.creationDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.createdByDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tighteningRepairModelBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvList
            // 
            this.dgvList.AllowUserToAddRows = false;
            this.dgvList.AllowUserToDeleteRows = false;
            this.dgvList.AutoGenerateColumns = false;
            this.dgvList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvList.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvList.BackgroundColor = System.Drawing.Color.White;
            this.dgvList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.noDataGridViewTextBoxColumn,
            this.minDataGridViewTextBoxColumn,
            this.minDescDataGridViewTextBoxColumn,
            this.maxDataGridViewTextBoxColumn,
            this.maxDescDataGridViewTextBoxColumn,
            this.targetDataGridViewTextBoxColumn,
            this.targetDescDataGridViewTextBoxColumn,
            this.resultDataGridViewTextBoxColumn,
            this.resultDescDataGridViewTextBoxColumn,
            this.testResultDataGridViewTextBoxColumn,
            this.jointMinDataGridViewTextBoxColumn,
            this.jointMinDescDataGridViewTextBoxColumn,
            this.jointMaxDataGridViewTextBoxColumn,
            this.jointMaxDescDataGridViewTextBoxColumn,
            this.jointTargetDataGridViewTextBoxColumn,
            this.jointTargetDescDataGridViewTextBoxColumn,
            this.jointResultDataGridViewTextBoxColumn,
            this.jointResultDescDataGridViewTextBoxColumn,
            this.jointTestResultDataGridViewTextBoxColumn,
            this.tighteningResultIdDataGridViewTextBoxColumn,
            this.tighteningResultDataGridViewTextBoxColumn,
            this.idDataGridViewTextBoxColumn,
            this.lastUpdateDateDataGridViewTextBoxColumn,
            this.lastUpdatedByDataGridViewTextBoxColumn,
            this.creationDateDataGridViewTextBoxColumn,
            this.createdByDataGridViewTextBoxColumn});
            this.dgvList.DataSource = this.tighteningRepairModelBindingSource;
            this.dgvList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvList.Location = new System.Drawing.Point(0, 0);
            this.dgvList.Margin = new System.Windows.Forms.Padding(4);
            this.dgvList.Name = "dgvList";
            this.dgvList.ReadOnly = true;
            this.dgvList.RowHeadersVisible = false;
            this.dgvList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvList.Size = new System.Drawing.Size(1132, 171);
            this.dgvList.TabIndex = 26;
            // 
            // tighteningRepairModelBindingSource
            // 
            this.tighteningRepairModelBindingSource.DataSource = typeof(Trace.Domain.Models.TighteningRepairModel);
            // 
            // noDataGridViewTextBoxColumn
            // 
            this.noDataGridViewTextBoxColumn.DataPropertyName = "No";
            this.noDataGridViewTextBoxColumn.HeaderText = "No";
            this.noDataGridViewTextBoxColumn.Name = "noDataGridViewTextBoxColumn";
            this.noDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // minDataGridViewTextBoxColumn
            // 
            this.minDataGridViewTextBoxColumn.DataPropertyName = "Min";
            this.minDataGridViewTextBoxColumn.HeaderText = "Min";
            this.minDataGridViewTextBoxColumn.Name = "minDataGridViewTextBoxColumn";
            this.minDataGridViewTextBoxColumn.ReadOnly = true;
            this.minDataGridViewTextBoxColumn.Visible = false;
            // 
            // minDescDataGridViewTextBoxColumn
            // 
            this.minDescDataGridViewTextBoxColumn.DataPropertyName = "MinDesc";
            this.minDescDataGridViewTextBoxColumn.HeaderText = "Min.";
            this.minDescDataGridViewTextBoxColumn.Name = "minDescDataGridViewTextBoxColumn";
            this.minDescDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // maxDataGridViewTextBoxColumn
            // 
            this.maxDataGridViewTextBoxColumn.DataPropertyName = "Max";
            this.maxDataGridViewTextBoxColumn.HeaderText = "Max";
            this.maxDataGridViewTextBoxColumn.Name = "maxDataGridViewTextBoxColumn";
            this.maxDataGridViewTextBoxColumn.ReadOnly = true;
            this.maxDataGridViewTextBoxColumn.Visible = false;
            // 
            // maxDescDataGridViewTextBoxColumn
            // 
            this.maxDescDataGridViewTextBoxColumn.DataPropertyName = "MaxDesc";
            this.maxDescDataGridViewTextBoxColumn.HeaderText = "Max.";
            this.maxDescDataGridViewTextBoxColumn.Name = "maxDescDataGridViewTextBoxColumn";
            this.maxDescDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // targetDataGridViewTextBoxColumn
            // 
            this.targetDataGridViewTextBoxColumn.DataPropertyName = "Target";
            this.targetDataGridViewTextBoxColumn.HeaderText = "Target";
            this.targetDataGridViewTextBoxColumn.Name = "targetDataGridViewTextBoxColumn";
            this.targetDataGridViewTextBoxColumn.ReadOnly = true;
            this.targetDataGridViewTextBoxColumn.Visible = false;
            // 
            // targetDescDataGridViewTextBoxColumn
            // 
            this.targetDescDataGridViewTextBoxColumn.DataPropertyName = "TargetDesc";
            this.targetDescDataGridViewTextBoxColumn.HeaderText = "Target";
            this.targetDescDataGridViewTextBoxColumn.Name = "targetDescDataGridViewTextBoxColumn";
            this.targetDescDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // resultDataGridViewTextBoxColumn
            // 
            this.resultDataGridViewTextBoxColumn.DataPropertyName = "Result";
            this.resultDataGridViewTextBoxColumn.HeaderText = "Result";
            this.resultDataGridViewTextBoxColumn.Name = "resultDataGridViewTextBoxColumn";
            this.resultDataGridViewTextBoxColumn.ReadOnly = true;
            this.resultDataGridViewTextBoxColumn.Visible = false;
            // 
            // resultDescDataGridViewTextBoxColumn
            // 
            this.resultDescDataGridViewTextBoxColumn.DataPropertyName = "ResultDesc";
            this.resultDescDataGridViewTextBoxColumn.HeaderText = "Result";
            this.resultDescDataGridViewTextBoxColumn.Name = "resultDescDataGridViewTextBoxColumn";
            this.resultDescDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // testResultDataGridViewTextBoxColumn
            // 
            this.testResultDataGridViewTextBoxColumn.DataPropertyName = "TestResult";
            this.testResultDataGridViewTextBoxColumn.HeaderText = "[OK]";
            this.testResultDataGridViewTextBoxColumn.Name = "testResultDataGridViewTextBoxColumn";
            this.testResultDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // jointMinDataGridViewTextBoxColumn
            // 
            this.jointMinDataGridViewTextBoxColumn.DataPropertyName = "JointMin";
            this.jointMinDataGridViewTextBoxColumn.HeaderText = "Joint Min";
            this.jointMinDataGridViewTextBoxColumn.Name = "jointMinDataGridViewTextBoxColumn";
            this.jointMinDataGridViewTextBoxColumn.ReadOnly = true;
            this.jointMinDataGridViewTextBoxColumn.Visible = false;
            // 
            // jointMinDescDataGridViewTextBoxColumn
            // 
            this.jointMinDescDataGridViewTextBoxColumn.DataPropertyName = "JointMinDesc";
            this.jointMinDescDataGridViewTextBoxColumn.HeaderText = "Joint Min.";
            this.jointMinDescDataGridViewTextBoxColumn.Name = "jointMinDescDataGridViewTextBoxColumn";
            this.jointMinDescDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // jointMaxDataGridViewTextBoxColumn
            // 
            this.jointMaxDataGridViewTextBoxColumn.DataPropertyName = "JointMax";
            this.jointMaxDataGridViewTextBoxColumn.HeaderText = "JointMax";
            this.jointMaxDataGridViewTextBoxColumn.Name = "jointMaxDataGridViewTextBoxColumn";
            this.jointMaxDataGridViewTextBoxColumn.ReadOnly = true;
            this.jointMaxDataGridViewTextBoxColumn.Visible = false;
            // 
            // jointMaxDescDataGridViewTextBoxColumn
            // 
            this.jointMaxDescDataGridViewTextBoxColumn.DataPropertyName = "JointMaxDesc";
            this.jointMaxDescDataGridViewTextBoxColumn.HeaderText = "Joint Max.";
            this.jointMaxDescDataGridViewTextBoxColumn.Name = "jointMaxDescDataGridViewTextBoxColumn";
            this.jointMaxDescDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // jointTargetDataGridViewTextBoxColumn
            // 
            this.jointTargetDataGridViewTextBoxColumn.DataPropertyName = "JointTarget";
            this.jointTargetDataGridViewTextBoxColumn.HeaderText = "JointTarget";
            this.jointTargetDataGridViewTextBoxColumn.Name = "jointTargetDataGridViewTextBoxColumn";
            this.jointTargetDataGridViewTextBoxColumn.ReadOnly = true;
            this.jointTargetDataGridViewTextBoxColumn.Visible = false;
            // 
            // jointTargetDescDataGridViewTextBoxColumn
            // 
            this.jointTargetDescDataGridViewTextBoxColumn.DataPropertyName = "JointTargetDesc";
            this.jointTargetDescDataGridViewTextBoxColumn.HeaderText = "Joint Target";
            this.jointTargetDescDataGridViewTextBoxColumn.Name = "jointTargetDescDataGridViewTextBoxColumn";
            this.jointTargetDescDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // jointResultDataGridViewTextBoxColumn
            // 
            this.jointResultDataGridViewTextBoxColumn.DataPropertyName = "JointResult";
            this.jointResultDataGridViewTextBoxColumn.HeaderText = "Result";
            this.jointResultDataGridViewTextBoxColumn.Name = "jointResultDataGridViewTextBoxColumn";
            this.jointResultDataGridViewTextBoxColumn.ReadOnly = true;
            this.jointResultDataGridViewTextBoxColumn.Visible = false;
            // 
            // jointResultDescDataGridViewTextBoxColumn
            // 
            this.jointResultDescDataGridViewTextBoxColumn.DataPropertyName = "JointResultDesc";
            this.jointResultDescDataGridViewTextBoxColumn.HeaderText = "Joint Result";
            this.jointResultDescDataGridViewTextBoxColumn.Name = "jointResultDescDataGridViewTextBoxColumn";
            this.jointResultDescDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // jointTestResultDataGridViewTextBoxColumn
            // 
            this.jointTestResultDataGridViewTextBoxColumn.DataPropertyName = "JointTestResult";
            this.jointTestResultDataGridViewTextBoxColumn.HeaderText = "[OK]";
            this.jointTestResultDataGridViewTextBoxColumn.Name = "jointTestResultDataGridViewTextBoxColumn";
            this.jointTestResultDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // tighteningResultIdDataGridViewTextBoxColumn
            // 
            this.tighteningResultIdDataGridViewTextBoxColumn.DataPropertyName = "TighteningResultId";
            this.tighteningResultIdDataGridViewTextBoxColumn.HeaderText = "TighteningResultId";
            this.tighteningResultIdDataGridViewTextBoxColumn.Name = "tighteningResultIdDataGridViewTextBoxColumn";
            this.tighteningResultIdDataGridViewTextBoxColumn.ReadOnly = true;
            this.tighteningResultIdDataGridViewTextBoxColumn.Visible = false;
            // 
            // tighteningResultDataGridViewTextBoxColumn
            // 
            this.tighteningResultDataGridViewTextBoxColumn.DataPropertyName = "TighteningResult";
            this.tighteningResultDataGridViewTextBoxColumn.HeaderText = "TighteningResult";
            this.tighteningResultDataGridViewTextBoxColumn.Name = "tighteningResultDataGridViewTextBoxColumn";
            this.tighteningResultDataGridViewTextBoxColumn.ReadOnly = true;
            this.tighteningResultDataGridViewTextBoxColumn.Visible = false;
            // 
            // idDataGridViewTextBoxColumn
            // 
            this.idDataGridViewTextBoxColumn.DataPropertyName = "Id";
            this.idDataGridViewTextBoxColumn.HeaderText = "Id";
            this.idDataGridViewTextBoxColumn.Name = "idDataGridViewTextBoxColumn";
            this.idDataGridViewTextBoxColumn.ReadOnly = true;
            this.idDataGridViewTextBoxColumn.Visible = false;
            // 
            // lastUpdateDateDataGridViewTextBoxColumn
            // 
            this.lastUpdateDateDataGridViewTextBoxColumn.DataPropertyName = "LastUpdateDate";
            this.lastUpdateDateDataGridViewTextBoxColumn.HeaderText = "LastUpdateDate";
            this.lastUpdateDateDataGridViewTextBoxColumn.Name = "lastUpdateDateDataGridViewTextBoxColumn";
            this.lastUpdateDateDataGridViewTextBoxColumn.ReadOnly = true;
            this.lastUpdateDateDataGridViewTextBoxColumn.Visible = false;
            // 
            // lastUpdatedByDataGridViewTextBoxColumn
            // 
            this.lastUpdatedByDataGridViewTextBoxColumn.DataPropertyName = "LastUpdatedBy";
            this.lastUpdatedByDataGridViewTextBoxColumn.HeaderText = "LastUpdatedBy";
            this.lastUpdatedByDataGridViewTextBoxColumn.Name = "lastUpdatedByDataGridViewTextBoxColumn";
            this.lastUpdatedByDataGridViewTextBoxColumn.ReadOnly = true;
            this.lastUpdatedByDataGridViewTextBoxColumn.Visible = false;
            // 
            // creationDateDataGridViewTextBoxColumn
            // 
            this.creationDateDataGridViewTextBoxColumn.DataPropertyName = "CreationDate";
            this.creationDateDataGridViewTextBoxColumn.HeaderText = "CreationDate";
            this.creationDateDataGridViewTextBoxColumn.Name = "creationDateDataGridViewTextBoxColumn";
            this.creationDateDataGridViewTextBoxColumn.ReadOnly = true;
            this.creationDateDataGridViewTextBoxColumn.Visible = false;
            // 
            // createdByDataGridViewTextBoxColumn
            // 
            this.createdByDataGridViewTextBoxColumn.DataPropertyName = "CreatedBy";
            this.createdByDataGridViewTextBoxColumn.HeaderText = "CreatedBy";
            this.createdByDataGridViewTextBoxColumn.Name = "createdByDataGridViewTextBoxColumn";
            this.createdByDataGridViewTextBoxColumn.ReadOnly = true;
            this.createdByDataGridViewTextBoxColumn.Visible = false;
            // 
            // TigtheningRepairsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1132, 171);
            this.Controls.Add(this.dgvList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TigtheningRepairsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TigtheningRepairsForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tighteningRepairModelBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvList;
        private System.Windows.Forms.BindingSource tighteningRepairModelBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn noDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn minDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn minDescDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn maxDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn maxDescDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn targetDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn targetDescDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultDescDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn testResultDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn jointMinDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn jointMinDescDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn jointMaxDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn jointMaxDescDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn jointTargetDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn jointTargetDescDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn jointResultDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn jointResultDescDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn jointTestResultDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn tighteningResultIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn tighteningResultDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastUpdateDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastUpdatedByDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn creationDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn createdByDataGridViewTextBoxColumn;
    }
}