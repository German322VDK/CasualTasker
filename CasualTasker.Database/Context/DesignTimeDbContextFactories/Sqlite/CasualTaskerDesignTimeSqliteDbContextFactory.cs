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
        /// <summary>
        /// Creates a new instance of <see cref="CasualTaskerDbContext"/> with the specified arguments.
        /// </summary>
        /// <param name="args">Arguments used to configure the context (not utilized in this implementation).</param>
        /// <returns>A new instance of <see cref="CasualTaskerDbContext"/> configured for SQLite.</returns>
        public CasualTaskerDbContext CreateDbContext(string[] args)
        {
            const string connection_string = "Data Source=CasualTaskManager.db;Cache=Shared";

            DbContextOptionsBuilder<CasualTaskerDbContext> optionsBuilder = new DbContextOptionsBuilder<CasualTaskerDbContext>();
            optionsBuilder.UseSqlite(connection_string);

            return new CasualTaskerDbContext(optionsBuilder.Options);
        }
    }
}
