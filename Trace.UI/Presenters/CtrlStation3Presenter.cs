using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trace.Data;
using Trace.Data.Service;
using Trace.Domain.Models;
using Trace.Domain.Services;
using Trace.UI.Views;

namespace Trace.UI.Presenters
{
    public class CtrlStation3Presenter
    {
        IDataService<MachineModel> _serviceMachine = new MachineService(new TraceDbContextFactory());
        IDataService<PlcTagModel> _servicePLCTag = new PLCTagService(new TraceDbContextFactory());
        IDataService<TraceabilityLogModel> _serviceTraceLog = new TraceabilityLogService(new TraceDbContextFactory());

        private readonly IStation3View _view;

        public CtrlStation3Presenter(IStation3View view)
        {
            _view = view;

            _view.ControlLoad += InitailizeControl;
            _view.MonitoringRailTime += MonitoringRailTimeAsync;
        }

        private async void MonitoringRailTimeAsync(object sender, EventArgs e)
        {
            var result1 = await _serviceTraceLog.GetListByMachineID(3, 1);
            var result2 = await _serviceTraceLog.GetListByMachineID(4, 1);

            TraceabilityLogModel log1 = result1.Where(x => x.CreationDate.Date == DateTime.Now.Date).FirstOrDefault();
            TraceabilityLogModel log2 = result2.Where(x => x.CreationDate.Date == DateTime.Now.Date).FirstOrDefault();

            if (log1 != null)
            {
                _view.traceabilityUpperLog = await _serviceTraceLog.GetByID(log1.Id);                
            }

            if(log2 != null)
            {
                _view.traceabilityLowerLog = await _serviceTraceLog.GetByID(log2.Id);
            }
        }

        private void InitailizeControl(object sender, EventArgs e)
        {
            _view.EnableTimer(_view.MonitorFlag);
        }
    }
}
