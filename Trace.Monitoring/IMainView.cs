using Opc.Da;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trace.Domain.Models;

namespace Trace.Monitoring
{
    public interface IMainView
    {
        event EventHandler FormLoad;
        event EventHandler Connect_Click;
        event EventHandler Disconnect_Click;

        event EventHandler InterLock;
        event EventHandler MakeReady;
        event EventHandler KeepLogging;
        event EventHandler CompleteAction;
        event EventHandler VerityCode;
        event EventHandler VerityActuater;
        event EventHandler RefreshData;

        Server daServer { get; set; }
        Subscription groupRead { get; set; }
        SubscriptionState groupStateRead { get; set; }

        Subscription groupWrite { get; set; }
        SubscriptionState groupStateWrite { get; set; }

        Subscription groupWriteMc1 { get; set; }
        SubscriptionState groupStateWriteMc1 { get; set; }

        Subscription groupWriteMc2 { get; set; }
        SubscriptionState groupStateWriteMc2 { get; set; }

        Subscription groupWriteMc3 { get; set; }
        SubscriptionState groupStateWriteMc3 { get; set; }

        Subscription groupWriteMc4 { get; set; }
        SubscriptionState groupStateWriteMc4 { get; set; }

        Subscription groupWriteMc5 { get; set; }
        SubscriptionState groupStateWriteMc5 { get; set; }

        Subscription groupWriteMc6 { get; set; }
        SubscriptionState groupStateWriteMc6 { get; set; }

        Subscription groupWriteMc7 { get; set; }
        SubscriptionState groupStateWriteMc7 { get; set; }


        //initialization of the sample object that contains opc values
        OPCObject myOpcObject { get; set; }
        Item[] items { get; set; }

        List<PlcTagModel> plcTags { get; set; }

        MachineModel machine1 { get; set; }
        MachineModel machine2 { get; set; }
        MachineModel machine3 { get; set; }
        MachineModel machine4 { get; set; }
        MachineModel machine5 { get; set; }
        MachineModel machine6 { get; set; }
        MachineModel machine7 { get; set; }

        bool connectedPlc { get; set; }
        bool systemReady { get; set; }
        string tagMainBlock { get; set; }
        string serverUrl { get; set; }
        string tagClockReady { get; set; }
        string tagTraceabilityReady { get; set; }

        string strConnectionMessage { get; set; }

        void group_DataChanged(object subscriptionHandle, object requestHandle, ItemValueResult[] values);

        void EnableClock();
        void DisableClock();
    }
}
