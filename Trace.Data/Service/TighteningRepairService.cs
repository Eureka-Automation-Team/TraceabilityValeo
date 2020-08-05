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

        public TighteningRepairModel Create(TighteningRepairModel entity)
        {
            entity.CreationDate = DateTime.Now;
            entity.LastUpdateDate = DateTime.Now;

            return _nonQueryDataService.Create(entity);
        }

        public bool DeleteByID(int id)
        {
            return _nonQueryDataService.Delete(id);
        }

        public IEnumerable<TighteningRepairModel> GetAll()
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TighteningRepairModel> entities = context.TighteningRepairs.ToList();
                return entities;
            }
        }

        public IEnumerable<TighteningRepairModel> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TighteningRepairModel> entities = context.TighteningRepairs
                                                    .Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                    .ToList();
                return entities;
            }
        }

        public TighteningRepairModel GetByID(int id)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                TighteningRepairModel entity = context.TighteningRepairs.FirstOrDefault((e) => e.Id == id);
                return entity;
            }
        }

        public IEnumerable<TighteningRepairModel> GetByPrimary(TighteningRepairModel model)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TighteningRepairModel> entities = context.TighteningRepairs
                                                    .Where(x => x.TighteningResultId == model.TighteningResultId)
                                                    .ToList();
                return entities;
            }
        }

        public IEnumerable<TighteningRepairModel> GetList(string whereClause, int takeRows)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TighteningRepairModel> entities = context.TighteningRepairs
                                                    .Take(takeRows)
                                                    .ToList();
                return entities;
            }
        }

        public IEnumerable<TighteningRepairModel> GetListByItemCode(string itemCode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TighteningRepairModel> GetListByMachineID(int id, int takeRows)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TighteningRepairModel> GetListByStationID(int id)
        {
            throw new NotImplementedException();
        }

        public TighteningRepairModel Update(TighteningRepairModel entity)
        {
            entity.LastUpdateDate = DateTime.Now;
            return _nonQueryDataService.Update(entity.Id, entity);
        }
    }
}
