using CasualTasker.Database.Context;
using CasualTasker.DTO;
using CasualTasker.Services.Fallbacks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CasualTasker.Services.Stores
{
    /// <summary>
    /// Represents a store for managing category entities.
    /// </summary>
    public sealed class CategoryStore : DbStore<CategoryDTO>
    {
        private readonly ILogger<CategoryStore> _logger;
        private readonly CasualTaskerDbContext _dbContext;
        private readonly ICategoryFallbackService _categoryFallbackService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryStore"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="logger">The logger for logging information and errors.</param>
        /// <param name="categoryFallbackService">The service for managing fallback categories.</param>
        public CategoryStore(
            CasualTaskerDbContext dbContext, 
            ILogger<CategoryStore> logger,
            ICategoryFallbackService categoryFallbackService) : base(dbContext, logger)
        {
            _categoryFallbackService = categoryFallbackService;
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Deletes a category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category to delete.</param>
        /// <returns>True if the category was successfully deleted; otherwise, false.</returns>
        public override bool Delete(int id)
        {
            _logger.LogInformation($"Начало работы метода {nameof(Delete)} класса {nameof(CategoryStore)}");

            var entity = CheckEntityExist(id, nameof(Delete), nameof(CategoryStore));
            if (entity == null)
                return false;

            UpdateTasksWithDeletedCategory(id);

            return base.Delete(id);
        }

        /// <summary>
        /// Asynchronously deletes a category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains true if the category was successfully deleted; otherwise, false.</returns>
        public override async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation($"Начало работы метода {nameof(Delete)} класса {nameof(CategoryStore)}");

            var entity = CheckEntityExist(id, nameof(Delete), nameof(CategoryStore));
            if (entity == null)
                return false;

            UpdateTasksWithDeletedCategory(id);

            return await base.DeleteAsync(id);
        }

        /// <summary>
        /// Updates tasks that are associated with a deleted category.
        /// </summary>
        /// <param name="id">The unique identifier of the deleted category.</param>
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
