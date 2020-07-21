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

        //------------------------------------
        public decimal JointMin { get; set; }
        public string JointMinDesc
        {
            get { return JointMin.ToString("#0.0") + "Nm"; }
        }
        public decimal JointMax { get; set; }
        public string JointMaxDesc
        {
            get { return JointMax.ToString("#0.0") + "Nm"; }
        }
        public decimal JointTarget { get; set; }
        public string JointTargetDesc
        {
            get { return JointTarget.ToString("#0.0") + "Nm"; }
        }
        public decimal JointResult { get; set; }
        public string JointResultDesc
        {
            get { return JointResult.ToString("#0.0") + "Nm"; }
        }

        //------------------------------------

        public string TestResult { get; set; }
        public string JointTestResult { get; set; }
        public string TestResultDescription
        {
            get { return TestResult; }
        }

        public bool RepairFlag { get; set; }

        public int TraceLogId { get; set; }
        public TraceabilityLogModel TraceLog { get; set; }

        public List<TighteningRepairModel> TighteningRepairs { get; set; }
    }
}
