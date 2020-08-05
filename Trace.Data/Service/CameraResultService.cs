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
    public class CameraResultService : IDataService<CameraResultModel>
    {
        private readonly TraceDbContextFactory _contextFactory;
        private readonly NonQueryDataService<CameraResultModel> _nonQueryDataService;

        public CameraResultService(TraceDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            _nonQueryDataService = new NonQueryDataService<CameraResultModel>(contextFactory);
        }

        public CameraResultModel Create(CameraResultModel entity)
        {
            entity.CreationDate = DateTime.Now;
            entity.LastUpdateDate = DateTime.Now;

            return _nonQueryDataService.Create(entity);
        }

        public bool DeleteByID(int id)
        {
            return _nonQueryDataService.Delete(id);
        }

        public IEnumerable<CameraResultModel> GetAll()
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<CameraResultModel> entities = context.CameraResults.ToList();
                return entities;
            }
        }

        public IEnumerable<CameraResultModel> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<CameraResultModel> entities = context.CameraResults
                                                    .Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                    .ToList();
                return entities;
            }
        }

        public CameraResultModel GetByID(int id)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                CameraResultModel entity = context.CameraResults.FirstOrDefault((e) => e.Id == id);
                return entity;
            }
        }

        public IEnumerable<CameraResultModel> GetByPrimary(CameraResultModel model)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<CameraResultModel> entities = context.CameraResults
                                                    .Where(x => x.TraceLogId == model.TraceLogId)
                                                    .ToList();
                return entities;
            }
        }

        public IEnumerable<CameraResultModel> GetList(string whereClause, int takeRows)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<CameraResultModel> entities = context.CameraResults
                                                    //.Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                    .Take(takeRows)
                                                    .ToList();
                return entities;
            }
        }

        public IEnumerable<CameraResultModel> GetListByItemCode(string itemCode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CameraResultModel> GetListByMachineID(int id, int takeRows)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CameraResultModel> GetListByStationID(int id)
        {
            throw new NotImplementedException();
        }

        public CameraResultModel Update(CameraResultModel entity)
        {
            entity.LastUpdateDate = DateTime.Now;
            return _nonQueryDataService.Update(entity.Id, entity);
        }
    }
}
