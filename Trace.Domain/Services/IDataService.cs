using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trace.Domain.Services
{
    public interface IDataService<T>
    {
        Task<IEnumerable<T>> GetAll();

        Task<IEnumerable<T>> GetList(string whereClause, int takeRows);

        Task<IEnumerable<T>> GetListByMachineID(int id, int takeRows);

        Task<IEnumerable<T>> GetListByStationID(int id);

        Task<IEnumerable<T>> GetListByItemCode(string itemCode);

        Task<IEnumerable<T>> GetByDateRange(DateTime startDate, DateTime endDate);

        Task<IEnumerable<T>> GetByPrimary(T model);

        Task<T> GetByID(int id);

        Task<T> Create(T entity);

        Task<T> Update(T entity);

        Task<bool> DeleteByID(int id);
    }
}
