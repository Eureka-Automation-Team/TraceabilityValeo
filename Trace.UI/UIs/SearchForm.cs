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
using Trace.UI.Presenters;
using Trace.UI.Views;

namespace Trace.UI.UIs
{
    public partial class SearchForm : Form, ISearchView
    {
        private readonly SearchPresenter _presenter;
        private int _pageNumber;

        public SearchForm()
        {
            InitializeComponent();

            _presenter = new SearchPresenter(this);
        }

        public int pageNumber
        {
            get { return _pageNumber; }
            set { _pageNumber = value; }
        }
        public BindingSource DataBinding 
        {
            get { return traceabilityLogModelBindingSource; }
            set { traceabilityLogModelBindingSource = value; } 
        }
        public string itemCode
        {
            get { return txtitemCode.Text; }
            set { txtitemCode.Text = value; }
        }
        public string partSerialNo
        {
            get { return txtpartSerialNo.Text; }
            set { txtpartSerialNo.Text = value; }
        }
        public DateTime startDate
        {
            get { return dtpstartDate.Value; }
            set { dtpstartDate.Value = value; }
        }
        public DateTime endDate
        {
            get { return dtpendDate.Value; }
            set { dtpendDate.Value = value; }
        }

        public event EventHandler Form_Load;
        public event EventHandler Search_Click;
        public event EventHandler Clear_Click;
        public event EventHandler Selected_Row;
        public event EventHandler PreviousPage;
        public event EventHandler NextPage;

        public void CloseMe()
        {
            this.Close();
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            CloseMe();
        }

        private void mnuClear_Click(object sender, EventArgs e)
        {
            if (Clear_Click != null)
                Clear_Click(sender, e);
        }

        private void mnuSearch_Click(object sender, EventArgs e)
        {
            if (Search_Click != null)
                Search_Click(sender, e);
        }

        private void mnuDetail_Click(object sender, EventArgs e)
        {
            if (Selected_Row != null)
                Selected_Row(sender, e);
        }

        private void dgvList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Selected_Row != null)
                Selected_Row(sender, e);
        }

        private void mnuExportCSV_Click(object sender, EventArgs e)
        {
            if (dgvList.Rows.Count > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = "Output.csv";
                bool fileError = false;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(sfd.FileName))
                    {
                        try
                        {
                            File.Delete(sfd.FileName);
                        }
                        catch (IOException ex)
                        {
                            fileError = true;
                            MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                        }
                    }
                    if (!fileError)
                    {
                        try
                        {
                            int columnCount = 10;// dgvList.Columns.Count;
                            string columnNames = "";
                            string[] outputCsv = new string[dgvList.Rows.Count + 1];
                            for (int i = 0; i < columnCount; i++)
                            {
                                columnNames += dgvList.Columns[i].HeaderText.ToString() + ",";
                            }
                            outputCsv[0] += columnNames;

                            for (int i = 1; (i - 1) < dgvList.Rows.Count; i++)
                            {
                                for (int j = 0; j < columnCount; j++)
                                {
                                    outputCsv[i] += dgvList.Rows[i - 1].Cells[j].Value?.ToString() + ",";
                                }
                            }

                            File.WriteAllLines(sfd.FileName, outputCsv, Encoding.UTF8);
                            MessageBox.Show("Data Exported Successfully !!!", "Info");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error :" + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No Record To Export !!!", "Info");
            }
        }
    }
}
