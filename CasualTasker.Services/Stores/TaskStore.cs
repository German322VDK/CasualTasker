using CasualTasker.Database.Context;
using CasualTasker.DTO;
using CasualTasker.Services.Fallbacks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CasualTasker.Services.Stores
{
    /// <summary>
    /// Represents a store for managing task entities.
    /// </summary>
    public class TaskStore : DbStore<TaskDTO>
    {
        private readonly ILogger<TaskStore> _logger;
        private readonly CasualTaskerDbContext _dbContext;
        private readonly ICategoryFallbackService _categoryFallbackService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskStore"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="categoryFallbackService">The service for managing fallback categories.</param>
        /// <param name="logger">The logger for logging information and errors.</param>
        public TaskStore(
            CasualTaskerDbContext dbContext,
            ICategoryFallbackService categoryFallbackService,
            ILogger<TaskStore> logger) : base(dbContext, logger)
        {
            _dbContext = dbContext;
            _categoryFallbackService = categoryFallbackService;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new task to the store.
        /// </summary>
        /// <param name="item">The task to add.</param>
        /// <returns>The added task.</returns>
        public override TaskDTO Add(TaskDTO item)
        {
            _logger.LogInformation($"Начало работы метода {nameof(Add)} класса {nameof(TaskStore)}");

            ValidationEntityWithException(item, nameof(Add), nameof(TaskStore));

            item = SetCategory(item, nameof(Add), nameof(TaskStore));

            return base.Add(item);
        }

        /// <summary>
        /// Asynchronously adds a new task to the store.
        /// </summary>
        /// <param name="item">The task to add.</param>
        /// <returns>A task that represents the asynchronous operation, containing the added task.</returns>
        public override async Task<TaskDTO> AddAsync(TaskDTO item)
        {
            _logger.LogInformation($"Начало работы метода {nameof(AddAsync)} класса {nameof(TaskStore)}");

            ValidationEntityWithException(item, nameof(Add), nameof(TaskStore));

            item = SetCategory(item, nameof(Add), nameof(TaskStore));

            return await base.AddAsync(item);
        }

        /// <summary>
        /// Retrieves all tasks from the store, including their categories.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TaskDTO}"/> representing the collection of tasks.</returns>
        public override IQueryable<TaskDTO> GetAll() =>
            _dbContext.Tasks.Include(el => el.Category);

        /// <summary>
        /// Retrieves a task by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the task.</param>
        /// <returns>The task with the specified identifier, or null if not found.</returns>
        public override TaskDTO GetById(int id) =>
            GetAll().FirstOrDefault(el => el.Id == id);

        /// <summary>
        /// Sets the category for the task.
        /// </summary>
        /// <param name="item">The task for which to set the category.</param>
        /// <param name="methodName">The name of the method where the category is being set.</param>
        /// <param name="className">The name of the class where the category is being set.</param>
        /// <returns>The task with the assigned category.</returns>
        private TaskDTO SetCategory(TaskDTO item, string methodName, string className)
        {
            if (item.Category == null)
            {
                _logger.LogInformation($"Категория элемента равна null поэтому её нужно инициализировать категорией по умолчанию" +
                    $" в методе {methodName} класса {className}");
                item.Category = _categoryFallbackService.CommonCategory;
            }
            else
            {
                item.Category = _dbContext.Categories.FirstOrDefault(el => el.Id == item.Category.Id);
            }

            return item;
        }
    }
}
