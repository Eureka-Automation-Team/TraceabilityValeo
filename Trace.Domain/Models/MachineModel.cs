using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trace.Domain.Models
{
    public class MachineModel : DomainObject
    {
        public string ManchineName { get; set; }
        public string ModelName { get; set; }
        public int StationId { get; set; }
        public bool OnlineFlag { get; set; }
        public string StatusName
        {
            get { return (OnlineFlag) ? "Online" : "Offline"; } 
        }

        public StationModel Station { get; set; }
    }
}
