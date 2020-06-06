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

        //private Opc.Da.Subscription groupWrite;
        //private Opc.Da.SubscriptionState groupStateWrite;

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
                items[0].ItemName = "[TEST]Program:Traceability_System.Request_to_PC_Read_DATA";//this reads 2 word (short - 16 bit)
                items = groupRead.AddItems(items);
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
                short[] receivedData = (short[])values[i].Value;
                if (values[i].ItemName == "[TEST]Program:Traceability_System.Request_to_PC_Read_DATA")
                {
                    myOpcObject.DataN7 = receivedData;
                    //remember that it's in another thread (so if you want to update the UI you should use anonyms methods)
                    textBox1.Invoke(new EventHandler(delegate { textBox1.Text = myOpcObject.DataN7[0].ToString(); }));
                }
                else if (values[i].ItemName == "[UNTITLED_1]B10:0")
                {
                    myOpcObject.BitsB10 = receivedData;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Connect("RSLinx OPC Server");
        }
    }
}
