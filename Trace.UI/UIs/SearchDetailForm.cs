using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trace.Domain.Models;
using Trace.UI.Controls;
using Trace.UI.Presenters;
using Trace.UI.Views;

namespace Trace.UI.UIs
{
    public partial class SearchDetailForm : Form, ISearchDetailView
    {
        private readonly SearchDetailPresenter _presenter;
        private List<TraceabilityLogModel> _logList;
        public SearchDetailForm()
        {
            InitializeComponent();
            _presenter = new SearchDetailPresenter(this);
        }

        public List<TraceabilityLogModel> logList
        {
            get { return _logList; }
            set { _logList = value; }
        }

        public event EventHandler Form_Load;

        public void LoadDetail(TraceabilityLogModel log)
        {
            //panelContainer.Controls.Clear();

            if(log.StationId == 1)
            {
                uCtrlStation1 uc = new uCtrlStation1();
                uc.MonitorFlag = false;
                uc.traceabilityLog = log;
                uc.Dock = DockStyle.Top;
                this.panelContainer.Controls.Add(uc);
            }

            if (log.StationId == 2)
            {
                uCtrlStation2 uc = new uCtrlStation2();
                uc.MonitorFlag = false;
                uc.traceabilityLog = log;
                uc.Dock = DockStyle.Top;
                this.panelContainer.Controls.Add(uc);
            }

            if (log.StationId == 3)
            {
                uCtrlStation3 uc = new uCtrlStation3();
                uc.MonitorFlag = false;
                if(log.MachineId == 3) 
                    uc.traceabilityUpperLog = log;
                if (log.MachineId == 4)
                    uc.traceabilityLowerLog = log;
                uc.Dock = DockStyle.Top;
                this.panelContainer.Controls.Add(uc);
            }

            if (log.StationId == 4)
            {
                uCtrlStation4 uc = new uCtrlStation4();
                uc.MonitorFlag = false;
                uc.traceabilityLog = log;
                uc.Dock = DockStyle.Top;
                this.panelContainer.Controls.Add(uc);
            }

            if (log.StationId == 5)
            {
                uCtrlStation5 uc = new uCtrlStation5();
                uc.MonitorFlag = false;
                if (log.MachineId == 6)
                    uc.traceabilityUpperLog = log;
                if (log.MachineId == 7)
                    uc.traceabilityLowerLog = log; 
                uc.Dock = DockStyle.Top;
                this.panelContainer.Controls.Add(uc);
            }
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SearchDetailForm_Load(object sender, EventArgs e)
        {
            if (Form_Load != null)
                Form_Load(sender, e);
        }
    }
}
