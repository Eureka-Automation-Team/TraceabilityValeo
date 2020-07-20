using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trace.Domain.Models;

namespace Trace.UI.Views
{
    public interface IStation2View
    {
        event EventHandler ControlLoad;
        event EventHandler MonitoringRailTime;
        event EventHandler ShowTighteningRepairs;

        bool MonitorFlag { get; set; }

        TraceabilityLogModel traceabilityLog { get; set; }

        void EnableTimer(bool enable);
    }
}
