using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trace.Domain.Models
{
    public class TighteningResultModel : DomainObject
    {
        public string No { get; set; }
        public decimal Min { get; set; }
        public string MinDesc
        {
            get { return Min.ToString("#0.0") + "Nm"; }
        }
        public decimal Max { get; set; }
        public string MaxDesc
        {
            get { return Max.ToString("#0.0") + "Nm"; }
        }
        public decimal Target { get; set; }
        public string TargetDesc
        {
            get { return Target.ToString("#0.0") + "Nm"; }
        }
        public decimal Result { get; set; }
        public string ResultDesc
        {
            get { return Result.ToString("#0.0") + "Nm"; }
        }

        public string TestResult { get; set; }
        public string TestResultDescription
        {
            get { return TestResult; }
        }

        public int TraceLogId { get; set; }
        public TraceabilityLogModel TraceLog { get; set; }
    }
}
