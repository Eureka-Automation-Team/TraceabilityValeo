using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trace.Domain.Models
{
    public class CameraResultModel : DomainObject
    {
        public string CameraName { get; set; }
        public bool TestResult { get; set; }
        public string TestResultDescription
        {
            get { return (TestResult) ? "OK" : "NOK"; }
        }
        public int TraceLogId { get; set; }
        public TraceabilityLogModel TraceLog { get; set; }
    }
}
