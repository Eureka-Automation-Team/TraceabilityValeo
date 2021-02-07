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

        public TraceabilityLogModel Create(TraceabilityLogModel entity)
        {
            entity.CreationDate = DateTime.Now;
            entity.LastUpdateDate = DateTime.Now;

            return _nonQueryDataService.Create(entity);
        }

        public bool DeleteByID(int id)
        {
            return _nonQueryDataService.Delete(id);
        }

        public IEnumerable<TraceabilityLogModel> GetAll()
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TraceabilityLogModel> entities = context.TraceabilityLogs.ToList();
                return entities;
            }
        }

        public IEnumerable<TraceabilityLogModel> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TraceabilityLogModel> entities = context.TraceabilityLogs
                                                    .Where(x => DbFunctions.TruncateTime(x.CreationDate) >= startDate.Date
                                                    && DbFunctions.TruncateTime(x.CreationDate) <= endDate.Date)
                                                    .ToList();
                return entities;
            }
        }

        public TraceabilityLogModel GetByID(int id)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                TraceabilityLogModel entity = context.TraceabilityLogs
                                                           .Include(i => i.Machine)
                                                           .Include(i => i.Station)
                                                           .Include(i => i.PartAssemblies)
                                                           .Include(i => i.TighteningResults)
                                                           .Include(i => i.CameraResults)
                                                           .FirstOrDefault((e) => e.Id == id);
                return entity;
            }
        }

        public IEnumerable<TraceabilityLogModel> GetByPrimary(TraceabilityLogModel model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TraceabilityLogModel> GetList(string whereClause, int takeRows)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TraceabilityLogModel> entities =  context.TraceabilityLogs
                                                    //.Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                    .OrderByDescending(o => o.CreationDate)
                                                    //.Take(takeRows)                                                    
                                                    .ToList();
                return entities;
            }
        }

        public IEnumerable<TraceabilityLogModel> GetListByItemCode(string itemCode)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TraceabilityLogModel> entities = context.TraceabilityLogs
                                                    .Where(x => x.ItemCode == itemCode)
                                                    //.OrderByDescending(o => o.CreationDate)
                                                    .ToList();
                return entities;
            }
        }

        public IEnumerable<TraceabilityLogModel> GetListByMachineID(int id, int takeRows)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TraceabilityLogModel> entities = context.TraceabilityLogs
                                                    .Where(x => x.MachineId == id)
                                                    .OrderByDescending(o => o.CreationDate)
                                                    .Take(takeRows)
                                                    .ToList();
                return entities;
            }
        }

        public IEnumerable<TraceabilityLogModel> GetListByStationID(int id)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TraceabilityLogModel> entities = context.TraceabilityLogs
                                                    .Where(x => x.StationId == id)
                                                    .OrderByDescending(o => o.CreationDate)
                                                    //.Take(takeRows)
                                                    .ToList();
                return entities;
            }
        }

        public TraceabilityLogModel Update(TraceabilityLogModel entity)
        {
            entity.LastUpdateDate = DateTime.Now;
            return _nonQueryDataService.Update(entity.Id, entity);
        }
    }
}
