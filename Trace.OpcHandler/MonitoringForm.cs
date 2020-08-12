using Opc.Da;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trace.Domain.Models;
using Trace.OpcHandlerMachine01;
using Trace.OpcHandlerMachine01.Presenters;

namespace Trace.OpcHandler
{
    public partial class MonitoringForm : Form, IMonitoringView
    {
        private readonly MainPresenter _presenter;
        private Server _daServer = null;
        private Subscription _groupRead;
        private SubscriptionState _groupStateRead;
        private Subscription _groupWrite;
        private SubscriptionState _groupStateWrite;

        private List<PlcTagModel> _plcTags;
        private bool _connectedPlc;
        private bool _systemReady;
        private string _tagMainBlock;
        private string _tagClockReady;
        private string _tagTraceabilityReady;
        private MachineModel _machine;
        private Item[] _items;

        public MonitoringForm()
        {
            InitializeComponent();
            this._presenter = new MainPresenter(this);
        }

        public Server daServer
        {
            get { return _daServer; }
            set { _daServer = value; }
        }
        public Subscription groupRead
        {
            get { return _groupRead; }
            set { _groupRead = value; }
        }
        public SubscriptionState groupStateRead
        {
            get { return _groupStateRead; }
            set { _groupStateRead = value; }
        }
        public Subscription groupWrite
        {
            get { return _groupWrite; }
            set { _groupWrite = value; }
        }
        public SubscriptionState groupStateWrite
        {
            get { return _groupStateWrite; }
            set { _groupStateWrite = value; }
        }
        public List<PlcTagModel> plcTags
        {
            get { return _plcTags; }
            set { _plcTags = value; }
        }
        public MachineModel machine
        {
            get { return _machine; }
            set
            {
                _machine = value;

                if (_machine != null)
                {
                    butStatusMc.Text = _machine.StatusName;
                    SetButtonMachineStatusColor(butStatusMc, _machine.OnlineFlag);

                    butRequestLogging.Text = _machine.RequestLogging.ToString().ToUpper();
                    SetButtonStatusColor(butRequestLogging, Convert.ToInt32(_machine.RequestLogging));

                    this.Tag = _machine;
                    this.Text = "Station 1 : " + _machine.ManchineName;
                    butCompletedLogging.Text = _machine.CompletedLoggingDesc;
                    SetButtonStatusColor(butCompletedLogging, _machine.CompletedLogging);
                    if (_machine.RequestLogging && _machine.CompletedLogging == 0)
                        KeepLogging(_machine, null);

                    if (_machine.RequestVerifyCode)
                    {
                        butRequestVerifyCode1.Text = _machine.CodeVerifyResultDesc;
                    }
                    else
                    {
                        butRequestVerifyCode1.Text = string.Empty;
                    }
                    SetButtonStatusColor(butRequestVerifyCode1, Convert.ToInt32(_machine.RequestVerifyCode));
                    //if (_machine.RequestVerifyCode && _machine.CodeVerifyResult == 0)
                    //    VerityCode(_machine, null);
                }
            }
        }


        public bool connectedPlc
        {
            get { return _connectedPlc; }
            set
            {
                _connectedPlc = value;
                txtServerUrl.ReadOnly = _connectedPlc;
                butMakeReady.Visible = _connectedPlc;
                if (_connectedPlc)
                {
                    butConnect.Text = "Disconnect";
                    systemReady = true;
                    EnableClock();
                }
                else
                {
                    butConnect.Text = "Connect";
                    systemReady = false;
                    DisableClock();
                }
            }
        }
        public bool systemReady
        {
            get { return _systemReady; }
            set
            {
                _systemReady = value;

                if (_systemReady)
                {
                    butMakeReady.Text = "Ready";
                    butMakeReady.BackColor = Color.GreenYellow;
                    butRefresh.Visible = true;
                    EnableClock();
                }
                else
                {
                    butMakeReady.Text = "Not ready";
                    butMakeReady.BackColor = Color.Gray;
                    butRefresh.Visible = false;
                    DisableClock();
                }
            }
        }
        public string tagMainBlock
        {
            get { return _tagMainBlock; }
            set { _tagMainBlock = value; }
        }
        public string serverUrl
        {
            get { return txtServerUrl.Text; }
            set { txtServerUrl.Text = value; }
        }
        public string tagClockReady
        {
            get { return _tagClockReady; }
            set { _tagClockReady = value; }
        }
        public string tagTraceabilityReady
        {
            get { return _tagTraceabilityReady; }
            set { _tagTraceabilityReady = value; }
        }
        public string ResultnMessage
        {
            get { return txtMessageResult.Text; }
            set { txtMessageResult.Text = value; }
        }
        public Item[] items
        {
            get { return _items; }
            set { _items = value; }
        }

        public event EventHandler FormLoad;
        public event EventHandler Connect_Click;
        public event EventHandler Disconnect_Click;
        public event EventHandler InterLock;
        public event EventHandler MakeReady;
        public event EventHandler KeepLogging;
        public event EventHandler CompleteAction;
        public event EventHandler VerityCode;
        public event EventHandler VerityActuater;
        public event EventHandler RefreshData;

        public void DisableClock()
        {
            timerConnect.Enabled = true;
            timerInter.Enabled = false;            
        }

        public void EnableClock()
        {
            timerInter.Interval = 2000;
            timerInter.Enabled = true;
            timerConnect.Enabled = false;
        }

        public void group_DataChanged(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].ItemName == this._tagTraceabilityReady)
                {
                    int receivedData = (Int16)values[i].Value;
                    butMakeReady.Invoke(new EventHandler(delegate { this.systemReady = Convert.ToBoolean(receivedData); }));
                }

                //Machine 1 Status
                if (values[i].ItemName == tagMainBlock + "ST1StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc.Invoke(new EventHandler(
                        delegate
                        {
                            var mac = this.machine;
                            mac.OnlineFlag = receivedData;
                            this.machine = mac;
                        }));
                }

                if (values[i].ItemName == tagMainBlock + "ST1ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            var mac = this.machine;
                            mac.RequestLogging = val;
                            this.machine = mac;

                            if (val)
                                KeepLogging(this.machine, null);
                        }));
                }
                if (values[i].ItemName == tagMainBlock + "ST1LoggingApp")
                {
                    int receivedData = (Int16)values[i].Value;
                    butCompletedLogging.Invoke(new EventHandler(
                        delegate
                        {
                            var mac = this.machine;
                            mac.CompletedLogging = receivedData;
                            this.machine = mac;
                        }));
                }
                if (values[i].ItemName == tagMainBlock + "ST1ReqChkCodeVerify")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestVerifyCode1.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            var mac = this.machine;
                            mac.RequestVerifyCode = val;
                            this.machine = mac;
                            if (val)
                                VerityCode(this.machine, null);
                        }));
                }
            }
        }

        private void MonitoringForm_Load(object sender, EventArgs e)
        {
            if (FormLoad != null)
                FormLoad(sender, e);

            timerConnect.Enabled = true;
            _connectedPlc = false;
            DisableClock();
        }

        private void butConnect_Click(object sender, EventArgs e)
        {
            if (this.connectedPlc)
            {
                if (Disconnect_Click != null)
                    Disconnect_Click(sender, e);
            }
            else
            {
                if (Connect_Click != null)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    Connect_Click(sender, e);
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void timerConnect_Tick(object sender, EventArgs e)
        {
            if (Connect_Click != null)
                Connect_Click(sender, e);
        }

        private void timerInter_Tick(object sender, EventArgs e)
        {
            if (InterLock != null)
                InterLock(sender, e);
        }

        private void SetButtonMachineStatusColor(Button butStatus, int mcStatus)
        {
            if (mcStatus == 0)
            {
                butStatus.BackColor = Color.Gray;
            }
            else if (mcStatus == 1)
            {
                butStatus.BackColor = Color.GreenYellow;
            }
            else
            {
                butStatus.BackColor = Color.Orange;
            }
        }

        private void SetButtonStatusColor(Button butStatus, int v)
        {
            if (v == 0)
            {
                butStatus.BackColor = Color.Gray;
            }
            else if (v == 1)
            {
                butStatus.BackColor = Color.GreenYellow;
            }
            else if (v == 2)
            {
                butStatus.BackColor = Color.OrangeRed;
            }
            else if (v == 3)
            {
                butStatus.BackColor = Color.Red;
            }
        }

        private void butMakeReady_Click(object sender, EventArgs e)
        {
            if (MakeReady != null)
                MakeReady(sender, e);

            if (systemReady)
            {
                Cursor.Current = Cursors.WaitCursor;
                group_DataChanged(null, null, groupRead.Read(groupRead.Items));
                Cursor.Current = Cursors.Default;
            }
        }

        private void butRefresh_Click(object sender, EventArgs e)
        {
            if (RefreshData != null)
                RefreshData(sender, e);
        }

        private void butRequestLogging_Click(object sender, EventArgs e)
        {
            this.machine.RequestLogging = true;
            KeepLogging(this.machine, null);
        }

        private void butRequestVerifyCode1_Click(object sender, EventArgs e)
        {
            this.machine.RequestVerifyCode = true;
            VerityCode(this.machine, null);
        }
    }
}
