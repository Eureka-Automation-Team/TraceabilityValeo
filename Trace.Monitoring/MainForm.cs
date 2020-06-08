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

namespace Trace.Monitoring
{
    public partial class MainForm : Form, IMainView
    {
        private Server _daServer = null;
        private Subscription _groupRead;        
        private SubscriptionState _groupStateRead;

        private Subscription _groupWrite;
        private SubscriptionState _groupStateWrite;

        private Item[] _items = new Item[1];

        //initialization of the sample object that contains opc values
        OPCObject _myOpcObject = new OPCObject();

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

        public event EventHandler FormLoad;
        public event EventHandler Connect_Click;
        public event EventHandler Disconnect_Click;

        public MainForm()
        {
            InitializeComponent();
        }

        public void Connect(string serverName)
        {
            /* When the factory creates an HDA server, it passes along 2 parameters:
             *    SerializationInfo info
             *    StreamingContext context
             *
             * The Factory class casts the COM object (pointing to the HDA server) to the IServer interface.
             * All calls to the interface or proxied to the COM object.
             */
            Opc.URL url = new Opc.URL(serverName);
            OpcCom.Factory fact = new OpcCom.Factory();
            _daServer = new Server(fact, url);

            try
            {
                //_hdaServer.Connect();
                _daServer.Connect(url, new Opc.ConnectData(new System.Net.NetworkCredential()));
                Console.WriteLine(String.Format("Connect to server {0}", serverName));

                //3rd Create a group if items            
                _groupStateRead = new SubscriptionState();
                _groupStateRead.Name = "Group";
                _groupStateRead.UpdateRate = 1000;// this isthe time between every reads from OPC server
                _groupStateRead.Active = true;//this must be true if you the group has to read value
                _groupRead = (Opc.Da.Subscription)_daServer.CreateSubscription(_groupStateRead);
                _groupRead.DataChanged += new Opc.Da.DataChangedEventHandler(group_DataChanged);//callback when the data are readed                            

                // add items to the group    (in Rockwell names are identified like [Name of PLC in the server]Block of word:number of word,number of consecutive readed words)        
                _items[0] = new Item();
                _items[0].ItemName = "[TRACEABILITY]Program:Traceability_System.ST1LoggingApp";//this reads 2 word (short - 16 bit)
                _items = _groupRead.AddItems(_items);

                _groupStateWrite = new SubscriptionState();
                _groupStateWrite.Name = "Group Write";
                _groupStateWrite.Active = false;//not needed to read if you want to write only
                _groupWrite = (Opc.Da.Subscription)_daServer.CreateSubscription(_groupStateWrite);

            }
            catch (Opc.ConnectFailedException opcConnExc)
            {
                Console.WriteLine(String.Format("Could not connect to server {0}", serverName));
                Console.WriteLine(opcConnExc.ToString());
            }

            Console.WriteLine("Are we connected? " + _daServer.IsConnected);
        }

        private void group_DataChanged(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                int receivedData = (Int16)values[i].Value;
                if (values[i].ItemName == "[TRACEABILITY]Program:Traceability_System.ST1LoggingApp")
                {
                    //myOpcObject.DataN7 = receivedData;
                    //remember that it's in another thread (so if you want to update the UI you should use anonyms methods)
                    textBox1.Invoke(new EventHandler(delegate { textBox1.Text = receivedData.ToString(); }));
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

        private void button1_Click(object sender, EventArgs e)
        {
            Connect(txtServerUrl.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WriteWord();
        }
    }
}
