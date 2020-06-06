using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trace.Data
{
    public class TraceDbContextFactory : IDbContextFactory<TraceDbContext>
    {
        public TraceDbContext Create()
        {
            //Passw0rd@1
            string connectionString = ConfigurationManager.ConnectionStrings["my_sql"].ConnectionString;
            return new TraceDbContext(connectionString);
            //return new TraceabilityDbContext("server=localhost;database=traceability;user=root;password=Passw0rd@1;");
        }
    }
}
