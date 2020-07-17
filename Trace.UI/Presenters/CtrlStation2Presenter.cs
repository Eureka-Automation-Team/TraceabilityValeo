using PagedList;
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
    public class CtrlStation2Presenter
    {
        IDataService<MachineModel> _serviceMachine = new MachineService(new TraceDbContextFactory());
        IDataService<PlcTagModel> _servicePLCTag = new PLCTagService(new TraceDbContextFactory());
        IDataService<TraceabilityLogModel> _serviceTraceLog = new TraceabilityLogService(new TraceDbContextFactory());

        private readonly IStation2View _view;

        public CtrlStation2Presenter(IStation2View view)
        {
            _view = view;

            _view.ControlLoad += InitailizeControl;
            _view.MonitoringRailTime += MonitoringRailTimeAsync;
        }

        private async void MonitoringRailTimeAsync(object sender, EventArgs e)
        {
            var result = await _serviceTraceLog.GetListByMachineID(2, 1);

            TraceabilityLogModel log = result.Where(x => x.CreationDate.Date == DateTime.Now.Date).FirstOrDefault();

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
