using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trace.Data.Service.Common;
using Trace.Domain.Models;
using Trace.Domain.Services;

namespace Trace.Data.Service
{
    public class TighteningResultService : IDataService<TighteningResultModel>
    {
        private readonly TraceDbContextFactory _contextFactory;
        private readonly NonQueryDataService<TighteningResultModel> _nonQueryDataService;

        public TighteningResultService(TraceDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            _nonQueryDataService = new NonQueryDataService<TighteningResultModel>(contextFactory);
        }

        public async Task<TighteningResultModel> Create(TighteningResultModel entity)
        {
            entity.CreationDate = DateTime.Now;
            entity.LastUpdateDate = DateTime.Now;

            return await _nonQueryDataService.Create(entity);
        }

        public async Task<bool> DeleteByID(int id)
        {
            return await _nonQueryDataService.Delete(id);
        }

        public async Task<IEnumerable<TighteningResultModel>> GetAll()
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TighteningResultModel> entities = await context.TighteningResults.ToListAsync();
                return entities;
            }
        }

        public async Task<IEnumerable<TighteningResultModel>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TighteningResultModel> entities = await context.TighteningResults
                                                    .Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                    .ToListAsync();
                return entities;
            }
        }

        public async Task<TighteningResultModel> GetByID(int id)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                TighteningResultModel entity = await context.TighteningResults.FirstOrDefaultAsync((e) => e.Id == id);
                return entity;
            }
        }

        public async Task<IEnumerable<TighteningResultModel>> GetByPrimary(TighteningResultModel model)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TighteningResultModel> entities = await context.TighteningResults
                                                    .Where(x => x.TraceLogId == model.TraceLogId)
                                                    .ToListAsync();
                return entities;
            }
        }

        public async Task<IEnumerable<TighteningResultModel>> GetList(string whereClause, int takeRows)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TighteningResultModel> entities = await context.TighteningResults
                                                    //.Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                    .Take(takeRows)
                                                    .ToListAsync();
                return entities;
            }
        }

        public Task<IEnumerable<TighteningResultModel>> GetListByItemCode(string itemCode)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TighteningResultModel>> GetListByMachineID(int id, int takeRows)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TighteningResultModel>> GetListByStationID(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<TighteningResultModel> Update(TighteningResultModel entity)
        {
            entity.LastUpdateDate = DateTime.Now;
            return await _nonQueryDataService.Update(entity.Id, entity);
        }
    }
}
