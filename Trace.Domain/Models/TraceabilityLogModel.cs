using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trace.Domain.Models
{
    public class TraceabilityLogModel : DomainObject
    {
        public int StationId { get; set; }
        public int MachineId { get; set; }
        public string ItemCode { get; set; }
        public string ModelRunning { get; set; }
        public string Description { get; set; }
        public string PartSerialNumber { get; set; }
        public string Actuator { get; set; }
        public DateTime ProductionDate { get; set; }
        public string SwNumber { get; set; }
        public string CurrentMaximum { get; set; }
        public string OpenAngle { get; set; }
        public string LineErrorCounter { get; set; }
        public string Attribute1 { get; set; }        
        public string Attribute2 { get; set; }
        public string Attribute3 { get; set; }
        public string Attribute4 { get; set; }
        public string Attribute5 { get; set; }
        public string Attribute6 { get; set; }
        public string Attribute7 { get; set; }
        public string Attribute8 { get; set; }
        public string Attribute9 { get; set; }
        public string Attribute10 { get; set; }
        public int MaxRepairTime { get; set; }
        public bool FinalResult { get; set; }
        public string ImagePath { get; set; }
        public string QRCodePath { get; set; }
        public int RepairTime { get; set; }

        public StationModel Station { get; set; }
        public MachineModel Machine { get; set; }
        public List<PartAssemblyModel> PartAssemblies { get; set; }
        public List<TighteningResultModel> TighteningResults { get; set; }
        public List<CameraResultModel> CameraResults { get; set; }
    }
}