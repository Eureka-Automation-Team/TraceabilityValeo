﻿using System;

namespace Trace.Domain.Models
{
    public class MachineModel : DomainObject
    {
        public string ManchineName { get; set; }
        public string ModelName { get; set; }
        public int StationId { get; set; }
        public int OnlineFlag { get; set; }
        public string StatusName
        {
            get { return (Enum.GetName(typeof(enumStatusMc), OnlineFlag)); }
        }

        public bool RequestLogging { get; set; }
        public int CompletedLogging { get; set; }
        public string CompletedLoggingDesc
        {
            get { return (Enum.GetName(typeof(enumCompletedLogging), CompletedLogging)); }
        }
        public bool RequestVerifyCode { get; set; }
        public string CodeVerify { get; set; }
        public int CodeVerifyResult { get; set; }
        public string MessageResult { get; set; }

        public StationModel Station { get; set; }
    }

    enum enumCompletedLogging
    {
        NONE = 0,
        COMPLETE = 1,
        DUPPLICATE = 2,
        DATA_INVALID = 3
    }

    enum enumStatusMc
    {
        OFFLINE = 0,
        ONLINE = 1,
        ALARM = 2
    }
}
