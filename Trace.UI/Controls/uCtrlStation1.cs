using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trace.Domain.Models;
using Trace.UI.Views;
using Trace.UI.Presenters;

namespace Trace.UI.Controls
{
    public partial class uCtrlStation1 : UserControl, IStation1View
    {
        private readonly CtrlStation1Presenter _presenter;
        private bool _MonitorFlag;
        private TraceabilityLogModel _traceabilityLog;
        public uCtrlStation1()
        {
            InitializeComponent();

            this._presenter = new CtrlStation1Presenter(this);
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

                if(_traceabilityLog != null)
                {
                    txtStationNumber.Text = _traceabilityLog.Station.StationNumber;
                    txtManchineName.Text = _traceabilityLog.Machine.ManchineName;
                    txtItemCode.Text = _traceabilityLog.ItemCode;
                    partAssemblyModelBindingSource.DataSource = _traceabilityLog.PartAssemblies;
                    tighteningResultModelBindingSource.DataSource = _traceabilityLog.TighteningResults;
                    cameraResultModelBindingSource.DataSource = _traceabilityLog.CameraResults;
                    lblFinalResult.Text = _traceabilityLog.FinalResultDesc;

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
                    cameraResultModelBindingSource.DataSource = null;

                    lblFinalResult.Text = "";
                    lblFinalResult.BackColor = Color.White;
                }
            }
        }

        public event EventHandler ControlLoad;
        public event EventHandler MonitoringRailTime;

        public void EnableTimer(bool enable)
        {
            timer1.Interval = 1000;
            timer1.Enabled = enable;
        }

        private void uCtrlStation1_Load(object sender, EventArgs e)
        {
            if (ControlLoad != null)
                ControlLoad(sender, e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (MonitoringRailTime != null)
                MonitoringRailTime(sender, e);
        }
    }
}
