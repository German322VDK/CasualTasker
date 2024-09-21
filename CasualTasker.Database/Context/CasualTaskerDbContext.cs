using CasualTasker.DTO;
using Microsoft.EntityFrameworkCore;

namespace CasualTasker.Database.Context
{
    public class CasualTaskerDbContext : DbContext
    {
        public DbSet<TaskDTO> Tasks { get; set; }
        public DbSet<CategoryDTO> Categories { get; set; }
        public CasualTaskerDbContext(DbContextOptions<CasualTaskerDbContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
    }
}
