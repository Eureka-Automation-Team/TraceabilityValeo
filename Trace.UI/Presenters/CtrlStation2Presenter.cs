using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trace.Data;
using Trace.Data.Service;
using Trace.Domain.Models;
using Trace.Domain.Services;
using Trace.UI.UIs;
using Trace.UI.Views;

namespace Trace.UI.Presenters
{
    public class CtrlStation2Presenter
    {
        IDataService<MachineModel> _serviceMachine = new MachineService(new TraceDbContextFactory());
        IDataService<PlcTagModel> _servicePLCTag = new PLCTagService(new TraceDbContextFactory());
        IDataService<TraceabilityLogModel> _serviceTraceLog = new TraceabilityLogService(new TraceDbContextFactory());
        IDataService<TighteningRepairModel> _serviceTigtheningRepair = new TighteningRepairService(new TraceDbContextFactory());

        private readonly IStation2View _view;

        public CtrlStation2Presenter(IStation2View view)
        {
            _view = view;

            _view.ControlLoad += InitailizeControl;
            _view.MonitoringRailTime += MonitoringRailTimeAsync;
            _view.ShowTighteningRepairs += ShowTighteningRepairs;
        }

        private void ShowTighteningRepairs(object sender, EventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            TighteningResultModel current = (TighteningResultModel)grid.CurrentRow.DataBoundItem;

            if (current != null)
            {
                TighteningRepairModel repair = new TighteningRepairModel();
                repair.TighteningResultId = current.Id;
                var logsRepair = _serviceTigtheningRepair.GetByPrimary(repair);

                using (TigtheningRepairsForm frm = new TigtheningRepairsForm())
                {
                    frm.DataBinding.DataSource = logsRepair.ToList();
                    frm.ShowDialog();
                }
            }
        }

        private void MonitoringRailTimeAsync(object sender, EventArgs e)
        {
            var result = _serviceTraceLog.GetListByMachineID(2, 1);

            TraceabilityLogModel log = result.Where(x => x.CreationDate.Date == DateTime.Now.Date).FirstOrDefault();
            //TraceabilityLogModel log = result.FirstOrDefault();
            if (log != null)
            {
                _view.traceabilityLog = _serviceTraceLog.GetByID(log.Id);
            }
        }

        private void InitailizeControl(object sender, EventArgs e)
        {
            _view.EnableTimer(_view.MonitorFlag);
        }
    }
}
