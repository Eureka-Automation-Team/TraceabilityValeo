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
    public class CtrlStation5Presenter
    {
        IDataService<MachineModel> _serviceMachine = new MachineService(new TraceDbContextFactory());
        IDataService<PlcTagModel> _servicePLCTag = new PLCTagService(new TraceDbContextFactory());
        IDataService<TraceabilityLogModel> _serviceTraceLog = new TraceabilityLogService(new TraceDbContextFactory());

        private readonly IStation5View _view;

        public CtrlStation5Presenter(IStation5View view)
        {
            _view = view;

            _view.ControlLoad += InitailizeControl;
            _view.MonitoringRailTime += MonitoringRailTimeAsync;
        }

        private void MonitoringRailTimeAsync(object sender, EventArgs e)
        {
            var result1 = _serviceTraceLog.GetListByMachineID(6, 1);
            var result2 = _serviceTraceLog.GetListByMachineID(7, 1);

            TraceabilityLogModel log1 = result1.Where(x => x.CreationDate.Date == DateTime.Now.Date).FirstOrDefault();
            TraceabilityLogModel log2 = result2.Where(x => x.CreationDate.Date == DateTime.Now.Date).FirstOrDefault();

            //TraceabilityLogModel log1 = result1.FirstOrDefault();
            //TraceabilityLogModel log2 = result2.FirstOrDefault();

            if (log1 != null)
            {
                _view.traceabilityUpperLog = _serviceTraceLog.GetByID(log1.Id);
                _view.CountQtyUpper = _serviceTraceLog.GetListByMachineID(6, 500).Where(x => x.CreationDate.Date == DateTime.Now.Date).Count();
            }
            if(log2 != null)
            {
                _view.traceabilityLowerLog = _serviceTraceLog.GetByID(log2.Id);
                _view.CountQtyLower = _serviceTraceLog.GetListByMachineID(7, 500).Where(x => x.CreationDate.Date == DateTime.Now.Date).Count();
            }
        }

        private void InitailizeControl(object sender, EventArgs e)
        {
            _view.EnableTimer(_view.MonitorFlag);

            if (!_view.MonitorFlag)
                _view.GenerateQrCode();
        }
    }
}
