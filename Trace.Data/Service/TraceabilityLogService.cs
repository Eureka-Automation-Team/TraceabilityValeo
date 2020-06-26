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

        public async Task<IEnumerable<TraceabilityLogModel>> GetAll()
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TraceabilityLogModel> entities = await context.TraceabilityLogs.ToListAsync();
                return entities;
            }
        }

        public async Task<IEnumerable<TraceabilityLogModel>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TraceabilityLogModel> entities = await context.TraceabilityLogs
                                                    .Where(x => DbFunctions.TruncateTime(x.CreationDate) >= startDate.Date
                                                    && DbFunctions.TruncateTime(x.CreationDate) <= endDate.Date)
                                                    .ToListAsync();
                return entities;
            }
        }

        public async Task<TraceabilityLogModel> GetByID(int id)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                TraceabilityLogModel entity = await context.TraceabilityLogs
                                                           .Include(i => i.Machine)
                                                           .Include(i => i.Station)
                                                           .Include(i => i.PartAssemblies)
                                                           .Include(i => i.TighteningResults)
                                                           .Include(i => i.CameraResults)
                                                           .FirstOrDefaultAsync((e) => e.Id == id);
                return entity;
            }
        }

        public async Task<IEnumerable<TraceabilityLogModel>> GetList(string whereClause, int takeRows)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TraceabilityLogModel> entities = await context.TraceabilityLogs
                                                    //.Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                    .OrderByDescending(o => o.CreationDate)
                                                    .Take(takeRows)                                                    
                                                    .ToListAsync();
                return entities;
            }
        }

        public async Task<IEnumerable<TraceabilityLogModel>> GetListByItemCode(string itemCode)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TraceabilityLogModel> entities = await context.TraceabilityLogs
                                                    .Where(x => x.ItemCode == itemCode)
                                                    //.OrderByDescending(o => o.CreationDate)
                                                    .ToListAsync();
                return entities;
            }
        }

        public async Task<IEnumerable<TraceabilityLogModel>> GetListByMachineID(int id, int takeRows)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TraceabilityLogModel> entities = await context.TraceabilityLogs
                                                    .Where(x => x.MachineId == id)
                                                    .OrderByDescending(o => o.CreationDate)
                                                    .Take(takeRows)
                                                    .ToListAsync();
                return entities;
            }
        }

        public async Task<IEnumerable<TraceabilityLogModel>> GetListByStationID(int id)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TraceabilityLogModel> entities = await context.TraceabilityLogs
                                                    .Where(x => x.StationId == id)
                                                    .OrderByDescending(o => o.CreationDate)
                                                    //.Take(takeRows)
                                                    .ToListAsync();
                return entities;
            }
        }

        public async Task<TraceabilityLogModel> Update(TraceabilityLogModel entity)
        {
            entity.LastUpdateDate = DateTime.Now;
            return await _nonQueryDataService.Update(entity.Id, entity);
        }
    }
}
