﻿using Opc.Da;
using System;
using System.Collections.Generic;
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
        }

        private void Connect(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_view.serverUrl))
            {
                Connect(_view.serverUrl);
            }
        }

        private void FormLoad(object sender, EventArgs e)
        {
            _view.serverUrl = "opcda://localhost/RSLinx OPC Server";
        }

        private void Connect(string serverName)
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
                Console.WriteLine(String.Format("Connect to server {0}", serverName));

                //3rd Create a group if items            
                _view.groupStateRead = new SubscriptionState();
                _view.groupStateRead.Name = "Group";
                _view.groupStateRead.UpdateRate = 1000;// this isthe time between every reads from OPC server
                _view.groupStateRead.Active = true;//this must be true if you the group has to read value
                _view.groupRead = (Opc.Da.Subscription)_view.daServer.CreateSubscription(_view.groupStateRead);
                _view.groupRead.DataChanged += new Opc.Da.DataChangedEventHandler(group_DataChanged);//callback when the data are readed                            

                // add items to the group    (in Rockwell names are identified like [Name of PLC in the server]Block of word:number of word,number of consecutive readed words)        
                _view.items[0] = new Item();
                _view.items[0].ItemName = "[TRACEABILITY]Program:Traceability_System.ST1LoggingApp";//this reads 2 word (short - 16 bit)
                _view.items = _view.groupRead.AddItems(_view.items);

                _view.groupStateWrite = new SubscriptionState();
                _view.groupStateWrite.Name = "Group Write";
                _view.groupStateWrite.Active = false;//not needed to read if you want to write only
                _view.groupWrite = (Opc.Da.Subscription)_view.daServer.CreateSubscription(_view.groupStateWrite);

            }
            catch (Opc.ConnectFailedException opcConnExc)
            {
                //Console.WriteLine(String.Format("Could not connect to server {0}", serverName));
                //Console.WriteLine(opcConnExc.ToString());
                MessageBox.Show(String.Format("Could not connect to server {0}{1}{2}"
                                            , serverName
                                            , Environment.NewLine
                                            , opcConnExc.ToString())
                                            , "Connection failed!"
                                            , MessageBoxButtons.OK
                                            , MessageBoxIcon.Error);
            }

            //Console.WriteLine("Are we connected? " + _view.daServer.IsConnected);
        }

        private void group_DataChanged(object subscriptionHandle, object requestHandle, ItemValueResult[] values)
        {
            //throw new NotImplementedException();
        }
    }
}
