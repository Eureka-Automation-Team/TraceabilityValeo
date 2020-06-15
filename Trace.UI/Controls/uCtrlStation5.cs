﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trace.UI.Views;
using Trace.UI.Presenters;
using Trace.Domain.Models;

namespace Trace.UI.Controls
{
    public partial class uCtrlStation5 : UserControl, IStation5View
    {
        private readonly CtrlStation5Presenter _presenter;
        private bool _MonitorFlag;
        private TraceabilityLogModel _traceabilityUpperLog;
        private TraceabilityLogModel _traceabilityLowerLog;

        public uCtrlStation5()
        {
            InitializeComponent();

            _presenter = new CtrlStation5Presenter(this);
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
                    txtUpperManchineName.Text = _traceabilityUpperLog.Machine.ManchineName;
                    txtUpperItemCode.Text = _traceabilityUpperLog.ItemCode;
                    txtUpperPartSerialNumber.Text = _traceabilityUpperLog.PartSerialNumber;
                    txtUpperActuator.Text = _traceabilityUpperLog.Actuator;
                    txtUpperProductionDate.Text = _traceabilityUpperLog.ProductionDate.ToLongDateString();
                    txtUpperSwNumber.Text = _traceabilityUpperLog.SwNumber;
                    txtUpperCurrentMaximum.Text = _traceabilityUpperLog.CurrentMaximum;
                    txtUpperOpenAngle.Text = _traceabilityUpperLog.OpenAngle;
                    txtUpperLineErrorCounter.Text = _traceabilityUpperLog.LineErrorCounter;

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
                    txtUpperManchineName.Text = string.Empty;
                    txtUpperItemCode.Text = string.Empty;
                    txtUpperPartSerialNumber.Text = string.Empty;
                    txtUpperActuator.Text = string.Empty;
                    txtUpperProductionDate.Text = string.Empty;
                    txtUpperSwNumber.Text = string.Empty;
                    txtUpperCurrentMaximum.Text = string.Empty;
                    txtUpperOpenAngle.Text = string.Empty;
                    txtUpperLineErrorCounter.Text = string.Empty;


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
                    txtLowerManchineName.Text = _traceabilityLowerLog.Machine.ManchineName;
                    txtLowerItemCode.Text = _traceabilityLowerLog.ItemCode;
                    txtLowerPartSerialNumber.Text = _traceabilityLowerLog.PartSerialNumber;
                    txtLowerActuator.Text = _traceabilityLowerLog.Actuator;
                    txtLowerProductionDate.Text = _traceabilityLowerLog.ProductionDate.ToLongDateString();
                    txtLowerSwNumber.Text = _traceabilityLowerLog.SwNumber;
                    txtLowerCurrentMaximum.Text = _traceabilityLowerLog.CurrentMaximum;
                    txtLowerOpenAngle.Text = _traceabilityLowerLog.OpenAngle;
                    txtLowerLineErrorCounter.Text = _traceabilityLowerLog.LineErrorCounter;

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
                    txtLowerManchineName.Text = string.Empty;
                    txtLowerItemCode.Text = string.Empty;
                    txtLowerPartSerialNumber.Text = string.Empty;
                    txtLowerActuator.Text = string.Empty;
                    txtLowerProductionDate.Text = string.Empty;
                    txtLowerSwNumber.Text = string.Empty;
                    txtLowerCurrentMaximum.Text = string.Empty;
                    txtLowerOpenAngle.Text = string.Empty;
                    txtLowerLineErrorCounter.Text = string.Empty;


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
            panelQRCode.Visible = !enable;
        }

        private void uCtrlStation5_Load(object sender, EventArgs e)
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