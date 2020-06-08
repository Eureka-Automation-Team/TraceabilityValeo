﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trace.Domain.Models
{
    public class PlcTagModel : DomainObject
    {
        public string PlcTag { get; set; }
        public string Description { get; set; }
        public bool ReadAbleFlag { get; set; }
        public bool WriteAbleFlag { get; set; }        
        public string DataType { get; set; }
        public string TypeCode { get; set; }
        public int MachineId { get; set; }
        public int StationId { get; set; }

        public MachineModel Machine { get; set; }
        public StationModel Station { get; set; }
    }
}