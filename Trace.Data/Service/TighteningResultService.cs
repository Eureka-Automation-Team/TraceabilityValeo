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

        public TighteningResultModel Create(TighteningResultModel entity)
        {
            entity.CreationDate = DateTime.Now;
            entity.LastUpdateDate = DateTime.Now;

            return _nonQueryDataService.Create(entity);
        }

        public bool DeleteByID(int id)
        {
            return _nonQueryDataService.Delete(id);
        }

        public IEnumerable<TighteningResultModel> GetAll()
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TighteningResultModel> entities = context.TighteningResults.ToList();
                return entities;
            }
        }

        public IEnumerable<TighteningResultModel> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TighteningResultModel> entities = context.TighteningResults
                                                    .Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                    .ToList();
                return entities;
            }
        }

        public TighteningResultModel GetByID(int id)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                TighteningResultModel entity = context.TighteningResults.FirstOrDefault((e) => e.Id == id);
                return entity;
            }
        }

        public IEnumerable<TighteningResultModel> GetByPrimary(TighteningResultModel model)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TighteningResultModel> entities = context.TighteningResults
                                                    .Where(x => x.TraceLogId == model.TraceLogId)
                                                    .ToList();
                return entities;
            }
        }

        public IEnumerable<TighteningResultModel> GetList(string whereClause, int takeRows)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<TighteningResultModel> entities = context.TighteningResults
                                                    //.Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                    .Take(takeRows)
                                                    .ToList();
                return entities;
            }
        }

        public IEnumerable<TighteningResultModel> GetListByItemCode(string itemCode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TighteningResultModel> GetListByMachineID(int id, int takeRows)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TighteningResultModel> GetListByStationID(int id)
        {
            throw new NotImplementedException();
        }

        public TighteningResultModel Update(TighteningResultModel entity)
        {
            entity.LastUpdateDate = DateTime.Now;
            return _nonQueryDataService.Update(entity.Id, entity);
        }
    }
}
