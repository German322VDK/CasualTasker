using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CasualTasker.Database.Context.DesignTimeDbContextFactories.Sqlite
{
    public class CasualTaskerDesignTimeSqliteDbContextFactory : IDesignTimeDbContextFactory<CasualTaskerDbContext>
    {
        public CasualTaskerDbContext CreateDbContext(string[] args)
        {
            const string connection_string = "Data Source=CasualTaskManager.db;Cache=Shared";

            DbContextOptionsBuilder<CasualTaskerDbContext> optionsBuilder = new DbContextOptionsBuilder<CasualTaskerDbContext>();
            optionsBuilder.UseSqlite(connection_string);

            return new CasualTaskerDbContext(optionsBuilder.Options);
        }
    }
}
