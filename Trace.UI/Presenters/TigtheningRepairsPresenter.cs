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
    public class TigtheningRepairsPresenter
    {
        IDataService<TraceabilityLogModel> _serviceTraceLog = new TraceabilityLogService(new TraceDbContextFactory());
        IDataService<TighteningRepairModel> _serviceTigtheningRepair = new TighteningRepairService(new TraceDbContextFactory());

        private readonly ITigtheningRepairsView _view;

        public TigtheningRepairsPresenter(ITigtheningRepairsView view)
        {
            _view = view;

            _view.FormLoad += FormLoad;
        }

        private void FormLoad(object sender, EventArgs e)
        {
            
        }
    }
}
