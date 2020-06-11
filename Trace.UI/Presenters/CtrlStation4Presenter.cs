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
    public class CtrlStation4Presenter
    {
        IDataService<MachineModel> _serviceMachine = new MachineService(new TraceDbContextFactory());
        IDataService<PlcTagModel> _servicePLCTag = new PLCTagService(new TraceDbContextFactory());
        IDataService<TraceabilityLogModel> _serviceTraceLog = new TraceabilityLogService(new TraceDbContextFactory());

        private readonly IStation4View _view;

        public CtrlStation4Presenter(IStation4View view)
        {
            _view = view;

            _view.ControlLoad += InitailizeControl;
            _view.MonitoringRailTime += MonitoringRailTimeAsync;
        }

        private async void MonitoringRailTimeAsync(object sender, EventArgs e)
        {
            var result = await _serviceTraceLog.GetListByMachineID(5, 1);

            TraceabilityLogModel log = result.FirstOrDefault();

            if (log != null)
            {
                _view.traceabilityLog = await _serviceTraceLog.GetByID(log.Id);
            }
        }

        private void InitailizeControl(object sender, EventArgs e)
        {
            _view.EnableTimer(_view.MonitorFlag);
        }
    }
}
