using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trace.UI.Views
{
    public interface IMainView
    {
        event EventHandler FormLoad;
        event EventHandler OpenForm;

        void LoadSubForm(object form);
    }
}
