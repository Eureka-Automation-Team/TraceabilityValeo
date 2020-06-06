using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trace.Domain.Models;

namespace Trace.Data.Service.Common
{
    public class NonQueryDataService<T> where T : DomainObject
    {
        private readonly TraceDbContextFactory _contextFactory;

        public NonQueryDataService(TraceDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<T> Create(T entity)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                T createdResult = context.Set<T>().Add(entity);
                await context.SaveChangesAsync();

                return createdResult;
            }
        }

        public async Task<T> Update(int id, T entity)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                T updatedResult = await context.Set<T>().FirstOrDefaultAsync((o) => o.Id == id);
                if (updatedResult != null)
                {
                    context.Entry(updatedResult).CurrentValues.SetValues(entity);
                    await context.SaveChangesAsync();
                }

                return entity;
            }
        }

        public async Task<bool> Delete(int id)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                T entity = await context.Set<T>().FirstOrDefaultAsync((e) => e.Id == id);
                context.Set<T>().Remove(entity);
                await context.SaveChangesAsync();

                return true;
            }
        }
    }
}
