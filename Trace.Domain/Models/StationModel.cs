using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trace.Domain.Models
{
    public class StationModel : DomainObject
    {
        public int StationNumber { get; set; }

        public List<MachineModel> MachineList { get; set; }
    }
}
