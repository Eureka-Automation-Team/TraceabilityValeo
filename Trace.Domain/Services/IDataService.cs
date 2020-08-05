using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trace.Domain.Services
{
    public interface IDataService<T>
    {
        IEnumerable<T> GetAll();

        IEnumerable<T> GetList(string whereClause, int takeRows);

        IEnumerable<T> GetListByMachineID(int id, int takeRows);

        IEnumerable<T> GetListByStationID(int id);

        IEnumerable<T> GetListByItemCode(string itemCode);

        IEnumerable<T> GetByDateRange(DateTime startDate, DateTime endDate);

        IEnumerable<T> GetByPrimary(T model);

        T GetByID(int id);

        T Create(T entity);

        T Update(T entity);

        bool DeleteByID(int id);
    }
}
