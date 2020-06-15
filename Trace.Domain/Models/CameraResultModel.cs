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
        public string TestResult { get; set; }
        public string TestResultDescription
        {
            get { return (TestResult); }
        }
        public int TraceLogId { get; set; }
        public TraceabilityLogModel TraceLog { get; set; }
    }
}
