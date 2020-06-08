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

        Server daServer { get; set; }
        Subscription groupRead { get; set; }
        SubscriptionState groupStateRead { get; set; }

        Subscription groupWrite { get; set; }
        SubscriptionState groupStateWrite { get; set; }
        

        //initialization of the sample object that contains opc values
        OPCObject myOpcObject { get; set; }
        Item[] items { get; set; }

        List<PlcTagModel> plcTags { get; set; }

        bool connectedPlc { get; set; }
        string tagMainBlock { get; set; }
        string serverUrl { get; set; }
        string tagClockReady { get; set; }
        string tagTraceabilityReady { get; set; }

        void group_DataChanged(object subscriptionHandle, object requestHandle, ItemValueResult[] values);
    }
}
