using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trace.Domain.Models;

namespace Trace.UI.Views
{
    public interface ISearchView
    {
        event EventHandler Form_Load;
        event EventHandler Search_Click;
        event EventHandler Clear_Click;
        event EventHandler Selected_Row;
        event EventHandler PreviousPage;
        event EventHandler NextPage;

        int pageNumber { get; set; }
        //IPagedList<TraceabilityLogModel> list { get; set; }
        BindingSource DataBinding { get; set; }
        string itemCode { get; set; }
        string partSerialNo { get; set; }
        DateTime startDate { get; set; }
        DateTime endDate { get; set; }

        void CloseMe();
    }
}
