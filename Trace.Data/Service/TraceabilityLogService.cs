using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trace.Data.Service.Common;
using Trace.Domain.Models;
using Trace.Domain.Services;

namespace Trace.Data.Service
{
    public class TraceabilityLogService : IDataService<TraceabilityLogModel>
    {
        private readonly TraceDbContextFactory _contextFactory;
        private readonly NonQueryDataService<TraceabilityLogModel> _nonQueryDataService;

        public TraceabilityLogService(TraceDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            _nonQueryDataService = new NonQueryDataService<TraceabilityLogModel>(contextFactory);
        }

        public async Task<TraceabilityLogModel> Create(TraceabilityLogModel entity)
        {
            entity.CreationDate = DateTime.Now;
            entity.LastUpdateDate = DateTime.Now;

            return await _nonQueryDataService.Create(entity);
        }

        public async Task<bool> DeleteByID(int id)
        {
            return await _nonQueryDataService.Delete(id);
        }

        public Task<IEnumerable<TraceabilityLogModel>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TraceabilityLogModel>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<TraceabilityLogModel> GetByID(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TraceabilityLogModel>> GetList(string whereClause, int takeRows)
        {
            throw new NotImplementedException();
        }

        public async Task<TraceabilityLogModel> Update(TraceabilityLogModel entity)
        {
            entity.LastUpdateDate = DateTime.Now;
            return await _nonQueryDataService.Update(entity.Id, entity);
        }
    }
}
