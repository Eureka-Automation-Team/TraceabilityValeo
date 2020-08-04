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
    public class SearchPresenter
    {
        IDataService<TraceabilityLogModel> _serviceTraceLog = new TraceabilityLogService(new TraceDbContextFactory());

        private readonly ISearchView _view;

        public SearchPresenter(ISearchView view)
        {
            _view = view;

            _view.Form_Load += InitailLoad;
            _view.Clear_Click += ClearForm;
            _view.Search_Click += Search;
            _view.Selected_Row += Selected;
        }

        private async void Selected(object sender, EventArgs e)
        {
            var current = (TraceabilityLogModel)_view.DataBinding.Current;

            if(current != null)
            {
                var logsRelate = await _serviceTraceLog.GetListByItemCode(current.ItemCode);

                using(SearchDetailForm frm = new SearchDetailForm())
                {
                    frm.logList = logsRelate.ToList();
                    frm.ShowDialog();
                }
            }
        }

        private async void Search(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            _view.DataBinding.DataSource = await GetLogsListAsync();
            Cursor.Current = Cursors.Default;
        }

        private void ClearForm(object sender, EventArgs e)
        {
            _view.itemCode = string.Empty;
            _view.partSerialNo = string.Empty;
            _view.startDate = DateTime.Now;
            _view.endDate = DateTime.Now;

            _view.DataBinding.DataSource = new List<TraceabilityLogModel>();
        }

        private void InitailLoad(object sender, EventArgs e)
        {
            ClearForm(null, null);
        }

        private async Task<IPagedList<TraceabilityLogModel>> GetLogsListAsync(int pageNumber = 1, int pageSize = 500)
        {
            return await Task.Factory.StartNew(() =>
            {
                var result = _serviceTraceLog.GetList("",500).Result.Where(x => x.CreationDate.Date >= _view.startDate.Date && x.CreationDate.Date <= _view.endDate.Date);

                if (!string.IsNullOrEmpty(_view.itemCode)) result = result.Where(x => x.ItemCode.ToUpper().Contains(_view.itemCode.ToUpper()));
                if (!string.IsNullOrEmpty(_view.partSerialNo)) result = result.Where(x => (string.IsNullOrEmpty(x.PartSerialNumber) ? "" : x.PartSerialNumber).ToUpper().Contains(_view.partSerialNo.ToUpper()));


                return result.ToPagedList(pageNumber, pageSize);
            });
        }
    }
}
