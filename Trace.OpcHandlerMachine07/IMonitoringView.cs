﻿using Opc.Da;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trace.Domain.Models;

namespace Trace.OpcHandlerMachine07
{
    public interface IMonitoringView
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

        List<PlcTagModel> plcTags { get; set; }
        Item[] items { get; set; }

        MachineModel machine { get; set; }

        bool connectedPlc { get; set; }
        bool systemReady { get; set; }
        string tagMainBlock { get; set; }
        string serverUrl { get; set; }
        string tagClockReady { get; set; }
        string tagTraceabilityReady { get; set; }

        string ResultnMessage { get; set; }

        /*---- Start Code Migration ----*/
        event EventHandler ResetComplete;
        event EventHandler ResetVerify;
        event EventHandler ResetActuater;
        bool lockingAppFlag { get; set; }
        bool verifyResultFlag { get; set; }
        bool getActuaterFlag { get; set; }
        List<OPCVar> OPCEventVars { get; set; }
        List<OPCVar> OPCWriteVars { get; set; }
        OPCClient OPC { get; set; }
        void ComErrorMessage(string text);
        void CheckNotifications();
        /*---- End Code Migration ----*/

        void group_DataChanged(object subscriptionHandle, object requestHandle, ItemValueResult[] values);
        void EnableClock();
        void DisableClock();
    }
}
