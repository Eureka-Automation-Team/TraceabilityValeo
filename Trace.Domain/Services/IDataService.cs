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

        Task<IEnumerable<T>> GetAll(int id);

        Task<IEnumerable<T>> GetByDateRange(DateTime startDate, DateTime endDate);

        Task<IEnumerable<T>> GetTakeRows(int takeRows);

        Task<T> GetByID(int id);

        Task<T> GetByCode(T model);

        Task<T> Create(T entity);

        Task<T> Update(int id, T entity);

        Task<bool> Delete(int id);
    }
}
