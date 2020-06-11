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
    public partial class uCtrlStation3 : UserControl, IStation3View
    {
        private readonly CtrlStation3Presenter _presenter;
        private bool _MonitorFlag;
        private TraceabilityLogModel _traceabilityUpperLog;
        private TraceabilityLogModel _traceabilityLowerLog;

        public uCtrlStation3()
        {
            InitializeComponent();

            _presenter = new CtrlStation3Presenter(this);
        }

        public bool MonitorFlag
        {
            get { return _MonitorFlag; }
            set { _MonitorFlag = value; }
        }
        public TraceabilityLogModel traceabilityUpperLog
        {
            get { return _traceabilityUpperLog; }
            set
            {
                _traceabilityUpperLog = value;

                if (_traceabilityUpperLog != null)
                {
                    txtStationNumber.Text = _traceabilityUpperLog.Station.StationNumber;
                    txtUpperDescription.Text = _traceabilityUpperLog.Description;
                    txtUpperManchineName.Text = _traceabilityUpperLog.Machine.ManchineName;
                    txtUpperItemCode.Text = _traceabilityUpperLog.ItemCode;


                    if (_traceabilityUpperLog.FinalResult)
                    {
                        lblUpperFinalResult.Text = "OK";
                        lblUpperFinalResult.BackColor = Color.LawnGreen;
                    }
                    else
                    {
                        lblUpperFinalResult.Text = "NOK";
                        lblUpperFinalResult.BackColor = Color.Red;
                    }
                }
                else
                {
                    txtStationNumber.Text = string.Empty;
                    txtUpperDescription.Text = string.Empty;
                    txtUpperManchineName.Text = string.Empty;
                    txtUpperItemCode.Text = string.Empty;

                    lblUpperFinalResult.Text = "";
                    lblUpperFinalResult.BackColor = Color.White;
                }
            }
        }
        public TraceabilityLogModel traceabilityLowerLog
        {
            get { return _traceabilityLowerLog; }
            set
            {
                _traceabilityLowerLog = value;

                if (_traceabilityLowerLog != null)
                {
                    txtStationNumber.Text = _traceabilityLowerLog.Station.StationNumber;
                    txtLowerDescription.Text = _traceabilityLowerLog.Description;
                    txtLowerManchineName.Text = _traceabilityLowerLog.Machine.ManchineName;
                    txtLowerItemCode.Text = _traceabilityLowerLog.ItemCode;


                    if (_traceabilityLowerLog.FinalResult)
                    {
                        lblLowerFinalResult.Text = "OK";
                        lblLowerFinalResult.BackColor = Color.LawnGreen;
                    }
                    else
                    {
                        lblLowerFinalResult.Text = "NOK";
                        lblLowerFinalResult.BackColor = Color.Red;
                    }
                }
                else
                {
                    txtStationNumber.Text = string.Empty;
                    txtLowerDescription.Text = string.Empty;
                    txtLowerManchineName.Text = string.Empty;
                    txtLowerItemCode.Text = string.Empty;

                    lblLowerFinalResult.Text = "";
                    lblLowerFinalResult.BackColor = Color.White;
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

        private void uCtrlStation3_Load(object sender, EventArgs e)
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
