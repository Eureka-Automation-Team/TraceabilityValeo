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
    public class SearchDetailPresenter
    {
        IDataService<TraceabilityLogModel> _serviceTraceLog = new TraceabilityLogService(new TraceDbContextFactory());

        private readonly ISearchDetailView _view;

        public SearchDetailPresenter(ISearchDetailView view)
        {
            _view = view;

            _view.Form_Load += InitailLoad;
        }

        private void InitailLoad(object sender, EventArgs e)
        {
            foreach(var item in _view.logList.OrderByDescending(o => o.StationId))
            {
                var log =  _serviceTraceLog.GetByID(item.Id);
                _view.LoadDetail(log);
            }
        }
    }
}
