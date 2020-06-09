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
                    txtManchineName1.Text = _machine1.ManchineName;
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
                    txtManchineName2.Text = _machine2.ManchineName;
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
                    txtManchineName3.Text = _machine3.ManchineName;
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
                    txtManchineName4.Text = _machine4.ManchineName;
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
                    txtManchineName5.Text = _machine5.ManchineName;
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
                    txtManchineName6.Text = _machine6.ManchineName;
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
                    txtManchineName7.Text = _machine7.ManchineName;
                }
            }
        }

        public event EventHandler FormLoad;
        public event EventHandler Connect_Click;
        public event EventHandler Disconnect_Click;
        public event EventHandler InterLock;
        public event EventHandler MakeReady;
        public event EventHandler LoggingMachine3_1;

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
                if (values[i].ItemName == tagMainBlock + "." + "ST1StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc1.Invoke(new EventHandler(
                        delegate 
                        { 
                            butStatusMc1.Text = Convert.ToBoolean(receivedData) ? "ONLINE" : "OFFLINE";
                            SetButtonStatusColor(butStatusMc1, Convert.ToBoolean(receivedData));
                        }
                        ));                    
                }
                if (values[i].ItemName == tagMainBlock + "." + "ST1ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging1.Invoke(new EventHandler(delegate { butRequestLogging1.Text = Convert.ToBoolean(receivedData).ToString().ToUpper(); }));
                    SetButtonStatusColor(butRequestLogging1, Convert.ToBoolean(receivedData));
                }
                if (values[i].ItemName == tagMainBlock + "." + "ST1LoggingApp")
                {
                    int receivedData = (Int16)values[i].Value;
                    butCompletedLogging1.Invoke(new EventHandler(delegate { butCompletedLogging1.Text = Convert.ToBoolean(receivedData).ToString().ToUpper(); }));
                    SetButtonStatusColor(butCompletedLogging1, Convert.ToBoolean(receivedData));
                }

                //Machine 2
                if (values[i].ItemName == tagMainBlock + "." + "ST2StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc2.Invoke(new EventHandler(delegate { butStatusMc2.Text = Convert.ToBoolean(receivedData) ? "ONLINE" : "OFFLINE"; }));
                    SetButtonStatusColor(butStatusMc2, Convert.ToBoolean(receivedData));
                }
                if (values[i].ItemName == tagMainBlock + "." + "ST2ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging2.Invoke(new EventHandler(delegate { butRequestLogging2.Text = Convert.ToBoolean(receivedData).ToString().ToUpper(); }));
                    SetButtonStatusColor(butRequestLogging2, Convert.ToBoolean(receivedData));
                }
                if (values[i].ItemName == tagMainBlock + "." + "ST2LoggingApp")
                {
                    int receivedData = (Int16)values[i].Value;
                    butCompletedLogging2.Invoke(new EventHandler(delegate { butCompletedLogging2.Text = Convert.ToBoolean(receivedData).ToString().ToUpper(); }));
                    SetButtonStatusColor(butCompletedLogging2, Convert.ToBoolean(receivedData));
                }

                //Machine 3
                if (values[i].ItemName == tagMainBlock + "." + "ST3_1StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc3.Invoke(new EventHandler(delegate { butStatusMc3.Text = Convert.ToBoolean(receivedData) ? "ONLINE" : "OFFLINE"; }));
                    SetButtonStatusColor(butStatusMc3, Convert.ToBoolean(receivedData));
                }
                if (values[i].ItemName == tagMainBlock + "." + "ST3_1ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging3.Invoke(new EventHandler(
                        delegate 
                        { 
                            butRequestLogging3.Text = Convert.ToBoolean(receivedData).ToString().ToUpper();
                            SetButtonStatusColor(butRequestLogging3, Convert.ToBoolean(receivedData));
                            LoggingMachine3_1(values, null);
                        }));                    
                }
                if (values[i].ItemName == tagMainBlock + "." + "ST3_1LoggingApp")
                {
                    int receivedData = (Int16)values[i].Value;
                    butCompletedLogging3.Invoke(new EventHandler(delegate { butCompletedLogging3.Text = Convert.ToBoolean(receivedData).ToString().ToUpper(); }));
                    SetButtonStatusColor(butCompletedLogging3, Convert.ToBoolean(receivedData));
                }

                //Machine 4
                if (values[i].ItemName == tagMainBlock + "." + "ST3_2StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc4.Invoke(new EventHandler(delegate { butStatusMc4.Text = Convert.ToBoolean(receivedData) ? "ONLINE" : "OFFLINE"; }));
                    SetButtonStatusColor(butStatusMc4, Convert.ToBoolean(receivedData));
                }
                if (values[i].ItemName == tagMainBlock + "." + "ST3_2ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging4.Invoke(new EventHandler(delegate { butRequestLogging4.Text = Convert.ToBoolean(receivedData).ToString().ToUpper(); }));
                    SetButtonStatusColor(butRequestLogging4, Convert.ToBoolean(receivedData));
                }
                if (values[i].ItemName == tagMainBlock + "." + "ST3_2LoggingApp")
                {
                    int receivedData = (Int16)values[i].Value;
                    butCompletedLogging4.Invoke(new EventHandler(delegate { butCompletedLogging4.Text = Convert.ToBoolean(receivedData).ToString().ToUpper(); }));
                    SetButtonStatusColor(butCompletedLogging4, Convert.ToBoolean(receivedData));
                }

                //Machine 5
                if (values[i].ItemName == tagMainBlock + "." + "ST4StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc5.Invoke(new EventHandler(delegate { butStatusMc5.Text = Convert.ToBoolean(receivedData) ? "ONLINE" : "OFFLINE"; }));
                    SetButtonStatusColor(butStatusMc5, Convert.ToBoolean(receivedData));
                }
                if (values[i].ItemName == tagMainBlock + "." + "ST4ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging5.Invoke(new EventHandler(delegate { butRequestLogging5.Text = Convert.ToBoolean(receivedData).ToString().ToUpper(); }));
                    SetButtonStatusColor(butRequestLogging5, Convert.ToBoolean(receivedData));
                }
                if (values[i].ItemName == tagMainBlock + "." + "ST4LoggingApp")
                {
                    int receivedData = (Int16)values[i].Value;
                    butCompletedLogging5.Invoke(new EventHandler(delegate { butCompletedLogging5.Text = Convert.ToBoolean(receivedData).ToString().ToUpper(); }));
                    SetButtonStatusColor(butCompletedLogging5, Convert.ToBoolean(receivedData));
                }

                //Machine 6
                if (values[i].ItemName == tagMainBlock + "." + "ST5_1StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc6.Invoke(new EventHandler(delegate { butStatusMc6.Text = Convert.ToBoolean(receivedData) ? "ONLINE" : "OFFLINE"; }));
                    SetButtonStatusColor(butStatusMc6, Convert.ToBoolean(receivedData));
                }
                if (values[i].ItemName == tagMainBlock + "." + "ST5_1ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging6.Invoke(new EventHandler(delegate { butRequestLogging6.Text = Convert.ToBoolean(receivedData).ToString().ToUpper(); }));
                    SetButtonStatusColor(butRequestLogging6, Convert.ToBoolean(receivedData));
                }
                if (values[i].ItemName == tagMainBlock + "." + "ST5_1LoggingApp")
                {
                    int receivedData = (Int16)values[i].Value;
                    butCompletedLogging6.Invoke(new EventHandler(delegate { butCompletedLogging6.Text = Convert.ToBoolean(receivedData).ToString().ToUpper(); }));
                    SetButtonStatusColor(butCompletedLogging6, Convert.ToBoolean(receivedData));
                }

                //Machine 7
                if (values[i].ItemName == tagMainBlock + "." + "ST5_2StatusMc")
                {
                    int receivedData = (Int16)values[i].Value;
                    butStatusMc7.Invoke(new EventHandler(delegate { butStatusMc7.Text = Convert.ToBoolean(receivedData) ? "ONLINE" : "OFFLINE"; }));
                    SetButtonStatusColor(butStatusMc7, Convert.ToBoolean(receivedData));
                }
                if (values[i].ItemName == tagMainBlock + "." + "ST5_2ReqLogging")
                {
                    int receivedData = (Int16)values[i].Value;
                    butRequestLogging7.Invoke(new EventHandler(delegate { butRequestLogging7.Text = Convert.ToBoolean(receivedData).ToString().ToUpper(); }));
                    SetButtonStatusColor(butRequestLogging7, Convert.ToBoolean(receivedData));
                }
                if (values[i].ItemName == tagMainBlock + "." + "ST5_2LoggingApp")
                {
                    int receivedData = (Int16)values[i].Value;
                    butCompletedLogging7.Invoke(new EventHandler(delegate { butCompletedLogging7.Text = Convert.ToBoolean(receivedData).ToString().ToUpper(); }));
                    SetButtonStatusColor(butCompletedLogging7, Convert.ToBoolean(receivedData));
                }
            }
        }

        private void SetButtonStatusColor(Button butStatus, bool v)
        {
            if (v)
            {
                butStatus.BackColor = Color.GreenYellow;
            }
            else
            {
                butStatus.BackColor = Color.Gray;
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
    }
}
