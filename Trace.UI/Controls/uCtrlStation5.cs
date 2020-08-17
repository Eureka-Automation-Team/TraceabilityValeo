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
                    txtUpperProductionDate.Text = _traceabilityUpperLog.ProductionDate.ToString("dd/MM/yyyy HH:mm");
                    txtUpperSwNumber.Text = _traceabilityUpperLog.SwNumber;
                    //txtUpperCurrentMaximum.Text = _traceabilityUpperLog.CurrentMaximum;
                    //txtUpperOpenAngle.Text = _traceabilityUpperLog.OpenAngle;
                    txtUpperLineErrorCounter.Text = _traceabilityUpperLog.LineErrorCounter;
                    lblUpperFinalResult.Text = _traceabilityUpperLog.FinalResultDesc;
                    txtUpperCameraCheck.Text = _traceabilityUpperLog.Attribute1;
                    //txtUpperVaneCheck.Text = _traceabilityUpperLog.Attribute2;

                    tighteningResultBindingUpper.DataSource = _traceabilityUpperLog.TighteningResults;
                    cameraResultBindingUpper.DataSource = _traceabilityUpperLog.CameraResults;

                    if (_traceabilityUpperLog.FinalResult == 1)
                    {

                        lblUpperFinalResult.BackColor = Color.LawnGreen;
                    }
                    else if (_traceabilityUpperLog.FinalResult == 2)
                    {
                        lblUpperFinalResult.BackColor = Color.Red;
                    }
                    else
                    {
                        lblUpperFinalResult.BackColor = Color.Yellow;
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
                    //txtUpperCurrentMaximum.Text = string.Empty;
                    //txtUpperOpenAngle.Text = string.Empty;
                    txtUpperLineErrorCounter.Text = string.Empty;
                    txtUpperCameraCheck.Text = string.Empty;
                    //txtUpperVaneCheck.Text = string.Empty;

                    tighteningResultBindingUpper.DataSource = null;
                    cameraResultBindingUpper.DataSource = null;

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
                    txtLowerProductionDate.Text = _traceabilityLowerLog.ProductionDate.ToString("dd/MM/yyyy HH:mm");
                    txtLowerSwNumber.Text = _traceabilityLowerLog.SwNumber;
                    //txtLowerCurrentMaximum.Text = _traceabilityLowerLog.CurrentMaximum;
                    //txtLowerOpenAngle.Text = _traceabilityLowerLog.OpenAngle;
                    txtLowerLineErrorCounter.Text = _traceabilityLowerLog.LineErrorCounter;
                    lblLowerFinalResult.Text = _traceabilityLowerLog.FinalResultDesc;
                    txtLowerCameraCheck.Text = _traceabilityLowerLog.Attribute1;
                    //txtLowerVaneCheck.Text = _traceabilityLowerLog.Attribute2;

                    tighteningResultBindingLower.DataSource = _traceabilityLowerLog.TighteningResults;
                    cameraResultBindingLower.DataSource = _traceabilityLowerLog.CameraResults;

                    if (_traceabilityLowerLog.FinalResult == 1)
                    {

                        lblLowerFinalResult.BackColor = Color.LawnGreen;
                    }
                    else if (_traceabilityLowerLog.FinalResult == 2)
                    {
                        lblLowerFinalResult.BackColor = Color.Red;
                    }
                    else
                    {
                        lblLowerFinalResult.BackColor = Color.Yellow;
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
                    //txtLowerCurrentMaximum.Text = string.Empty;
                    //txtLowerOpenAngle.Text = string.Empty;
                    txtLowerLineErrorCounter.Text = string.Empty;
                    txtLowerCameraCheck.Text = string.Empty;
                    //txtLowerVaneCheck.Text = string.Empty;

                    tighteningResultBindingLower.DataSource = null;
                    cameraResultBindingLower.DataSource = null;

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
            picQRCodeUpper.Visible = !enable;
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

            SetGrid();
        }

        public void GenerateQrCode()
        {
            QRCoder.QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
            string partSerialNo = string.Empty;

            if (traceabilityUpperLog != null)
                partSerialNo = traceabilityUpperLog.PartSerialNumber;

            if (traceabilityLowerLog != null)
                partSerialNo = traceabilityLowerLog.PartSerialNumber;

            if (!string.IsNullOrEmpty(partSerialNo))
            {
                var qrData = qrGenerator.CreateQrCode(partSerialNo, QRCoder.QRCodeGenerator.ECCLevel.H);
                var qrCode = new QRCoder.QRCode(qrData);
                var image = qrCode.GetGraphic(150);
                if (traceabilityUpperLog != null)
                    picQRCodeUpper.Image = image;

                if (traceabilityLowerLog != null)
                    picQRCodeLower.Image = image;
            }
        }

        public void SetGrid()
        {
            if (this.traceabilityUpperLog != null)
            {
                if (this.traceabilityUpperLog.ModelRunningFlag == 1)
                {
                    dgvEOLUpper.Columns[22].Visible = true;
                    dataGridView3.Columns[1].Visible = true;
                }
                else if (this.traceabilityUpperLog.ModelRunningFlag == 2)
                {
                    dgvEOLUpper.Columns[22].Visible = false;
                    dataGridView3.Columns[1].Visible = false;
                }
            }

            if (this.traceabilityLowerLog != null)
            { 
                if (this.traceabilityLowerLog.ModelRunningFlag == 1)
                {
                    dgvEOLLower.Columns[22].Visible = true;
                    dataGridView1.Columns[1].Visible = true;
                }
                else if (this.traceabilityLowerLog.ModelRunningFlag == 2)
                {
                    dgvEOLLower.Columns[22].Visible = false;
                    dataGridView1.Columns[1].Visible = false;
                }
            }
        }
    }
}
