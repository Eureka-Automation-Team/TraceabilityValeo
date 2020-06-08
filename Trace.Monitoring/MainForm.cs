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

        public event EventHandler FormLoad;
        public event EventHandler Connect_Click;
        public event EventHandler Disconnect_Click;
        public event EventHandler InterLock;
        public event EventHandler MakeReady;

        public MainForm()
        {
            InitializeComponent();

            this._presenter = new MainPresenter(this);
        }


        public void group_DataChanged(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                int receivedData = (Int16)values[i].Value;
                if (values[i].ItemName == this._tagTraceabilityReady)
                {
                    butMakeReady.Invoke(new EventHandler(delegate { this.systemReady = Convert.ToBoolean(receivedData); }));
                }
                if(values[i].ItemName == "[TRACEABILITY]Program:Traceability_System.ST1StatusMc")
                {
                    butStatusMc1.Invoke(new EventHandler(delegate { this.butStatusMc1.Text = Convert.ToBoolean(receivedData).ToString(); }));
                }

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
