using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trace.UI.Views;

namespace Trace.UI.UIs
{
    public partial class TigtheningRepairsForm : Form, ITigtheningRepairsView
    {
        public TigtheningRepairsForm()
        {
            InitializeComponent();
        }

        public BindingSource DataBinding 
        {
            get { return this.tighteningRepairModelBindingSource; }
            set { this.tighteningRepairModelBindingSource = value; } 
        }

        public event EventHandler FormLoad;
    }
}
