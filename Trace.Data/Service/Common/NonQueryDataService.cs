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

        public T Create(T entity)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                T createdResult = context.Set<T>().Add(entity);
                context.SaveChanges();

                return createdResult;
            }
        }

        public T Update(int id, T entity)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                T updatedResult = context.Set<T>().FirstOrDefault((o) => o.Id == id);
                if (updatedResult != null)
                {
                    context.Entry(updatedResult).CurrentValues.SetValues(entity);
                    context.SaveChanges();
                }

                return entity;
            }
        }

        public bool Delete(int id)
        {
            using (TraceDbContext context = _contextFactory.Create())
            {
                T entity =  context.Set<T>().FirstOrDefault((e) => e.Id == id);
                context.Set<T>().Remove(entity);
                 context.SaveChanges();

                return true;
            }
        }
    }
}
