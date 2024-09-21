using CasualTasker.Database.Context;
using CasualTasker.DTO;
using CasualTasker.Services.Fallbacks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CasualTasker.Services.Stores
{
    public class CategoryStore : DbStore<CategoryDTO>
    {
        private readonly ILogger<CategoryStore> _logger;
        private readonly CasualTaskerDbContext _dbContext;
        private readonly ICategoryFallbackService _categoryFallbackService;

        public CategoryStore(
            CasualTaskerDbContext dbContext, 
            ILogger<CategoryStore> logger,
            ICategoryFallbackService categoryFallbackService) : base(dbContext, logger)
        {
            _categoryFallbackService = categoryFallbackService;
            _dbContext = dbContext;
            _logger = logger;
        }

        public override bool Delete(int id)
        {
            _logger.LogInformation($"Начало работы метода {nameof(Delete)} класса {nameof(CategoryStore)}");

            var entity = CheckEntityExist(id, nameof(Delete), nameof(CategoryStore));
            if (entity == null)
                return false;

            UpdateTasksWithDeletedCategory(id);

            return base.Delete(id);
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation($"Начало работы метода {nameof(Delete)} класса {nameof(CategoryStore)}");

            var entity = CheckEntityExist(id, nameof(Delete), nameof(CategoryStore));
            if (entity == null)
                return false;

            UpdateTasksWithDeletedCategory(id);

            return await base.DeleteAsync(id);
        }

        private void UpdateTasksWithDeletedCategory(int id)
        {
            using (_dbContext.Database.BeginTransaction())
            {
                var fallbackCategory = _dbContext.Categories
                        .Single(ct => ct.Id == _categoryFallbackService.DeletedCategory.Id);

                var tasksToUpdate = _dbContext.Tasks
                    .Include(t => t.Category)
                    .Where(t => t.Category.Id == id)
                    .ToList();

                foreach (var task in _dbContext.Tasks.Include(el => el.Category).Where(el => el.Category.Id == id).ToList())
                {
                    task.Category = fallbackCategory;
                    _dbContext.Tasks.Update(task);
                }

                _dbContext.SaveChanges();
                _dbContext.Database.CommitTransaction();
            }
        }
    }
}
