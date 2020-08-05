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

        public PartAssemblyModel Create(PartAssemblyModel entity)
        {
            entity.CreationDate = DateTime.Now;
            entity.LastUpdateDate = DateTime.Now;

            return _nonQueryDataService.Create(entity);
        }

        public bool DeleteByID(int id)
        {
            return _nonQueryDataService.Delete(id);
        }

        public IEnumerable<PartAssemblyModel> GetAll()
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<PartAssemblyModel> entities = context.PartAssemblies.ToList();
                return entities;
            }
        }

        public IEnumerable<PartAssemblyModel> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<PartAssemblyModel> entities = context.PartAssemblies
                                                    .Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                    .ToList();
                return entities;
            }
        }

        public PartAssemblyModel GetByID(int id)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                PartAssemblyModel entity = context.PartAssemblies.FirstOrDefault((e) => e.Id == id);
                return entity;
            }
        }

        public IEnumerable<PartAssemblyModel> GetByPrimary(PartAssemblyModel model)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<PartAssemblyModel> entities = context.PartAssemblies
                                                    .Where(x => x.TraceabilityLogId == model.TraceabilityLogId)
                                                    .ToList();
                return entities;
            }
        }

        public IEnumerable<PartAssemblyModel> GetList(string whereClause, int takeRows)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<PartAssemblyModel> entities = context.PartAssemblies
                                                    .Take(takeRows)
                                                    .ToList();
                return entities;
            }
        }

        public IEnumerable<PartAssemblyModel> GetListByItemCode(string itemCode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PartAssemblyModel> GetListByMachineID(int id, int takeRows)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PartAssemblyModel> GetListByStationID(int id)
        {
            throw new NotImplementedException();
        }

        public PartAssemblyModel Update(PartAssemblyModel entity)
        {
            entity.LastUpdateDate = DateTime.Now;
            return _nonQueryDataService.Update(entity.Id, entity);
        }
    }
}
