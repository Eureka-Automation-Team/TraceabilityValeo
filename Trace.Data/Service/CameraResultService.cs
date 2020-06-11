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

        public async Task<CameraResultModel> Create(CameraResultModel entity)
        {
            entity.CreationDate = DateTime.Now;
            entity.LastUpdateDate = DateTime.Now;

            return await _nonQueryDataService.Create(entity);
        }

        public async Task<bool> DeleteByID(int id)
        {
            return await _nonQueryDataService.Delete(id);
        }

        public async Task<IEnumerable<CameraResultModel>> GetAll()
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<CameraResultModel> entities = await context.CameraResults.ToListAsync();
                return entities;
            }
        }

        public async Task<IEnumerable<CameraResultModel>> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<CameraResultModel> entities = await context.CameraResults
                                                    .Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                    .ToListAsync();
                return entities;
            }
        }

        public async Task<CameraResultModel> GetByID(int id)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                CameraResultModel entity = await context.CameraResults.FirstOrDefaultAsync((e) => e.Id == id);
                return entity;
            }
        }

        public async Task<IEnumerable<CameraResultModel>> GetList(string whereClause, int takeRows)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                IEnumerable<CameraResultModel> entities = await context.CameraResults
                                                    //.Where(x => x.CreationDate.Date >= startDate.Date && x.CreationDate.Date <= endDate.Date)
                                                    .Take(takeRows)
                                                    .ToListAsync();
                return entities;
            }
        }

        public Task<IEnumerable<CameraResultModel>> GetListByMachineID(int id, int takeRows)
        {
            throw new NotImplementedException();
        }

        public async Task<CameraResultModel> Update(CameraResultModel entity)
        {
            entity.LastUpdateDate = DateTime.Now;
            return await _nonQueryDataService.Update(entity.Id, entity);
        }
    }
}
