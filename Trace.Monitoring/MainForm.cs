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

        private Item[] _items = new Item[1];

        //initialization of the sample object that contains opc values
        OPCObject _myOpcObject = new OPCObject();

        private List<PlcTagModel> _plcTags;
        private bool _connectedPlc;
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
                if (_connectedPlc)
                    butConnect.Text = "Disconnect";
                else
                    butConnect.Text = "Connect";
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
            set { _tagTraceabilityReady = value; }
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

        public event EventHandler FormLoad;
        public event EventHandler Connect_Click;
        public event EventHandler Disconnect_Click;

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
                    //myOpcObject.DataN7 = receivedData;
                    //remember that it's in another thread (so if you want to update the UI you should use anonyms methods)
                    txtTraceabilityRdy.Invoke(new EventHandler(delegate { txtTraceabilityRdy.Text = receivedData.ToString(); }));
                }
                //else if (values[i].ItemName == "[UNTITLED_1]B10:0")
                //{
                //    myOpcObject.BitsB10 = receivedData;
                //}
            }
        }

        private void WriteWord()
        {
            //Create the item to write (if the group doesn't have it, we need to insert it)
            Opc.Da.Item[] itemToAdd = new Opc.Da.Item[1];
            itemToAdd[0] = new Opc.Da.Item();
            itemToAdd[0].ItemName = "[TRACEABILITY]Program:Traceability_System.ST1LoggingApp";

            //create the item that contains the value to write
            Opc.Da.ItemValue[] writeValues = new Opc.Da.ItemValue[1];
            writeValues[0] = new Opc.Da.ItemValue(itemToAdd[0]);

            //make a scan of group to see if it already contains the item
            bool itemFound = false;
            foreach (Opc.Da.Item item in groupWrite.Items)
            {
                if (item.ItemName == itemToAdd[0].ItemName)
                {
                    // if it find the item i set the new value
                    writeValues[0].ServerHandle = item.ServerHandle;
                    itemFound = true;
                }
            }
            if (!itemFound)
            {
                //if it doesn't find it, we add it
                groupWrite.AddItems(itemToAdd);
                writeValues[0].ServerHandle = groupWrite.Items[groupWrite.Items.Length - 1].ServerHandle;
            }
            //set the value to write
            writeValues[0].Value = 1;
            //write
            groupWrite.Write(writeValues);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            WriteWord();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (FormLoad != null)
                FormLoad(sender, e);
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
    }
}
