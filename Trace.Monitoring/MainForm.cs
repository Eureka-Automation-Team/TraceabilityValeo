﻿using Opc.Da;
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

        private Subscription _groupRead1;
        private SubscriptionState _groupStateRead1;
        private Subscription _groupRead2;
        private SubscriptionState _groupStateRead2;
        private Subscription _groupRead3;
        private SubscriptionState _groupStateRead3;
        private Subscription _groupRead4;
        private SubscriptionState _groupStateRead4;
        private Subscription _groupRead5;
        private SubscriptionState _groupStateRead5;
        private Subscription _groupRead6;
        private SubscriptionState _groupStateRead6;
        private Subscription _groupRead7;
        private SubscriptionState _groupStateRead7;

        private Subscription _groupWrite;
        private SubscriptionState _groupStateWrite;
        private Subscription _groupWriteMc1;
        private SubscriptionState _groupStateWriteMc1;
        private Subscription _groupWriteMc2;
        private SubscriptionState _groupStateWriteMc2;
        private Subscription _groupWriteMc3;
        private SubscriptionState _groupStateWriteMc3;
        private Subscription _groupWriteMc4;
        private SubscriptionState _groupStateWriteMc4;
        private Subscription _groupWriteMc5;
        private SubscriptionState _groupStateWriteMc5;
        private Subscription _groupWriteMc6;
        private SubscriptionState _groupStateWriteMc6;
        private Subscription _groupWriteMc7;
        private SubscriptionState _groupStateWriteMc7;

        private Item[] _items;
        private Item[] _items1;
        private Item[] _items2;
        private Item[] _items3;
        private Item[] _items4;
        private Item[] _items5;
        private Item[] _items6;
        private Item[] _items7;

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

        public Subscription groupWriteMc1
        {
            get { return _groupWriteMc1; }
            set { _groupWriteMc1 = value; }
        }
        public SubscriptionState groupStateWriteMc1
        {
            get { return _groupStateWriteMc1; }
            set { _groupStateWriteMc1 = value; }
        }

        public Subscription groupWriteMc2
        {
            get { return _groupWriteMc2; }
            set { _groupWriteMc2 = value; }
        }
        public SubscriptionState groupStateWriteMc2
        {
            get { return _groupStateWriteMc2; }
            set { _groupStateWriteMc2 = value; }
        }
        public Subscription groupWriteMc3
        {
            get { return _groupWriteMc3; }
            set { _groupWriteMc3 = value; }
        }
        public SubscriptionState groupStateWriteMc3
        {
            get { return _groupStateWriteMc3; }
            set { _groupStateWriteMc3 = value; }
        }
        public Subscription groupWriteMc4
        {
            get { return _groupWriteMc4; }
            set { _groupWriteMc4 = value; }
        }
        public SubscriptionState groupStateWriteMc4
        {
            get { return _groupStateWriteMc4; }
            set { _groupStateWriteMc4 = value; }
        }
        public Subscription groupWriteMc5
        {
            get { return _groupWriteMc5; }
            set { _groupWriteMc5 = value; }
        }
        public SubscriptionState groupStateWriteMc5
        {
            get { return _groupStateWriteMc5; }
            set { _groupStateWriteMc5 = value; }
        }
        public Subscription groupWriteMc6
        {
            get { return _groupWriteMc6; }
            set { _groupWriteMc6 = value; }
        }
        public SubscriptionState groupStateWriteMc6
        {
            get { return _groupStateWriteMc6; }
            set { _groupStateWriteMc6 = value; }
        }
        public Subscription groupWriteMc7
        {
            get { return _groupWriteMc7; }
            set { _groupWriteMc7 = value; }
        }
        public SubscriptionState groupStateWriteMc7
        {
            get { return _groupStateWriteMc7; }
            set { _groupStateWriteMc7 = value; }
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

        public Item[] items1
        {
            get { return _items1; }
            set { _items1 = value; }
        }
        public Item[] items2
        {
            get { return _items2; }
            set { _items2 = value; }
        }
        public Item[] items3
        {
            get { return _items3; }
            set { _items3 = value; }
        }
        public Item[] items4
        {
            get { return _items4; }
            set { _items4 = value; }
        }
        public Item[] items5
        {
            get { return _items5; }
            set { _items5 = value; }
        }
        public Item[] items6
        {
            get { return _items6; }
            set { _items6 = value; }
        }
        public Item[] items7
        {
            get { return _items7; }
            set { _items7 = value; }
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

        public MachineModel machine1
        {
            get { return _machine1; }
            set
            {
                _machine1 = value;

                if (_machine1 != null)
                {
                    butStatusMc1.Text = _machine1.StatusName;
                    SetButtonMachineStatusColor(butStatusMc1, _machine1.OnlineFlag);

                    butRequestLogging1.Text = _machine1.RequestLogging.ToString().ToUpper();
                    SetButtonStatusColor(butRequestLogging1, Convert.ToInt32(_machine1.RequestLogging));

                    txtManchineName1.Tag = _machine1;
                    txtManchineName1.Text = _machine1.ManchineName;
                    txtMessageResult1.Text = _machine1.MessageResult;
                    butCompletedLogging1.Text = _machine1.CompletedLoggingDesc;
                    txtPosition1.Text = _machine1.TighteningPosition.ToString();
                    SetButtonStatusColor(butCompletedLogging1, _machine1.CompletedLogging);
                    if (_machine1.RequestLogging && _machine1.CompletedLogging == 0)
                        KeepLogging(_machine1, null);

                    if (_machine1.RequestVerifyCode)
                    {
                        butRequestVerifyCode1.Text = _machine1.CodeVerifyResultDesc;
                    }
                    else
                    {
                        butRequestVerifyCode1.Text = string.Empty;
                    }
                    SetButtonStatusColor(butRequestVerifyCode1, Convert.ToInt32(_machine1.RequestVerifyCode));
                    if (_machine1.RequestVerifyCode && _machine1.CodeVerifyResult == 0)
                        VerityCode(_machine1, null);
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
                    butStatusMc2.Text = _machine2.StatusName;
                    SetButtonMachineStatusColor(butStatusMc2, _machine2.OnlineFlag);

                    butRequestLogging2.Text = _machine2.RequestLogging.ToString().ToUpper();
                    SetButtonStatusColor(butRequestLogging2, Convert.ToInt32(_machine2.RequestLogging));

                    txtManchineName2.Tag = _machine2;
                    txtManchineName2.Text = _machine2.ManchineName;
                    txtMessageResult2.Text = _machine2.MessageResult;
                    butCompletedLogging2.Text = _machine2.CompletedLoggingDesc;
                    txtPosition2.Text = _machine2.TighteningPosition.ToString();
                    SetButtonStatusColor(butCompletedLogging2, _machine2.CompletedLogging);
                    if (_machine2.RequestLogging && _machine2.CompletedLogging == 0)
                        KeepLogging(_machine2, null);

                    if (_machine2.RequestVerifyCode)
                    {
                        butRequestVerifyCode2.Text = _machine2.CodeVerifyResultDesc;
                    }
                    else
                    {
                        butRequestVerifyCode2.Text = string.Empty;
                    }
                    SetButtonStatusColor(butRequestVerifyCode2, Convert.ToInt32(_machine2.RequestVerifyCode));
                    if (_machine2.RequestVerifyCode && _machine2.CodeVerifyResult == 0)
                        VerityCode(_machine2, null);
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
                    butStatusMc3.Text = _machine3.StatusName;
                    SetButtonMachineStatusColor(butStatusMc3, _machine3.OnlineFlag);

                    butRequestLogging3.Text = _machine3.RequestLogging.ToString().ToUpper();
                    SetButtonStatusColor(butRequestLogging3, Convert.ToInt32(_machine3.RequestLogging));

                    txtManchineName3.Tag = _machine3;
                    txtManchineName3.Text = _machine3.ManchineName;
                    txtMessageResult3.Text = _machine3.MessageResult;
                    butCompletedLogging3.Text = _machine3.CompletedLoggingDesc;
                    SetButtonStatusColor(butCompletedLogging3, _machine3.CompletedLogging);
                    if (_machine3.RequestLogging && _machine3.CompletedLogging == 0)
                        KeepLogging(_machine3, null);

                    if (_machine3.RequestVerifyCode)
                    {
                        butRequestVerifyCode3.Text = _machine3.CodeVerifyResultDesc;
                    }
                    else
                    {
                        butRequestVerifyCode3.Text = string.Empty;
                    }
                    SetButtonStatusColor(butRequestVerifyCode3, Convert.ToInt32(_machine3.RequestVerifyCode));
                    if (_machine3.RequestVerifyCode && _machine3.CodeVerifyResult == 0)
                        VerityCode(_machine3, null);
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
                    butStatusMc4.Text = _machine4.StatusName;
                    SetButtonMachineStatusColor(butStatusMc4, _machine4.OnlineFlag);

                    butRequestLogging4.Text = _machine4.RequestLogging.ToString().ToUpper();
                    SetButtonStatusColor(butRequestLogging4, Convert.ToInt32(_machine4.RequestLogging));

                    txtManchineName4.Tag = _machine4;
                    txtManchineName4.Text = _machine4.ManchineName;
                    txtMessageResult4.Text = _machine4.MessageResult;
                    butCompletedLogging4.Text = _machine4.CompletedLoggingDesc;
                    SetButtonStatusColor(butCompletedLogging4, _machine4.CompletedLogging);
                    if (_machine4.RequestLogging && _machine4.CompletedLogging == 0)
                        KeepLogging(_machine4, null);

                    if (_machine4.RequestVerifyCode)
                    {
                        butRequestVerifyCode4.Text = _machine4.CodeVerifyResultDesc;
                    }
                    else
                    {
                        butRequestVerifyCode4.Text = string.Empty;
                    }
                    SetButtonStatusColor(butRequestVerifyCode4, Convert.ToInt32(_machine4.RequestVerifyCode));
                    if (_machine4.RequestVerifyCode && _machine4.CodeVerifyResult == 0)
                        VerityCode(_machine4, null);
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
                    butStatusMc5.Text = _machine5.StatusName;
                    SetButtonMachineStatusColor(butStatusMc5, _machine5.OnlineFlag);

                    butRequestLogging5.Text = _machine5.RequestLogging.ToString().ToUpper();
                    SetButtonStatusColor(butRequestLogging5, Convert.ToInt32(_machine5.RequestLogging));

                    txtManchineName5.Tag = _machine5;
                    txtManchineName5.Text = _machine5.ManchineName;
                    txtMessageResult5.Text = _machine5.MessageResult;
                    butCompletedLogging5.Text = _machine5.CompletedLoggingDesc;
                    txtPosition4.Text = _machine5.TighteningPosition.ToString();
                    SetButtonStatusColor(butCompletedLogging5, _machine5.CompletedLogging);
                    if (_machine5.RequestLogging && _machine5.CompletedLogging == 0)
                        KeepLogging(_machine5, null);

                    if (_machine5.RequestVerifyCode)
                    {
                        butRequestVerifyCode5.Text = _machine5.CodeVerifyResultDesc;
                    }
                    else
                    {
                        butRequestVerifyCode5.Text = string.Empty;
                    }
                    SetButtonStatusColor(butRequestVerifyCode5, Convert.ToInt32(_machine5.RequestVerifyCode));
                    if (_machine5.RequestVerifyCode && _machine5.CodeVerifyResult == 0)
                        VerityCode(_machine5, null);
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
                    butStatusMc6.Text = _machine6.StatusName;
                    SetButtonMachineStatusColor(butStatusMc6, _machine6.OnlineFlag);

                    butRequestLogging6.Text = _machine6.RequestLogging.ToString().ToUpper();
                    SetButtonStatusColor(butRequestLogging6, Convert.ToInt32(_machine6.RequestLogging));

                    txtManchineName6.Tag = _machine6;
                    txtManchineName6.Text = _machine6.ManchineName;
                    txtMessageResult6.Text = _machine6.MessageResult;
                    butCompletedLogging6.Text = _machine6.CompletedLoggingDesc;
                    SetButtonStatusColor(butCompletedLogging6, _machine6.CompletedLogging);
                    if (_machine6.RequestLogging && _machine6.CompletedLogging == 0)
                        KeepLogging(_machine6, null);

                    if (_machine6.RequestVerifyCode)
                    {
                        butRequestVerifyCode6.Text = _machine6.CodeVerifyResultDesc;
                    }
                    else
                    {
                        butRequestVerifyCode6.Text = string.Empty;
                    }
                    SetButtonStatusColor(butRequestVerifyCode6, Convert.ToInt32(_machine6.RequestVerifyCode));
                    if (_machine6.RequestVerifyCode && _machine6.CodeVerifyResult == 0)
                        VerityCode(_machine6, null);

                    //***********************
                    if (_machine6.RequestCodeActuater)
                    {
                        butRequestCodeActuater1.Text = _machine6.ActuatorResultDesc;                        
                    }
                    else
                    {
                        butRequestCodeActuater1.Text = string.Empty;
                    }
                    SetButtonStatusColor(butRequestCodeActuater1, Convert.ToInt32(_machine6.RequestCodeActuater));
                    if (_machine6.RequestCodeActuater && _machine6.ActuatorResult == 0)
                        VerityActuater(_machine6, null);
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
                    butStatusMc7.Text = _machine7.StatusName;
                    SetButtonMachineStatusColor(butStatusMc7, _machine7.OnlineFlag);

                    butRequestLogging7.Text = _machine7.RequestLogging.ToString().ToUpper();
                    SetButtonStatusColor(butRequestLogging7, Convert.ToInt32(_machine7.RequestLogging));

                    txtManchineName7.Tag = _machine7;
                    txtManchineName7.Text = _machine7.ManchineName;
                    txtMessageResult7.Text = _machine7.MessageResult;
                    butCompletedLogging7.Text = _machine7.CompletedLoggingDesc;
                    SetButtonStatusColor(butCompletedLogging7, _machine7.CompletedLogging);
                    if (_machine7.RequestLogging && _machine7.CompletedLogging == 0)
                        KeepLogging(_machine7, null);

                    if (_machine7.RequestVerifyCode)
                    {
                        butRequestVerifyCode7.Text = _machine7.CodeVerifyResultDesc;
                    }
                    else
                    {
                        butRequestVerifyCode7.Text = string.Empty;
                    }

                    SetButtonStatusColor(butRequestVerifyCode7, Convert.ToInt32(_machine7.RequestVerifyCode));
                    if (_machine7.RequestVerifyCode && _machine7.CodeVerifyResult == 0)
                        VerityCode(_machine7, null);
                    //***********************
                    if (_machine7.RequestCodeActuater)
                    {
                        butRequestCodeActuater2.Text = _machine7.ActuatorResultDesc;
                    }
                    else
                    {
                        butRequestCodeActuater2.Text = string.Empty;
                    }
                    SetButtonStatusColor(butRequestCodeActuater2, Convert.ToInt32(_machine7.RequestCodeActuater));
                    if (_machine7.RequestCodeActuater && _machine7.ActuatorResult == 0)
                        VerityActuater(_machine7, null);
                }
            }
        }

        public string strConnectionMessage
        {
            get { return tslConnectionStatus.Text; }
            set { tslConnectionStatus.Text = value; }
        }

        public string TagValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Subscription groupReadMachine1
        {
            get { return _groupRead1; }
            set { _groupRead1 = value; }
        }
        public SubscriptionState groupStateReadMachine1
        {
            get { return _groupStateRead1; }
            set { _groupStateRead1 = value; }
        }
        public Subscription groupReadMachine2
        {
            get { return _groupRead2; }
            set { _groupRead2 = value; }
        }
        public SubscriptionState groupStateReadMachine2
        {
            get { return _groupStateRead2; }
            set { _groupStateRead2 = value; }
        }
        public Subscription groupReadMachine3
        {
            get { return _groupRead3; }
            set { _groupRead3 = value; }
        }
        public SubscriptionState groupStateReadMachine3
        {
            get { return _groupStateRead3; }
            set { _groupStateRead3 = value; }
        }
        public Subscription groupReadMachine4
        {
            get { return _groupRead4; }
            set { _groupRead4 = value; }
        }
        public SubscriptionState groupStateReadMachine4
        {
            get { return _groupStateRead4; }
            set { _groupStateRead4 = value; }
        }
        public Subscription groupReadMachine5
        {
            get { return _groupRead5; }
            set { _groupRead5 = value; }
        }
        public SubscriptionState groupStateReadMachine5
        {
            get { return _groupStateRead5; }
            set { _groupStateRead5 = value; }
        }
        public Subscription groupReadMachine6
        {
            get { return _groupRead6; }
            set { _groupRead6 = value; }
        }
        public SubscriptionState groupStateReadMachine6
        {
            get { return _groupStateRead6; }
            set { _groupStateRead6 = value; }
        }
        public Subscription groupReadMachine7
        {
            get { return _groupRead7; }
            set { _groupRead7 = value; }
        }
        public SubscriptionState groupStateReadMachine7
        {
            get { return _groupStateRead7; }
            set { _groupStateRead7 = value; }
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
        public event EventHandler ReadTag;
        public event EventHandler WriteTag;
        public event EventHandler MonitoringTag;

        public MainForm()
        {
            InitializeComponent();

            this._presenter = new MainPresenter(this);
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

        public void EnableClock()
        {
            timerInter.Interval = 2000;
            timerInter.Enabled = true;
            //timer1.Enabled = true;
            timerConnect.Enabled = false;
            timerRefresh.Enabled = true;
        }

        public void DisableClock()
        {
            timerInter.Enabled = false;
            timerConnect.Enabled = true;
            timerRefresh.Enabled = false;
            //timer1.Enabled = false;
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

            if (systemReady)
            {
                Cursor.Current = Cursors.WaitCursor;
                group_DataChanged(null, null, groupRead.Read(groupRead.Items));
                Cursor.Current = Cursors.Default;
            }
        }

        private void butCompletedLogging1_Click(object sender, EventArgs e)
        {
            if (!this.machine1.RequestLogging)
                return;

            if (this.machine1.CompletedLogging != 0)
                return;

            if (CompleteAction != null)
                CompleteAction(sender, e);
        }

        private void butCompletedLogging2_Click(object sender, EventArgs e)
        {
            if (!this.machine2.RequestLogging)
                return;

            if (this.machine2.CompletedLogging != 0)
                return;

            if (CompleteAction != null)
                CompleteAction(sender, e);
        }

        private void butCompletedLogging3_Click(object sender, EventArgs e)
        {
            if (!this.machine3.RequestLogging)
                return;

            if (this.machine3.CompletedLogging != 0)
                return;

            if (CompleteAction != null)
                CompleteAction(sender, e);
        }

        private void butCompletedLogging4_Click(object sender, EventArgs e)
        {
            if (!this.machine4.RequestLogging)
                return;

            if (this.machine4.CompletedLogging != 0)
                return;

            if (CompleteAction != null)
                CompleteAction(sender, e);
        }

        private void butCompletedLogging5_Click(object sender, EventArgs e)
        {
            //if (!this.machine5.RequestLogging)
            //    return;

            //if (this.machine5.CompletedLogging != 0)
            //    return;

            if (CompleteAction != null)
                CompleteAction(sender, e);
        }

        private void butCompletedLogging6_Click(object sender, EventArgs e)
        {
            if (!this.machine6.RequestLogging)
                return;

            if (this.machine6.CompletedLogging != 0)
                return;

            if (CompleteAction != null)
                CompleteAction(sender, e);
        }

        private void butCompletedLogging7_Click(object sender, EventArgs e)
        {
            if (!this.machine7.RequestLogging)
                return;

            if (this.machine7.CompletedLogging != 0)
                return;

            if (CompleteAction != null)
                CompleteAction(sender, e);
        }

        private void butRefresh_Click(object sender, EventArgs e)
        {
            if (RefreshData != null)
                RefreshData(sender, e);
        }

        private void timerConnect_Tick(object sender, EventArgs e)
        {
            if (Connect_Click != null)
                Connect_Click(sender, e);
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            if (RefreshData != null)
                RefreshData(sender, e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (MonitoringTag != null)
                MonitoringTag(sender, e);
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
                    butStatusMc1.Invoke(new EventHandler(
                        delegate
                        {
                            var mac = this.machine1;
                            mac.OnlineFlag = receivedData;
                            this.machine1 = mac;
                        }));
                }

                //Machine 2 Status
                if (values[i].ItemName == tagMainBlock + "ST2StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc2.Invoke(new EventHandler(
                        delegate
                        {
                            var mac = this.machine2;
                            mac.OnlineFlag = receivedData;
                            this.machine2 = mac;
                        }));
                }

                //Machine 3 Status
                if (values[i].ItemName == tagMainBlock + "ST3_1StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc3.Invoke(new EventHandler(
                        delegate
                        {
                            var mac = this.machine3;
                            mac.OnlineFlag = receivedData;
                            this.machine3 = mac;
                        }));
                }

                //Machine 4 Status
                if (values[i].ItemName == tagMainBlock + "ST3_2StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc4.Invoke(new EventHandler(
                        delegate
                        {
                            var mac = this.machine4;
                            mac.OnlineFlag = receivedData;
                            this.machine4 = mac;
                        }));
                }

                //Machine 5 Status
                if (values[i].ItemName == tagMainBlock + "ST4StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc5.Invoke(new EventHandler(
                        delegate
                        {
                            var mac = this.machine5;
                            mac.OnlineFlag = receivedData;
                            this.machine5 = mac;
                        }));
                }

                //Machine 6 Status
                if (values[i].ItemName == tagMainBlock + "ST5_1StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc6.Invoke(new EventHandler(
                        delegate
                        {
                            var mac = this.machine6;
                            mac.OnlineFlag = receivedData;
                            this.machine6 = mac;
                        }));
                }

                //Machine 7 Status
                if (values[i].ItemName == tagMainBlock + "ST5_2StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc7.Invoke(new EventHandler(
                        delegate
                        {
                            var mac = this.machine7;
                            mac.OnlineFlag = receivedData;
                            this.machine7 = mac;
                        }));
                }
            }
        }

        public void group_DataChanged1(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].ItemName == tagMainBlock + "ST1ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging1.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            var mac = this.machine1;
                            mac.RequestLogging = val;
                            this.machine1 = mac;

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
                            var mac = this.machine1;
                            mac.CompletedLogging = receivedData;
                            this.machine1 = mac;
                        }));
                }
                if (values[i].ItemName == tagMainBlock + "ST1ReqChkCodeVerify")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestVerifyCode1.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            var mac = this.machine1;
                            mac.RequestVerifyCode = val;
                            this.machine1 = mac;
                            if (val)
                                VerityCode(_machine1, null);
                        }));
                }
                //if (values[i].ItemName == tagMainBlock + "ST1TestResult[19]")
                //{
                //    int receivedData;
                //    var succes = Int32.TryParse(values[i].Value.ToString(), out receivedData);
                //    if (!succes)
                //        receivedData = 0;

                //    txtPosition1.Invoke(new EventHandler(
                //        delegate
                //        {
                //            var mac = this.machine1;
                //            mac.TighteningPosition = receivedData;
                //            this.machine1 = mac;
                //        }));
                //}
            }
        }

        public void group_DataChanged2(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].ItemName == tagMainBlock + "ST2ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging2.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            var mac = this.machine2;
                            mac.RequestLogging = val;
                            this.machine2 = mac;

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
                            var mac = this.machine2;
                            mac.CompletedLogging = receivedData;
                            this.machine2 = mac;
                        }));
                }
                if (values[i].ItemName == tagMainBlock + "ST2ReqChkCodeVerify")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestVerifyCode2.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            var mac = this.machine2;
                            mac.RequestVerifyCode = val;
                            this.machine2 = mac;
                            if (val)
                                VerityCode(this.machine2, null);
                        }));
                }
                //if (values[i].ItemName == tagMainBlock + "ST2TestResult[19]")
                //{
                //    int receivedData;
                //    var succes = Int32.TryParse(values[i].Value.ToString(), out receivedData);
                //    if (!succes)
                //        receivedData = 0;

                //    txtPosition2.Invoke(new EventHandler(
                //        delegate
                //        {
                //            var mac = this.machine2;
                //            mac.TighteningPosition = receivedData;
                //            this.machine2 = mac;
                //        }));
                //}
            }
        }

        public void group_DataChanged3(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].ItemName == tagMainBlock + "ST3_1ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging3.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            var mac = this.machine3;
                            mac.RequestLogging = val;
                            this.machine3 = mac;

                            if (val)
                                KeepLogging((MachineModel)txtManchineName3.Tag, null);
                        }));
                }
                if (values[i].ItemName == tagMainBlock + "ST3_1LoggingApp")
                {
                    int receivedData = (Int16)values[i].Value;
                    butCompletedLogging3.Invoke(new EventHandler(
                        delegate
                        {
                            var mac = this.machine3;
                            mac.CompletedLogging = receivedData;
                            this.machine3 = mac;
                        }));
                }
                if (values[i].ItemName == tagMainBlock + "ST3_1ReqChkCodeVerify")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestVerifyCode3.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            var mac = this.machine3;
                            mac.RequestVerifyCode = val;
                            this.machine3 = mac;
                            if (val)
                                VerityCode(this.machine3, null);
                        }));
                }
            }
        }

        public void group_DataChanged4(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].ItemName == tagMainBlock + "ST3_2ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging4.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            var mac = this.machine4;
                            mac.RequestLogging = val;
                            this.machine4 = mac;

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
                            var mac = this.machine4;
                            mac.CompletedLogging = receivedData;
                            this.machine4 = mac;
                        }));
                }
                if (values[i].ItemName == tagMainBlock + "ST3_2ReqChkCodeVerify")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestVerifyCode4.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            var mac = this.machine4;
                            mac.RequestVerifyCode = val;
                            this.machine4 = mac;
                            if (val)
                                VerityCode(this.machine4, null);
                        }));
                }
            }
        }

        public void group_DataChanged5(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].ItemName == tagMainBlock + "ST4ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging5.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            var mac = this.machine5;
                            mac.RequestLogging = val;
                            this.machine5 = mac;

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
                            var mac = this.machine5;
                            mac.CompletedLogging = receivedData;
                            this.machine5 = mac;
                        }));
                }
                if (values[i].ItemName == tagMainBlock + "ST4ReqChkCodeVerify")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestVerifyCode5.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            var mac = this.machine5;
                            mac.RequestVerifyCode = val;
                            this.machine5 = mac;
                            if (val)
                                VerityCode(this.machine5, null);
                        }));
                }
                //if (values[i].ItemName == tagMainBlock + "ST4TestResult[19]")
                //{
                //    int receivedData;
                //    var succes = Int32.TryParse(values[i].Value.ToString(), out receivedData);
                //    if (!succes)
                //        receivedData = 0;

                //    txtPosition4.Invoke(new EventHandler(
                //        delegate
                //        {
                //            var mac = this.machine5;
                //            mac.TighteningPosition = receivedData;
                //            this.machine5 = mac;
                //        }));
                //}
            }
        }

        public void group_DataChanged6(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].ItemName == tagMainBlock + "ST5_1ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging6.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            var mac = this.machine6;
                            mac.RequestLogging = val;
                            this.machine6 = mac;

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
                            var mac = this.machine6;
                            mac.CompletedLogging = receivedData;
                            this.machine6 = mac;
                        }));
                }
                if (values[i].ItemName == tagMainBlock + "ST5_1ReqChkCodeVerify")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestVerifyCode6.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            var mac = this.machine6;
                            mac.RequestVerifyCode = val;
                            this.machine6 = mac;
                            if (val)
                                VerityCode(_machine6, null);
                        }));
                }
                if (values[i].ItemName == tagMainBlock + "ST5_1ReqCodeActuater")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestCodeActuater1.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            var mac = this.machine6;
                            mac.RequestCodeActuater = val;
                            this.machine6 = mac;
                            _machine6.RequestCodeActuater = val;
                            VerityActuater(_machine6, null);
                        }));
                }
            }
        }

        public void group_DataChanged7(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].ItemName == tagMainBlock + "ST5_2ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging7.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            var mac = this.machine7;
                            mac.RequestLogging = val;
                            this.machine7 = mac;

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
                            var mac = this.machine7;
                            mac.CompletedLogging = receivedData;
                            this.machine7 = mac;
                        }));
                }
                if (values[i].ItemName == tagMainBlock + "ST5_2ReqChkCodeVerify")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestVerifyCode7.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            var mac = this.machine7;
                            mac.RequestVerifyCode = val;
                            this.machine7 = mac;
                            if (val)
                                VerityCode(_machine7, null);
                        }));
                }
                if (values[i].ItemName == tagMainBlock + "ST5_2ReqCodeActuater")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestCodeActuater2.Invoke(new EventHandler(
                        delegate
                        {
                            bool val = Convert.ToBoolean(receivedData);
                            var mac = this.machine7;
                            mac.RequestCodeActuater = val;
                            this.machine7 = mac;
                            _machine7.RequestCodeActuater = val;
                            VerityActuater(_machine7, null);
                        }));
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (WriteTag != null)
                WriteTag(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (ReadTag != null)
                ReadTag(sender, e);
        }
    }
}
