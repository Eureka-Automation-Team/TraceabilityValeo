using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trace.UI.Views;
using Trace.Domain.Models;
using Trace.UI.Presenters;

namespace Trace.UI.Controls
{
    public partial class uCtrlStation4 : UserControl, IStation4View
    {
        private readonly CtrlStation4Presenter _presenter;
        private bool _MonitorFlag;
        private TraceabilityLogModel _traceabilityLog;

        public uCtrlStation4()
        {
            InitializeComponent();

            _presenter = new CtrlStation4Presenter(this);
        }

        public bool MonitorFlag
        {
            get { return _MonitorFlag; }
            set { _MonitorFlag = value; }
        }
        public TraceabilityLogModel traceabilityLog
        {
            get { return _traceabilityLog; }
            set
            {
                _traceabilityLog = value;

                if (_traceabilityLog != null)
                {
                    if (txtItemCode.Text.Trim() != _traceabilityLog.ItemCode.Trim())
                        partAssemblyModelBindingSource.DataSource = _traceabilityLog.PartAssemblies;

                    txtStationNumber.Text = _traceabilityLog.Station.StationNumber;
                    txtManchineName.Text = _traceabilityLog.Machine.ManchineName;
                    txtItemCode.Text = _traceabilityLog.ItemCode;                   
                    tighteningResultModelBindingSource.DataSource = _traceabilityLog.TighteningResults;
                    lblFinalResult.Text = _traceabilityLog.FinalResultDesc;
                    txtModelRunningFlag.Text = "LOWER " + _traceabilityLog.ModelRunningDesc.Replace("_", " ");

                    if (_traceabilityLog.FinalResult == 1)
                    {

                        lblFinalResult.BackColor = Color.LawnGreen;
                    }
                    else if (_traceabilityLog.FinalResult == 2)
                    {
                        lblFinalResult.BackColor = Color.Red;
                    }
                    else
                    {
                        lblFinalResult.BackColor = Color.Yellow;
                    }
                }
                else
                {
                    txtStationNumber.Text = string.Empty;
                    txtManchineName.Text = string.Empty;
                    txtItemCode.Text = string.Empty;
                    partAssemblyModelBindingSource.DataSource = null;
                    tighteningResultModelBindingSource.DataSource = null;
                    txtModelRunningFlag.Text = string.Empty;

                    lblFinalResult.Text = "";
                    lblFinalResult.BackColor = Color.White;
                }
            }
        }

        public event EventHandler ControlLoad;
        public event EventHandler MonitoringRailTime;
        public event EventHandler ShowTighteningRepairs;

        public void EnableTimer(bool enable)
        {
            timer1.Interval = 1000;
            timer1.Enabled = enable;
        }

        private void uCtrlStation4_Load(object sender, EventArgs e)
        {
            if (ControlLoad != null)
                ControlLoad(sender, e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (MonitoringRailTime != null)
                MonitoringRailTime(sender, e);

            SetGrid();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (ShowTighteningRepairs != null)
                ShowTighteningRepairs(sender, e);
        }

        public void SetGrid()
        {
            if (this.traceabilityLog != null)
            {
                if (this.traceabilityLog.ModelRunningFlag == 1)
                {
                    dataGridView1.Columns[13].Visible = true;
                }
                else if (this.traceabilityLog.ModelRunningFlag == 2)
                {
                    dataGridView1.Columns[13].Visible = false;
                }
            }
        }
    }
}
