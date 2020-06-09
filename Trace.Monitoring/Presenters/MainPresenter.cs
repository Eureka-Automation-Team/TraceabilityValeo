using Opc.Da;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trace.Data;
using Trace.Data.Service;
using Trace.Domain.Models;
using Trace.Domain.Services;

namespace Trace.Monitoring.Presenters
{
    public class MainPresenter
    {
        IDataService<MachineModel> _serviceMachine = new MachineService(new TraceDbContextFactory());
        IDataService<PlcTagModel> _servicePLCTag = new PLCTagService(new TraceDbContextFactory());

        private readonly IMainView _view;

        public MainPresenter(IMainView view)
        {
            _view = view;

            _view.FormLoad += FormLoad;
            _view.Connect_Click += Connect;
            _view.Disconnect_Click += Disconnect_Click;
            _view.InterLock += InterLock;
            _view.MakeReady += MakeReady;
            _view.KeepLogging += KeepLogging;
        }

        private void KeepLogging(object sender, EventArgs e)
        {
            MachineModel machine = (MachineModel)sender;
            var result = _view.groupRead.Read(_view.groupRead.Items).ToList();
            
            var machineTags = _servicePLCTag.GetAll().Result.ToList().Where(x => x.MachineId == machine.Id);
            var tags = (from tag in machineTags
                       where tag.MachineId == machine.Id
                       select new { Tag = _view.tagMainBlock + "." + tag.PlcTag }).ToArray();

            var r = result.Where(x => tags.Any(s => s.Tag == x.ItemName));
        }

        private void Disconnect_Click(object sender, EventArgs e)
        {
            _view.connectedPlc = !Disconnect();
        }

        private void MakeReady(object sender, EventArgs e)
        {
            if (_view.systemReady)
            {
                if (WriteWord(_view.tagTraceabilityReady, 0))
                    _view.systemReady = false;
            }
            else
            {
                if (WriteWord(_view.tagTraceabilityReady, 1))
                    _view.systemReady = true;
            }
        }

        private void InterLock(object sender, EventArgs e)
        {
            var result = WriteWord(_view.tagClockReady, 1);

            if (!result)
            {
                _view.connectedPlc = false;
                _view.systemReady = false;
            }
        }

        private void Connect(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_view.serverUrl))
            {
                _view.connectedPlc = Connect(_view.serverUrl);
            }
        }

        private bool Disconnect()
        {
            if (_view.daServer != null && _view.daServer.IsConnected)
                _view.daServer.Disconnect();

            return true;
        }

        [Obsolete]
        private async void FormLoad(object sender, EventArgs e)
        {

            _view.serverUrl = ConfigurationSettings.AppSettings["DefaultUrl"].ToString();
            _view.tagMainBlock = ConfigurationSettings.AppSettings["MainBlock"].ToString();

            var machines = await _serviceMachine.GetAll();
            int i = 1;
            foreach(var mac in machines.ToList().OrderBy(o => o.Id))
            {
                if (i == 1)
                    _view.machine1 = mac;
                if (i == 2)
                    _view.machine2 = mac;
                if (i == 3)
                    _view.machine3 = mac;
                if (i == 4)
                    _view.machine4 = mac;
                if (i == 5)
                    _view.machine5 = mac;
                if (i == 6)
                    _view.machine6 = mac;
                if (i == 7)
                    _view.machine7 = mac;

                i++;
            }

            var result = await _servicePLCTag.GetAll();
            _view.plcTags = result.ToList();

            if(_view.plcTags.Count > 0)
            {
                string clockTag = _view.plcTags.Where(x => x.TypeCode == "INTER_LOCK").FirstOrDefault().PlcTag;
                string readyTag = _view.plcTags.Where(x => x.TypeCode == "SYSTEM_READY").FirstOrDefault().PlcTag;
                _view.tagClockReady = _view.tagMainBlock + "." + clockTag;
                _view.tagTraceabilityReady = _view.tagMainBlock + "." + readyTag;
            }

        }

        private bool Connect(string serverName)
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
            _view.daServer = new Server(fact, url);

            try
            {
                //_hdaServer.Connect();
                _view.daServer.Connect(url, new Opc.ConnectData(new System.Net.NetworkCredential()));
                //Console.WriteLine(String.Format("Connect to server {0}", serverName));

                //3rd Create a group if items            
                _view.groupStateRead = new SubscriptionState();
                _view.groupStateRead.Name = "InterLockGroup";
                _view.groupStateRead.UpdateRate = 1000;// this isthe time between every reads from OPC server
                _view.groupStateRead.Active = true;//this must be true if you the group has to read value
                _view.groupRead = (Subscription)_view.daServer.CreateSubscription(_view.groupStateRead);
                _view.groupRead.DataChanged += new DataChangedEventHandler(_view.group_DataChanged);//callback when the data are readed                            

                // add items to the group    (in Rockwell names are identified like [Name of PLC in the server]Block of word:number of word,number of consecutive readed words)   
                if (_view.groupRead.Items != null)
                    _view.groupRead.RemoveItems(_view.groupRead.Items);

                _view.items = new Item[_view.plcTags.Count];
                int i = 0;
                foreach(var tag in _view.plcTags)
                {
                    _view.items[i] = new Item();
                    _view.items[i].ItemName = _view.tagMainBlock + "." + tag.PlcTag;

                    i++;
                }                
                _view.items = _view.groupRead.AddItems(_view.items);

                _view.groupStateWrite = new SubscriptionState();
                _view.groupStateWrite.Name = "Group Write";
                _view.groupStateWrite.Active = false;//not needed to read if you want to write only
                _view.groupWrite = (Subscription)_view.daServer.CreateSubscription(_view.groupStateWrite);

                return true;
            }
            catch (Opc.ConnectFailedException opcConnExc)
            {
                MessageBox.Show(String.Format("Could not connect to server {0}{1}{2}"
                                            , serverName
                                            , Environment.NewLine
                                            , opcConnExc.ToString())
                                            , "Connection failed!"
                                            , MessageBoxButtons.OK
                                            , MessageBoxIcon.Error);
                return false;
            }           
        }

        private bool WriteWord(string tag, int value)
        {
            //Create the item to write (if the group doesn't have it, we need to insert it)
            Item[] itemToAdd = new Item[1];
            itemToAdd[0] = new Item();
            itemToAdd[0].ItemName = tag;

            //create the item that contains the value to write
            ItemValue[] writeValues = new Opc.Da.ItemValue[1];
            writeValues[0] = new ItemValue(itemToAdd[0]);

            //make a scan of group to see if it already contains the item
            bool itemFound = false;
            foreach (Item item in _view.groupWrite.Items)
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
                _view.groupWrite.AddItems(itemToAdd);
                writeValues[0].ServerHandle = _view.groupWrite.Items[_view.groupWrite.Items.Length - 1].ServerHandle;
            }
            //set the value to write
            writeValues[0].Value = value;
            //write

            try
            {
                _view.groupWrite.Write(writeValues);
                return true;
            }catch(Exception ex)
            {
                return false;
            }
            
        }

    }
}
