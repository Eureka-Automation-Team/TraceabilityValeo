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

        public PlcTagModel Create(PlcTagModel entity)
        {
            entity.CreationDate = DateTime.Now;
            entity.LastUpdateDate = DateTime.Now;

            return _nonQueryDataService.Create(entity);
        }

        public bool DeleteByID(int id)
        {
            return _nonQueryDataService.Delete(id);
        }

        public IEnumerable<PlcTagModel> GetAll()
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<PlcTagModel> entities = context.PlcTags.ToList();
                return entities;
            }
        }

        public IEnumerable<PlcTagModel> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<PlcTagModel> entities = context.PlcTags
                                                        .Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                        .ToList();
                return entities;
            }
        }

        public PlcTagModel GetByID(int id)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                PlcTagModel entity = context.PlcTags.FirstOrDefault((e) => e.Id == id);
                return entity;
            }
        }

        public IEnumerable<PlcTagModel> GetByPrimary(PlcTagModel model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PlcTagModel> GetList(string whereClause, int takeRows)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<PlcTagModel> entities = context.PlcTags
                                                    //.Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                    .Take(takeRows)
                                                    .ToList();
                return entities;
            }
        }

        public IEnumerable<PlcTagModel> GetListByItemCode(string itemCode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PlcTagModel> GetListByMachineID(int id, int takeRows)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PlcTagModel> GetListByStationID(int id)
        {
            throw new NotImplementedException();
        }

        public PlcTagModel Update(PlcTagModel entity)
        {
            entity.LastUpdateDate = DateTime.Now;
            return _nonQueryDataService.Update(entity.Id, entity);
        }
    }
}
