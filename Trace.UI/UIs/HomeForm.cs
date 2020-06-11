using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Trace.UI.UIs
{
    public partial class HomeForm : Form
    {
        public HomeForm()
        {
            InitializeComponent();
            uCtrlStation11.MonitorFlag = true;
            uCtrlStation21.MonitorFlag = true;
            uCtrlStation31.MonitorFlag = true;
            uCtrlStation41.MonitorFlag = true;
            uCtrlStation51.MonitorFlag = true;
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void HomeForm_Load(object sender, EventArgs e)
        {
            
        }
    }
}
