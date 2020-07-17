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
    public class TighteningRepairService : IDataService<TighteningRepairModel>
    {
        private readonly TraceDbContextFactory _contextFactory;
        private readonly NonQueryDataService<TighteningRepairModel> _nonQueryDataService;

        public TighteningRepairService(TraceDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            _nonQueryDataService = new NonQueryDataService<TighteningRepairModel>(contextFactory);
        }

        public async Task<TighteningRepairModel> Create(TighteningRepairModel entity)
        {
            entity.CreationDate = DateTime.Now;
            entity.LastUpdateDate = DateTime.Now;

            return await _nonQueryDataService.Create(entity);
        }

        public async Task<bool> DeleteByID(int id)
        {
            return await _nonQueryDataService.Delete(id);
        }

        public async Task<IEnumerable<TighteningRepairModel>> GetAll()
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TighteningRepairModel> entities = await context.TighteningRepairs.ToListAsync();
                return entities;
            }
        }

        public async Task<IEnumerable<TighteningRepairModel>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TighteningRepairModel> entities = await context.TighteningRepairs
                                                    .Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                    .ToListAsync();
                return entities;
            }
        }

        public async Task<TighteningRepairModel> GetByID(int id)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                TighteningRepairModel entity = await context.TighteningRepairs.FirstOrDefaultAsync((e) => e.Id == id);
                return entity;
            }
        }

        public async Task<IEnumerable<TighteningRepairModel>> GetByPrimary(TighteningRepairModel model)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TighteningRepairModel> entities = await context.TighteningRepairs
                                                    .Where(x => x.TighteningResultId == model.TighteningResultId)
                                                    .ToListAsync();
                return entities;
            }
        }

        public async Task<IEnumerable<TighteningRepairModel>> GetList(string whereClause, int takeRows)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TighteningRepairModel> entities = await context.TighteningRepairs
                                                    .Take(takeRows)
                                                    .ToListAsync();
                return entities;
            }
        }

        public Task<IEnumerable<TighteningRepairModel>> GetListByItemCode(string itemCode)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TighteningRepairModel>> GetListByMachineID(int id, int takeRows)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TighteningRepairModel>> GetListByStationID(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<TighteningRepairModel> Update(TighteningRepairModel entity)
        {
            entity.LastUpdateDate = DateTime.Now;
            return await _nonQueryDataService.Update(entity.Id, entity);
        }
    }
}
