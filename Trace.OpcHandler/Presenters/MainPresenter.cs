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

namespace Trace.OpcHandlerMachine01.Presenters
{

    public class MainPresenter
    {
        IDataService<MachineModel> _serviceMachine = new MachineService(new TraceDbContextFactory());
        IDataService<PlcTagModel> _servicePLCTag = new PLCTagService(new TraceDbContextFactory());
        IDataService<TraceabilityLogModel> _serviceTraceLog = new TraceabilityLogService(new TraceDbContextFactory());
        IDataService<TighteningResultModel> _serviceTigthening = new TighteningResultService(new TraceDbContextFactory());
        IDataService<TighteningRepairModel> _serviceTigtheningRepair = new TighteningRepairService(new TraceDbContextFactory());
        IDataService<CameraResultModel> _serviceCameraResult = new CameraResultService(new TraceDbContextFactory());
        IDataService<PartAssemblyModel> _servicePartAssembly = new PartAssemblyService(new TraceDbContextFactory());

        private readonly IMonitoringView _view;
        public MainPresenter(IMonitoringView view)
        {
            _view = view;

            _view.FormLoad += Initailization;
            _view.Connect_Click += Connect;
            _view.Disconnect_Click += Disconnect;
            _view.InterLock += InterLock;
        }

        private void InterLock(object sender, EventArgs e)
        {
            bool result = false;

            Task t = Task.Run(() =>
            {
                result = WriteWordInterLock(_view.tagClockReady, 1);
            });
            TimeSpan ts = TimeSpan.FromMilliseconds(2000);
            //if (!t.Wait(ts))
            //    Console.WriteLine("The timeout interval elapsed.");

            if (!t.Wait(ts))
            {
                _view.connectedPlc = false;
                _view.systemReady = false;
                Disconnect();
                _view.ResultnMessage = "Inter Lock failed!";
                //Application.Exit();
            }
            else
            {
                _view.ResultnMessage = "Inter Lock completed.";
            }
        }

        private bool WriteWordInterLock(string tag, int value)
        {
            //Create the item to write (if the group doesn't have it, we need to insert it)
            Item[] itemToAdd = new Item[1];
            itemToAdd[0] = new Item();
            itemToAdd[0].ItemName = tag;

            //create the item that contains the value to write
            ItemValue[] writeValues = new ItemValue[1];
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
            }
            catch
            {
                return false;
            }
        }

        private void Disconnect(object sender, EventArgs e)
        {
            _view.connectedPlc = !Disconnect();
        }

        private void Connect(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_view.serverUrl))
            {
                _view.connectedPlc = Connect(_view.serverUrl);

                //if (_view.connectedPlc)
                //    LoadCurrentValue(_view.groupRead);
            }
        }

        private bool Connect(string serverUrl)
        {
            /* When the factory creates an HDA server, it passes along 2 parameters:
             *    SerializationInfo info
             *    StreamingContext context
             *
             * The Factory class casts the COM object (pointing to the HDA server) to the IServer interface.
             * All calls to the interface or proxied to the COM object.
             */


            Opc.URL url = new Opc.URL(serverUrl);
            OpcCom.Factory fact = new OpcCom.Factory();
            _view.daServer = new Server(fact, url);

            try
            {
                //_hdaServer.Connect();
                try
                {
                    _view.daServer.Connect(url, new Opc.ConnectData(new System.Net.NetworkCredential()));
                }
                catch (Exception ex)
                {
                    _view.ResultnMessage = ex.Message;
                    return false;
                }
                //Console.WriteLine(String.Format("Connect to server {0}", serverName));

                //3rd Create a group if items  
                #region Group read interlock and Machine status
                _view.groupStateRead = new SubscriptionState();
                _view.groupStateRead.Name = "InterLockGroup";
                _view.groupStateRead.UpdateRate = 500;// this isthe time between every reads from OPC server
                _view.groupStateRead.Active = true;//this must be true if you the group has to read value
                _view.groupRead = (Subscription)_view.daServer.CreateSubscription(_view.groupStateRead);
                _view.groupRead.DataChanged += new DataChangedEventHandler(_view.group_DataChanged);//callback when the data are readed                            

                // add items to the group    (in Rockwell names are identified like [Name of PLC in the server]Block of word:number of word,number of consecutive readed words)   
                if (_view.groupRead.Items != null)
                    _view.groupRead.RemoveItems(_view.groupRead.Items);

                _view.items = new Item[_view.plcTags.Count];
                int i = 0;
                foreach (var tag in _view.plcTags)
                {
                    _view.items[i] = new Item();
                    _view.items[i].ItemName = _view.tagMainBlock + tag.PlcTag;
                    _view.items[i].Active = true;

                    i++;
                }
                _view.items = _view.groupRead.AddItems(_view.items);
                #endregion

                #region Group write
                _view.groupStateWrite = new SubscriptionState();
                _view.groupStateWrite.Name = "WriteInterLock";
                _view.groupStateWrite.Active = false;//not needed to read if you want to write only
                _view.groupWrite = (Subscription)_view.daServer.CreateSubscription(_view.groupStateWrite);
                #endregion

                return true;
            }
            catch (Opc.ConnectFailedException opcConnExc)
            {
                MessageBox.Show(String.Format("Could not connect to server {0}{1}{2}"
                                            , serverUrl
                                            , Environment.NewLine
                                            , opcConnExc.ToString())
                                            , "Connection failed!"
                                            , MessageBoxButtons.OK
                                            , MessageBoxIcon.Error);
                return false;
            }
        }

        private bool Disconnect()
        {
            if (_view.daServer != null && _view.daServer.IsConnected)
                _view.daServer.Disconnect();

            return true;
        }

        private void Initailization(object sender, EventArgs e)
        {
            int machineId = ConvertToInt(ConfigurationManager.AppSettings["MachineId"].ToString());
            _view.serverUrl = ConfigurationManager.AppSettings["DefaultUrl"].ToString();
            _view.tagMainBlock = ConfigurationManager.AppSettings["MainBlock"].ToString();

            var m = _serviceMachine.GetByID(1);
            if (m != null)
            {
                _view.machine = m;

                var result = _servicePLCTag.GetAll();
                _view.plcTags = result.Where(x => x.MachineId == machineId && x.MachineId == 99).ToList();

                if (_view.plcTags.Count > 0)
                {
                    string clockTag = _view.plcTags.Where(x => x.TypeCode == "INTER_LOCK").FirstOrDefault().PlcTag;
                    string readyTag = _view.plcTags.Where(x => x.TypeCode == "SYSTEM_READY").FirstOrDefault().PlcTag;
                    _view.tagClockReady = _view.tagMainBlock + clockTag;
                    _view.tagTraceabilityReady = _view.tagMainBlock + readyTag;
                }
            }
        }

        private void LoadCurrentValue(Subscription groupRead)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                var currentResult = _view.groupRead.Read(_view.groupRead.Items).ToList();

                //Machine 1
                var mac1 = _view.machine;
                var onlineTag = _view.tagMainBlock + "ST1StatusMc";
                mac1.OnlineFlag = Convert.ToInt32(currentResult.Where(x => x.ItemName == onlineTag).FirstOrDefault().Value);

                var reqLoggingTag = _view.tagMainBlock + "ST1ReqLogging";
                mac1.RequestLogging = Convert.ToBoolean(currentResult.Where(x => x.ItemName == reqLoggingTag).FirstOrDefault().Value);

                var completeLoggingTag = _view.tagMainBlock + "ST1LoggingApp";
                mac1.CompletedLogging = Convert.ToInt32(currentResult.Where(x => x.ItemName == completeLoggingTag).FirstOrDefault().Value);

                var reqVerifyTag = _view.tagMainBlock + "ST1ReqChkCodeVerify";
                mac1.RequestVerifyCode = Convert.ToBoolean(currentResult.Where(x => x.ItemName == reqVerifyTag).FirstOrDefault().Value);

                var verifyResultTag = _view.tagMainBlock + "ST1CodeVerifyResult";
                mac1.CodeVerifyResult = Convert.ToInt32(currentResult.Where(x => x.ItemName == verifyResultTag).FirstOrDefault().Value);

                int receivedData;
                var positionTag = _view.tagMainBlock + "ST1TestResult[19]";
                var succes = Int32.TryParse(currentResult.Where(x => x.ItemName == positionTag).FirstOrDefault().Value.ToString(), out receivedData);
                if (!succes)
                    receivedData = 0;
                mac1.TighteningPosition = receivedData;

                _view.machine = mac1;

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                _view.ResultnMessage = ex.Message;
                Cursor.Current = Cursors.Default;
                return;
            }
        }

        private decimal ConvertToDecimal(string number)
        {
            decimal myDecimal;
            if (string.IsNullOrEmpty(number))
                number = "0";

            bool result = decimal.TryParse(number, out myDecimal);

            if (result)
                return myDecimal;
            else
                return 0;
        }

        private int ConvertToInt(string number)
        {
            int myInt;
            if (string.IsNullOrEmpty(number))
                number = "0";

            bool result = int.TryParse(number, out myInt);

            if (result)
                return myInt;
            else
                return 0;
        }
    }
}
