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
        event EventHandler MonitoringTag;

        event EventHandler ReadTag;
        event EventHandler WriteTag;
        string TagValue { get; set; }      

        Server daServer { get; set; }
        Subscription groupRead { get; set; }
        SubscriptionState groupStateRead { get; set; }

        Subscription groupReadMachine1 { get; set; }
        SubscriptionState groupStateReadMachine1 { get; set; }

        Subscription groupReadMachine2 { get; set; }
        SubscriptionState groupStateReadMachine2 { get; set; }

        Subscription groupReadMachine3 { get; set; }
        SubscriptionState groupStateReadMachine3 { get; set; }

        Subscription groupReadMachine4 { get; set; }
        SubscriptionState groupStateReadMachine4 { get; set; }

        Subscription groupReadMachine5 { get; set; }
        SubscriptionState groupStateReadMachine5 { get; set; }

        Subscription groupReadMachine6 { get; set; }
        SubscriptionState groupStateReadMachine6 { get; set; }

        Subscription groupReadMachine7 { get; set; }
        SubscriptionState groupStateReadMachine7 { get; set; }

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
        Item[] items1 { get; set; }
        Item[] items2 { get; set; }
        Item[] items3 { get; set; }
        Item[] items4 { get; set; }
        Item[] items5 { get; set; }
        Item[] items6 { get; set; }
        Item[] items7 { get; set; }


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

        void group_DataChanged1(object subscriptionHandle, object requestHandle, ItemValueResult[] values);
        void group_DataChanged2(object subscriptionHandle, object requestHandle, ItemValueResult[] values);
        void group_DataChanged3(object subscriptionHandle, object requestHandle, ItemValueResult[] values);
        void group_DataChanged4(object subscriptionHandle, object requestHandle, ItemValueResult[] values);
        void group_DataChanged5(object subscriptionHandle, object requestHandle, ItemValueResult[] values);
        void group_DataChanged6(object subscriptionHandle, object requestHandle, ItemValueResult[] values);
        void group_DataChanged7(object subscriptionHandle, object requestHandle, ItemValueResult[] values);

        void EnableClock();
        void DisableClock();
    }
}
