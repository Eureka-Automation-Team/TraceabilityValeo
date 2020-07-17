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
    public class PartAssemblyService : IDataService<PartAssemblyModel>
    {
        private readonly TraceDbContextFactory _contextFactory;
        private readonly NonQueryDataService<PartAssemblyModel> _nonQueryDataService;

        public PartAssemblyService(TraceDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            _nonQueryDataService = new NonQueryDataService<PartAssemblyModel>(contextFactory);
        }

        public async Task<PartAssemblyModel> Create(PartAssemblyModel entity)
        {
            entity.CreationDate = DateTime.Now;
            entity.LastUpdateDate = DateTime.Now;

            return await _nonQueryDataService.Create(entity);
        }

        public async Task<bool> DeleteByID(int id)
        {
            return await _nonQueryDataService.Delete(id);
        }

        public async Task<IEnumerable<PartAssemblyModel>> GetAll()
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<PartAssemblyModel> entities = await context.PartAssemblies.ToListAsync();
                return entities;
            }
        }

        public async Task<IEnumerable<PartAssemblyModel>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<PartAssemblyModel> entities = await context.PartAssemblies
                                                    .Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                    .ToListAsync();
                return entities;
            }
        }

        public async Task<PartAssemblyModel> GetByID(int id)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                PartAssemblyModel entity = await context.PartAssemblies.FirstOrDefaultAsync((e) => e.Id == id);
                return entity;
            }
        }

        public Task<IEnumerable<PartAssemblyModel>> GetByPrimary(PartAssemblyModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PartAssemblyModel>> GetList(string whereClause, int takeRows)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<PartAssemblyModel> entities = await context.PartAssemblies
                                                    //.Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                    .Take(takeRows)
                                                    .ToListAsync();
                return entities;
            }
        }

        public Task<IEnumerable<PartAssemblyModel>> GetListByItemCode(string itemCode)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PartAssemblyModel>> GetListByMachineID(int id, int takeRows)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PartAssemblyModel>> GetListByStationID(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PartAssemblyModel> Update(PartAssemblyModel entity)
        {
            entity.LastUpdateDate = DateTime.Now;
            return await _nonQueryDataService.Update(entity.Id, entity);
        }
    }
}
