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
    public class MainPresenter
    {
        IDataService<MachineModel> _serviceMachine = new MachineService(new TraceDbContextFactory());
        IDataService<PlcTagModel> _servicePLCTag = new PLCTagService(new TraceDbContextFactory());
        IDataService<TraceabilityLogModel> _serviceTraceLog = new TraceabilityLogService(new TraceDbContextFactory());

        private readonly IMainView _view;

        public MainPresenter(IMainView view)
        {
            _view = view;

            _view.FormLoad += InitializeForm;
            _view.OpenForm += OpenForm;
        }

        private void OpenForm(object sender, EventArgs e)
        {
            Button but = sender as Button;

            if(but.Name == "butHome")
            {
                _view.LoadSubForm(new HomeForm());
            }
            else if (but.Name == "butSearchHistories")
            {
                _view.LoadSubForm(new SearchForm());
            }
        }

        private void InitializeForm(object sender, EventArgs e)
        {
            _view.HideSideBar();
            _view.LoadSubForm(new HomeForm());
        }
    }
}
