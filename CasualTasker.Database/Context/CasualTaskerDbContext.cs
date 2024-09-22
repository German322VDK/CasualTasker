using CasualTasker.DTO;
using Microsoft.EntityFrameworkCore;

namespace CasualTasker.Database.Context
{
    /// <summary>
    /// Represents the database context for the CasualTasker application, 
    /// providing access to tasks and categories in the SQLite database.
    /// Inherits from <see cref="DbContext"/> and configures the query tracking behavior.
    /// </summary>
    public class CasualTaskerDbContext : DbContext
    {
        /// <summary>
        /// Gets or sets the <see cref="DbSet{TaskDTO}"/> representing the tasks in the database.
        /// </summary>
        public DbSet<TaskDTO> Tasks { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{CategoryDTO}"/> representing the categories in the database.
        /// </summary>
        public DbSet<CategoryDTO> Categories { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CasualTaskerDbContext"/> class.
        /// Configures the database options and disables default entity tracking 
        /// by setting the <see cref="ChangeTracker.QueryTrackingBehavior"/> to <see cref="QueryTrackingBehavior.NoTracking"/>.
        /// </summary>
        /// <param name="options">
        /// The options to be used by this <see cref="DbContext"/> instance, typically passed from the DI container.
        /// </param>
        public CasualTaskerDbContext(DbContextOptions<CasualTaskerDbContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
    }
}
