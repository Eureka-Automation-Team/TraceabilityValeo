using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Trace.UI.Views
{
    public interface ITigtheningRepairsView
    {
        event EventHandler FormLoad;

        BindingSource DataBinding { get; set; }
    }
}
