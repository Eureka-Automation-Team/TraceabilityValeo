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
            set { _machine = value; }
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
        public string strConnectionMessage
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
            throw new NotImplementedException();
        }

        private void MonitoringForm_Load(object sender, EventArgs e)
        {
            if (FormLoad != null)
                FormLoad(sender, e);

            timerConnect.Enabled = true;
            _connectedPlc = false;
            DisableClock();
        }
    }
}
