using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trace.Domain.Models;

namespace Trace.UI.Views
{
    public interface IStation3View
    {
        event EventHandler ControlLoad;
        event EventHandler MonitoringRailTime;

        bool MonitorFlag { get; set; }

        TraceabilityLogModel traceabilityUpperLog { get; set; }
        TraceabilityLogModel traceabilityLowerLog { get; set; }

        void EnableTimer(bool enable);
    }
}
