using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CasualTasker.Database.Context.DesignTimeDbContextFactories.Sqlite
{
    /// <summary>
    /// Provides a factory for creating instances of <see cref="CasualTaskerDbContext"/> at design time for SQLite database.
    /// This factory is typically used by tools like Entity Framework migrations when the application is not running.
    /// </summary>
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
