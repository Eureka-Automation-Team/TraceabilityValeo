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

        public MachineModel Create(MachineModel entity)
        {
            entity.CreationDate = DateTime.Now;
            entity.LastUpdateDate = DateTime.Now;

            return _nonQueryDataService.Create(entity);
        }

        public bool DeleteByID(int id)
        {
            return _nonQueryDataService.Delete(id);
        }

        public IEnumerable<MachineModel> GetAll()
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<MachineModel> entities = context.Machines.ToList();
                return entities;
            }
        }

        public IEnumerable<MachineModel> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<MachineModel> entities = context.Machines
                                                            .Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                            .ToList();
                return entities;
            }
        }

        public MachineModel GetByID(int id)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                MachineModel entity = context.Machines.FirstOrDefault((e) => e.Id == id);
                return entity;
            }
        }

        public IEnumerable<MachineModel> GetByPrimary(MachineModel model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MachineModel> GetList(string whereClause, int takeRows)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<MachineModel> entities = context.Machines
                                                            //.Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                            .Take(takeRows)
                                                            .ToList();
                return entities;
            }
        }

        public IEnumerable<MachineModel> GetListByItemCode(string itemCode)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MachineModel> GetListByMachineID(int id, int takeRows)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MachineModel> GetListByStationID(int id)
        {
            throw new NotImplementedException();
        }

        public MachineModel Update(MachineModel entity)
        {
            entity.LastUpdateDate = DateTime.Now;
            return _nonQueryDataService.Update(entity.Id, entity);
        }
    }
}
