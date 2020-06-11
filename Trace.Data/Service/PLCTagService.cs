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
    public class PLCTagService: IDataService<PlcTagModel>
    {
        private readonly TraceDbContextFactory _contextFactory;
        private readonly NonQueryDataService<PlcTagModel> _nonQueryDataService;

        public PLCTagService(TraceDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            _nonQueryDataService = new NonQueryDataService<PlcTagModel>(contextFactory);
        }

        public async Task<PlcTagModel> Create(PlcTagModel entity)
        {
            entity.CreationDate = DateTime.Now;
            entity.LastUpdateDate = DateTime.Now;

            return await _nonQueryDataService.Create(entity);
        }

        public async Task<bool> DeleteByID(int id)
        {
            return await _nonQueryDataService.Delete(id);
        }

        public async Task<IEnumerable<PlcTagModel>> GetAll()
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<PlcTagModel> entities = await context.PlcTags.ToListAsync();
                return entities;
            }
        }

        public async Task<IEnumerable<PlcTagModel>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<PlcTagModel> entities = await context.PlcTags
                                                    .Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                    .ToListAsync();
                return entities;
            }
        }

        public async Task<PlcTagModel> GetByID(int id)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                PlcTagModel entity = await context.PlcTags.FirstOrDefaultAsync((e) => e.Id == id);
                return entity;
            }
        }

        public async Task<IEnumerable<PlcTagModel>> GetList(string whereClause, int takeRows)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<PlcTagModel> entities = await context.PlcTags
                                                    //.Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                    .Take(takeRows)
                                                    .ToListAsync();
                return entities;
            }
        }

        public Task<IEnumerable<PlcTagModel>> GetListByMachineID(int id, int takeRows)
        {
            throw new NotImplementedException();
        }

        public async Task<PlcTagModel> Update(PlcTagModel entity)
        {
            entity.LastUpdateDate = DateTime.Now;
            return await _nonQueryDataService.Update(entity.Id, entity);
        }
    }
}
