using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trace.Domain.Models;

namespace Trace.UI.Views
{
    public interface ISearchDetailView
    {
        event EventHandler Form_Load;
        List<TraceabilityLogModel> logList { get; set; }
        void LoadDetail(TraceabilityLogModel log);
    }
}
