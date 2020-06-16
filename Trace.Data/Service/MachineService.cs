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
    public class MachineService : IDataService<MachineModel>
    {
        private readonly TraceDbContextFactory _contextFactory;
        private readonly NonQueryDataService<MachineModel> _nonQueryDataService;

        public MachineService(TraceDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            _nonQueryDataService = new NonQueryDataService<MachineModel>(contextFactory);
        }

        public async Task<MachineModel> Create(MachineModel entity)
        {
            entity.CreationDate = DateTime.Now;
            entity.LastUpdateDate = DateTime.Now;

            return await _nonQueryDataService.Create(entity);
        }

        public async Task<bool> DeleteByID(int id)
        {
            return await _nonQueryDataService.Delete(id);
        }

        public async Task<IEnumerable<MachineModel>> GetAll()
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<MachineModel> entities = await context.Machines.ToListAsync();
                return entities;
            }
        }

        public async Task<IEnumerable<MachineModel>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<MachineModel> entities = await context.Machines
                                                                  .Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                                  .ToListAsync();
                return entities;
            }
        }

        public async Task<MachineModel> GetByID(int id)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                MachineModel entity = await context.Machines.FirstOrDefaultAsync((e) => e.Id == id);
                return entity;
            }
        }

        public async Task<IEnumerable<MachineModel>> GetList(string whereClause, int takeRows)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<MachineModel> entities = await context.Machines
                                                                  //.Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                                  .Take(takeRows)
                                                                  .ToListAsync();
                return entities;
            }
        }

        public Task<IEnumerable<MachineModel>> GetListByItemCode(string itemCode)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MachineModel>> GetListByMachineID(int id, int takeRows)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MachineModel>> GetListByStationID(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<MachineModel> Update(MachineModel entity)
        {
            entity.LastUpdateDate = DateTime.Now;
            return await _nonQueryDataService.Update(entity.Id, entity);
        }
    }
}
