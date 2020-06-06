using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trace.Domain.Models
{
    public class PartAssemblyModel : DomainObject
    {
        public string PartName { get; set; }
        public string SerialNumber { get; set; }
        public int TraceabilityLogId { get; set; }

        public TraceabilityLogModel TraceabilityLog { get; set; }
    }
}
