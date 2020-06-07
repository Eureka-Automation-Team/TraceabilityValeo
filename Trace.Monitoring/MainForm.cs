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
    public partial class MainForm : Form
    {
        private Server _hdaServer = null;
        private Subscription groupRead;        
        private SubscriptionState groupState;

        private Subscription groupWrite;
        private SubscriptionState groupStateWrite;

        private Item[] items = new Opc.Da.Item[1];

        //initialization of the sample object that contains opc values
        OPCObject myOpcObject = new OPCObject();

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
            Opc.URL url = new Opc.URL(String.Format("opcda://localhost/{0}", serverName));
            OpcCom.Factory fact = new OpcCom.Factory();
            _hdaServer = new Server(fact, url);

            try
            {
                //_hdaServer.Connect();
                _hdaServer.Connect(url, new Opc.ConnectData(new System.Net.NetworkCredential()));
                Console.WriteLine(String.Format("Connect to server {0}", serverName));

                //3rd Create a group if items            
                groupState = new SubscriptionState();
                groupState.Name = "Group";
                groupState.UpdateRate = 1000;// this isthe time between every reads from OPC server
                groupState.Active = true;//this must be true if you the group has to read value
                groupRead = (Opc.Da.Subscription)_hdaServer.CreateSubscription(groupState);
                groupRead.DataChanged += new Opc.Da.DataChangedEventHandler(group_DataChanged);//callback when the data are readed                            

                // add items to the group    (in Rockwell names are identified like [Name of PLC in the server]Block of word:number of word,number of consecutive readed words)        
                items[0] = new Item();
                items[0].ItemName = "[TRACEABILITY]Program:Traceability_System.ST1LoggingApp";//this reads 2 word (short - 16 bit)
                items = groupRead.AddItems(items);

                groupStateWrite = new SubscriptionState();
                groupStateWrite.Name = "Group Write";
                groupStateWrite.Active = false;//not needed to read if you want to write only
                groupWrite = (Opc.Da.Subscription)_hdaServer.CreateSubscription(groupStateWrite);

            }
            catch (Opc.ConnectFailedException opcConnExc)
            {
                Console.WriteLine(String.Format("Could not connect to server {0}", serverName));
                Console.WriteLine(opcConnExc.ToString());
            }

            Console.WriteLine("Are we connected? " + _hdaServer.IsConnected);
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
            //Connect("RSLinx OPC Server");
            string serverName = "RSLinx OPC Server";
     
            Connect(serverName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WriteWord();
        }
    }
}
