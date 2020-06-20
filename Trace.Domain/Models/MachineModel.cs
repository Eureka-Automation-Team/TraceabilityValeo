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
            get { return (OnlineFlag == 0) ? "OFFLINE" : (OnlineFlag == 1 ? "ONLINE" : "ALARM"); } 
        }

        public bool RequestLogging { get; set; }
        public int CompletedLogging { get; set; }
        public bool RequestVerifyCode { get; set; }
        public string CodeVerify { get; set; }
        public int CodeVerifyResult { get; set; }

        public StationModel Station { get; set; }
    }
}
