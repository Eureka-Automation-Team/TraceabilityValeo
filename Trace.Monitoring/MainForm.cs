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
using Trace.Monitoring.Presenters;

namespace Trace.Monitoring
{
    public partial class MainForm : Form, IMainView
    {
        private readonly MainPresenter _presenter;
        private Server _daServer = null;
        private Subscription _groupRead;        
        private SubscriptionState _groupStateRead;

        private Subscription _groupWrite;
        private SubscriptionState _groupStateWrite;

        private Item[] _items;

        //initialization of the sample object that contains opc values
        OPCObject _myOpcObject = new OPCObject();

        private MachineModel _machine1;
        private MachineModel _machine2;
        private MachineModel _machine3;
        private MachineModel _machine4;
        private MachineModel _machine5;
        private MachineModel _machine6;
        private MachineModel _machine7;

        private List<PlcTagModel> _plcTags;
        private bool _connectedPlc;
        private bool _systemReady;
        private string _tagMainBlock;
        private string _tagClockReady;
        private string _tagTraceabilityReady;

        public string serverUrl 
        {
            get { return txtServerUrl.Text; }
            set { txtServerUrl.Text = value; }
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
        public OPCObject myOpcObject
        {
            get { return _myOpcObject; }
            set { _myOpcObject = value; }
        }
        public Item[] items
        {
            get { return _items; }
            set { _items = value; }
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

        public string tagClockReady
        {
            get { return _tagClockReady; }
            set { _tagClockReady = value; }
        }
        public string tagTraceabilityReady
        {
            get { return _tagTraceabilityReady; }
            set 
            { 
                _tagTraceabilityReady = value; 
            }
        }

        public string tagMainBlock
        {
            get { return _tagMainBlock; }
            set { _tagMainBlock = value; }
        }

        public List<PlcTagModel> plcTags
        {
            get { return _plcTags; }
            set { _plcTags = value; }
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
                    EnableClock();
                }
                else
                {
                    butMakeReady.Text = "Not ready";
                    butMakeReady.BackColor = Color.Gray;
                    DisableClock();
                }
            }
        }

        public MachineModel machine1
        {
            get { return _machine1; }
            set
            {
                _machine1 = value;

                if (_machine1 != null)
                {
                    txtManchineName1.Tag = _machine1;
                    txtManchineName1.Text = _machine1.ManchineName;
                    txtMessageResult1.Text = _machine1.MessageResult;
                    butCompletedLogging1.Text = _machine1.CompletedLoggingDesc;
                    SetButtonStatusColor(butCompletedLogging1, _machine1.CompletedLogging);

                    if (_machine1.RequestVerifyCode)
                    {
                        butRequestVerifyCode1.Text = _machine1.CodeVerifyResultDesc;
                    }
                    else
                    {
                        butRequestVerifyCode1.Text = string.Empty;
                    }
                }
            }
        }
        public MachineModel machine2
        {
            get { return _machine2; }
            set
            {
                _machine2 = value;

                if (_machine2 != null)
                {
                    txtManchineName2.Tag = _machine2;
                    txtManchineName2.Text = _machine2.ManchineName;
                    txtMessageResult2.Text = _machine2.MessageResult;
                    butCompletedLogging2.Text = _machine2.CompletedLoggingDesc;
                    SetButtonStatusColor(butCompletedLogging2, _machine2.CompletedLogging);

                    if (_machine2.RequestVerifyCode)
                    {
                        butRequestVerifyCode2.Text = _machine2.CodeVerifyResultDesc;
                    }
                    else
                    {
                        butRequestVerifyCode2.Text = string.Empty;
                    }
                }
            }
        }
        public MachineModel machine3
        {
            get { return _machine3; }
            set
            {
                _machine3 = value;

                if (_machine3 != null)
                {
                    txtManchineName3.Tag = _machine3;
                    txtManchineName3.Text = _machine3.ManchineName;
                    txtMessageResult3.Text = _machine3.MessageResult;
                    butCompletedLogging3.Text = _machine3.CompletedLoggingDesc;
                    SetButtonStatusColor(butCompletedLogging3, _machine3.CompletedLogging);

                    if (_machine3.RequestVerifyCode)
                    {
                        butRequestVerifyCode3.Text = _machine3.CodeVerifyResultDesc;
                    }
                    else
                    {
                        butRequestVerifyCode3.Text = string.Empty;
                    }
                }
            }
        }
        public MachineModel machine4
        {
            get { return _machine4; }
            set
            {
                _machine4 = value;

                if (_machine4 != null)
                {
                    txtManchineName4.Tag = _machine4;
                    txtManchineName4.Text = _machine4.ManchineName;
                    txtMessageResult4.Text = _machine4.MessageResult;
                    butCompletedLogging4.Text = _machine4.CompletedLoggingDesc;
                    SetButtonStatusColor(butCompletedLogging4, _machine4.CompletedLogging);

                    if (_machine4.RequestVerifyCode)
                    {
                        butRequestVerifyCode4.Text = _machine4.CodeVerifyResultDesc;
                    }
                    else
                    {
                        butRequestVerifyCode4.Text = string.Empty;
                    }
                }
            }
        }
        public MachineModel machine5
        {
            get { return _machine5; }
            set
            {
                _machine5 = value;

                if (_machine5 != null)
                {
                    txtManchineName5.Tag = _machine5;
                    txtManchineName5.Text = _machine5.ManchineName;
                    txtMessageResult5.Text = _machine5.MessageResult;
                    butCompletedLogging5.Text = _machine5.CompletedLoggingDesc;
                    SetButtonStatusColor(butCompletedLogging5, _machine5.CompletedLogging);

                    if (_machine5.RequestVerifyCode)
                    {
                        butRequestVerifyCode5.Text = _machine5.CodeVerifyResultDesc;
                    }
                    else
                    {
                        butRequestVerifyCode5.Text = string.Empty;
                    }
                }
            }
        }
        public MachineModel machine6
        {
            get { return _machine6; }
            set
            {
                _machine6 = value;

                if (_machine6 != null)
                {
                    txtManchineName6.Tag = _machine6;
                    txtManchineName6.Text = _machine6.ManchineName;
                    txtMessageResult6.Text = _machine6.MessageResult;
                    butCompletedLogging6.Text = _machine6.CompletedLoggingDesc;
                    SetButtonStatusColor(butCompletedLogging6, _machine6.CompletedLogging);

                    if (_machine5.RequestVerifyCode)
                    {
                        butRequestVerifyCode6.Text = _machine6.CodeVerifyResultDesc;
                    }
                    else
                    {
                        butRequestVerifyCode6.Text = string.Empty;
                    }
                }
            }
        }
        public MachineModel machine7
        {
            get { return _machine7; }
            set
            {
                _machine7 = value;

                if (_machine7 != null)
                {
                    txtManchineName7.Tag = _machine7;
                    txtManchineName7.Text = _machine7.ManchineName;
                    txtMessageResult7.Text = _machine7.MessageResult;
                    butCompletedLogging7.Text = _machine7.CompletedLoggingDesc;
                    SetButtonStatusColor(butCompletedLogging7, _machine7.CompletedLogging);

                    if (_machine5.RequestVerifyCode)
                    {
                        butRequestVerifyCode7.Text = _machine7.CodeVerifyResultDesc;
                    }
                    else
                    {
                        butRequestVerifyCode7.Text = string.Empty;
                    }
                }
            }
        }

        public event EventHandler FormLoad;
        public event EventHandler Connect_Click;
        public event EventHandler Disconnect_Click;
        public event EventHandler InterLock;
        public event EventHandler MakeReady;
        public event EventHandler KeepLogging;
        public event EventHandler CompleteAction;
        public event EventHandler VerityCode;

        public MainForm()
        {
            InitializeComponent();

            this._presenter = new MainPresenter(this);
        }


        public void group_DataChanged(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                
                //Machine 1
                if (values[i].ItemName == this._tagTraceabilityReady)
                {
                    int receivedData = (Int16)values[i].Value;
                    butMakeReady.Invoke(new EventHandler(delegate { this.systemReady = Convert.ToBoolean(receivedData); }));
                }
                if (values[i].ItemName == tagMainBlock + "ST1StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc1.Invoke(new EventHandler(
                        delegate 
                        { 
                            this.machine1.OnlineFlag = receivedData;
                            butStatusMc1.Text = this.machine1.StatusName;
                            SetButtonMachineStatusColor(butStatusMc1, receivedData);
                        }));                    
                }
                if (values[i].ItemName == tagMainBlock + "ST1ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging1.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            butRequestLogging1.Text = val.ToString().ToUpper();
                            SetButtonStatusColor(butRequestLogging1, receivedData);
                            if (val)
                                KeepLogging((MachineModel)txtManchineName1.Tag, null);
                        }));
                }
                if (values[i].ItemName == tagMainBlock + "ST1LoggingApp")
                {
                    int receivedData = (Int16)values[i].Value;
                    butCompletedLogging1.Invoke(new EventHandler(
                        delegate 
                        {
                            this.machine1.CompletedLogging = receivedData;
                            butCompletedLogging1.Text = this.machine1.CompletedLoggingDesc;
                            SetButtonStatusColor(butCompletedLogging1, receivedData);
                        }));                    
                }
                if (values[i].ItemName == tagMainBlock + "ST1ReqChkCodeVerify")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestVerifyCode1.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            this.machine1.RequestVerifyCode = val;
                            SetButtonStatusColor(butRequestVerifyCode1, receivedData);
                            if (val)
                                VerityCode(this.machine1, null);
                        }));
                }

                //Machine 2
                if (values[i].ItemName == tagMainBlock + "ST2StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc2.Invoke(new EventHandler(
                        delegate 
                        { 
                            this.machine2.OnlineFlag = receivedData;
                            butStatusMc2.Text = this.machine2.StatusName;
                            SetButtonMachineStatusColor(butStatusMc2, receivedData);
                        }));                    
                }
                if (values[i].ItemName == tagMainBlock + "ST2ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging2.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            butRequestLogging2.Text = val.ToString().ToUpper();
                            SetButtonStatusColor(butRequestLogging2, receivedData);
                            if (val)
                                KeepLogging((MachineModel)txtManchineName2.Tag, null);
                        }));
                }
                if (values[i].ItemName == tagMainBlock + "ST2LoggingApp")
                {
                    int receivedData = (Int16)values[i].Value;
                    butCompletedLogging2.Invoke(new EventHandler(
                        delegate 
                        {
                            this.machine2.CompletedLogging = receivedData;
                            butCompletedLogging2.Text = this.machine2.CompletedLoggingDesc;
                            SetButtonStatusColor(butCompletedLogging2, receivedData);
                        }));                    
                }
                if (values[i].ItemName == tagMainBlock + "ST2ReqChkCodeVerify")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestVerifyCode2.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            this.machine2.RequestVerifyCode = val;
                            SetButtonStatusColor(butRequestVerifyCode2, receivedData);
                            if (val)
                                VerityCode(this.machine2, null);
                        }));
                }

                //Machine 3
                if (values[i].ItemName == tagMainBlock + "ST3_1StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc3.Invoke(new EventHandler(
                        delegate 
                        { 
                            this.machine3.OnlineFlag = receivedData;
                            butStatusMc3.Text = this.machine3.StatusName;
                            SetButtonMachineStatusColor(butStatusMc3, receivedData);
                        }));                    
                }
                if (values[i].ItemName == tagMainBlock + "ST3_1ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging3.Invoke(new EventHandler(
                        delegate 
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            butRequestLogging3.Text = val.ToString().ToUpper();
                            SetButtonStatusColor(butRequestLogging3, receivedData);
                            if(val)
                                KeepLogging((MachineModel)txtManchineName3.Tag, null);
                        }));                    
                }
                if (values[i].ItemName == tagMainBlock + "ST3_1LoggingApp")
                {
                    int receivedData = (Int16)values[i].Value;
                    butCompletedLogging3.Invoke(new EventHandler(
                        delegate 
                        {
                            this.machine3.CompletedLogging = receivedData;
                            butCompletedLogging3.Text = this.machine3.CompletedLoggingDesc;
                            SetButtonStatusColor(butCompletedLogging3, receivedData);
                        }));                    
                }
                if (values[i].ItemName == tagMainBlock + "ST3_1ReqChkCodeVerify")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestVerifyCode3.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            this.machine3.RequestVerifyCode = val;
                            SetButtonStatusColor(butRequestVerifyCode3, receivedData);
                            if (val)
                                VerityCode(this.machine3, null);
                        }));
                }

                //Machine 4
                if (values[i].ItemName == tagMainBlock + "ST3_2StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc4.Invoke(new EventHandler(
                        delegate 
                        { 
                            this.machine4.OnlineFlag = receivedData;
                            butStatusMc4.Text = this.machine4.StatusName;
                            SetButtonMachineStatusColor(butStatusMc4, receivedData);
                        }));                    
                }
                if (values[i].ItemName == tagMainBlock + "ST3_2ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging4.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            butRequestLogging4.Text = val.ToString().ToUpper();
                            SetButtonStatusColor(butRequestLogging4, receivedData);
                            if (val)
                                KeepLogging((MachineModel)txtManchineName4.Tag, null);
                        }));
                }
                if (values[i].ItemName == tagMainBlock + "ST3_2LoggingApp")
                {
                    int receivedData = (Int16)values[i].Value;
                    butCompletedLogging4.Invoke(new EventHandler(
                        delegate 
                        {
                            this.machine4.CompletedLogging = receivedData;
                            butCompletedLogging4.Text = this.machine4.CompletedLoggingDesc;
                            SetButtonStatusColor(butCompletedLogging4, receivedData);
                        }));                    
                }
                if (values[i].ItemName == tagMainBlock + "ST3_2ReqChkCodeVerify")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestVerifyCode4.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            this.machine4.RequestVerifyCode = val;
                            SetButtonStatusColor(butRequestVerifyCode4, receivedData);
                            if (val)
                                VerityCode(this.machine4, null);
                        }));
                }

                //Machine 5
                if (values[i].ItemName == tagMainBlock + "ST4StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc5.Invoke(new EventHandler(
                        delegate 
                        { 
                            this.machine5.OnlineFlag = receivedData;
                            butStatusMc5.Text = this.machine5.StatusName;
                            SetButtonMachineStatusColor(butStatusMc5, receivedData);
                        }));                    
                }
                if (values[i].ItemName == tagMainBlock + "ST4ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging5.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            butRequestLogging5.Text = val.ToString().ToUpper();
                            SetButtonStatusColor(butRequestLogging5, receivedData);
                            if (val)
                                KeepLogging((MachineModel)txtManchineName5.Tag, null);
                        }));
                }
                if (values[i].ItemName == tagMainBlock + "ST4LoggingApp")
                {
                    int receivedData = (Int16)values[i].Value;
                    butCompletedLogging5.Invoke(new EventHandler(
                        delegate 
                        {
                            this.machine5.CompletedLogging = receivedData;
                            butCompletedLogging5.Text = this.machine5.CompletedLoggingDesc;
                            SetButtonStatusColor(butCompletedLogging5, receivedData);
                        }));                    
                }
                if (values[i].ItemName == tagMainBlock + "ST4ReqChkCodeVerify")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestVerifyCode5.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            this.machine5.RequestVerifyCode = val;
                            SetButtonStatusColor(butRequestVerifyCode5, receivedData);
                            if (val)
                                VerityCode(this.machine5, null);
                        }));
                }

                //Machine 6
                if (values[i].ItemName == tagMainBlock + "ST5_1StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc6.Invoke(new EventHandler(
                        delegate 
                        { 
                            this.machine6.OnlineFlag = receivedData;
                            butStatusMc6.Text = this.machine6.StatusName;
                            SetButtonMachineStatusColor(butStatusMc6, receivedData);
                        }));                    
                }
                if (values[i].ItemName == tagMainBlock + "ST5_1ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging6.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            butRequestLogging6.Text = val.ToString().ToUpper();
                            SetButtonStatusColor(butRequestLogging6, receivedData);
                            if (val)
                                KeepLogging((MachineModel)txtManchineName6.Tag, null);
                        }));
                }
                if (values[i].ItemName == tagMainBlock + "ST5_1LoggingApp")
                {
                    int receivedData = (Int16)values[i].Value;
                    butCompletedLogging6.Invoke(new EventHandler(
                        delegate 
                        {
                            this.machine6.CompletedLogging = receivedData;
                            butCompletedLogging6.Text = this.machine6.CompletedLoggingDesc;
                            SetButtonStatusColor(butCompletedLogging6, receivedData);
                        }));                    
                }
                if (values[i].ItemName == tagMainBlock + "ST5_1ReqChkCodeVerify")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestVerifyCode6.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            this.machine6.RequestVerifyCode = val;
                            SetButtonStatusColor(butRequestVerifyCode6, receivedData);
                            if (val)
                                VerityCode(this.machine6, null);
                        }));
                }

                //Machine 7
                if (values[i].ItemName == tagMainBlock + "ST5_2StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc7.Invoke(new EventHandler(
                        delegate 
                        { 
                            this.machine7.OnlineFlag = receivedData;
                            butStatusMc7.Text = this.machine7.StatusName;
                            SetButtonMachineStatusColor(butStatusMc7, receivedData);
                        }));                    
                }
                if (values[i].ItemName == tagMainBlock + "ST5_2ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging7.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            butRequestLogging7.Text = val.ToString().ToUpper();
                            SetButtonStatusColor(butRequestLogging7, receivedData);
                            if (val)
                                KeepLogging((MachineModel)txtManchineName7.Tag, null);
                        }));
                }
                if (values[i].ItemName == tagMainBlock + "ST5_2LoggingApp")
                {
                    int receivedData = (Int16)values[i].Value;
                    butCompletedLogging7.Invoke(new EventHandler(
                        delegate 
                        {
                            this.machine7.CompletedLogging = receivedData;
                            butCompletedLogging7.Text = this.machine7.CompletedLoggingDesc;
                            SetButtonStatusColor(butCompletedLogging7, receivedData);
                        }));                    
                }
                if (values[i].ItemName == tagMainBlock + "ST5_2ReqChkCodeVerify")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestVerifyCode7.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            this.machine7.RequestVerifyCode = val;
                            SetButtonStatusColor(butRequestVerifyCode7, receivedData);
                            if (val)
                                VerityCode(this.machine7, null);
                        }));
                }
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

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (FormLoad != null)
                FormLoad(sender, e);

            _connectedPlc = false;
            DisableClock();
        }

        private void butConnect_Click(object sender, EventArgs e)
        {
            if (this.connectedPlc)
            {
                if (Disconnect_Click != null)
                    Disconnect_Click(sender, e);
            }else
            {
                if (Connect_Click != null)
                    Connect_Click(sender, e);
            }
        }

        public void EnableClock()
        {
            timerInter.Interval = 2000;
            timerInter.Enabled = true;
        }

        public void DisableClock()
        {
            timerInter.Enabled = false;
        }

        private void timerInter_Tick(object sender, EventArgs e)
        {
            if (InterLock != null)
                InterLock(sender, e);
        }

        private void butMakeReady_Click(object sender, EventArgs e)
        {
            if (MakeReady != null)
                MakeReady(sender, e);
        }

        private void butCompletedLogging1_Click(object sender, EventArgs e)
        {
            if (this.machine1.CompletedLogging != 0)
                return;

            if (CompleteAction != null)
                CompleteAction(sender, e);
        }

        private void butCompletedLogging2_Click(object sender, EventArgs e)
        {
            if (this.machine2.CompletedLogging != 0)
                return;

            if (CompleteAction != null)
                CompleteAction(sender, e);
        }

        private void butCompletedLogging3_Click(object sender, EventArgs e)
        {
            if (this.machine3.CompletedLogging != 0)
                return;

            if (CompleteAction != null)
                CompleteAction(sender, e);
        }

        private void butCompletedLogging4_Click(object sender, EventArgs e)
        {
            if (this.machine4.CompletedLogging != 0)
                return;

            if (CompleteAction != null)
                CompleteAction(sender, e);
        }

        private void butCompletedLogging5_Click(object sender, EventArgs e)
        {
            if (this.machine5.CompletedLogging != 0)
                return;

            if (CompleteAction != null)
                CompleteAction(sender, e);
        }

        private void butCompletedLogging6_Click(object sender, EventArgs e)
        {
            if (this.machine6.CompletedLogging != 0)
                return;

            if (CompleteAction != null)
                CompleteAction(sender, e);
        }

        private void butCompletedLogging7_Click(object sender, EventArgs e)
        {
            if (this.machine7.CompletedLogging != 0)
                return;

            if (CompleteAction != null)
                CompleteAction(sender, e);
        }
    }
}
