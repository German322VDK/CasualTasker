using CasualTasker.Database.Context;
using CasualTasker.DTO;
using CasualTasker.Services.Fallbacks;
using CasualTasker.Services.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CasualTasker.Infrastructure.DbInitializers
{
    /// <summary>
    /// Initializes the CasualTasker database with default categories and tasks if none exist.
    /// </summary>
    public class CasualTaskerDbInitializer
    {
        private readonly ILogger _logger;
        private readonly CasualTaskerDbContext _dbContext;
        private readonly IStore<CategoryDTO> _categoriesStore;
        private readonly IStore<TaskDTO> _tasksStore;
        private readonly ICategoryFallbackService _categoryFallbackService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CasualTaskerDbInitializer"/> class.
        /// </summary>
        /// <param name="logger">The logger used for logging initialization process.</param>
        /// <param name="dbContext">The database context for the application.</param>
        /// <param name="categoriesStore">The store for managing category data.</param>
        /// <param name="tasksStore">The store for managing task data.</param>
        /// <param name="categoryFallbackService">The service providing fallback categories for deleted tasks.</param>
        public CasualTaskerDbInitializer(
            ILogger<CasualTaskerDbInitializer> logger, 
            CasualTaskerDbContext dbContext,
            IStore<CategoryDTO> categoriesStore,
            IStore<TaskDTO> tasksStore,
            ICategoryFallbackService categoryFallbackService
            )
        {
            _logger = logger;
            _dbContext = dbContext;
            _categoriesStore = categoriesStore;
            _tasksStore = tasksStore;
            _categoryFallbackService = categoryFallbackService;
        }

        /// <summary>
        /// Performs the database migration and initializes the categories and tasks if they do not exist.
        /// </summary>
        /// <returns>A task that represents the asynchronous initialization operation.</returns>
        public async Task InitializeAsync()
        {
            _logger.LogInformation($"Выполнение операции {nameof(InitializeAsync)} в классе {nameof(CasualTaskerDbInitializer)}");
            _dbContext.Database.Migrate();

            await InitializeCagetoriesAsync();
            await InitializeTasksAsync();
        }

        /// <summary>
        /// Initializes the default categories in the database if none exist.
        /// </summary>
        /// <returns>A task that represents the asynchronous category initialization operation.</returns>
        private async Task InitializeCagetoriesAsync()
        {
            _logger.LogInformation($"Выполнение операции {nameof(InitializeCagetoriesAsync)} в классе {nameof(CasualTaskerDbInitializer)}");
            if (await _dbContext.Categories.AnyAsync())
            {
                _logger.LogInformation($"Проверка наличия {nameof(_dbContext.Categories)}  " +
                    $"в операции {nameof(InitializeCagetoriesAsync)} в классе {nameof(CasualTaskerDbInitializer)}");
                return;
            }

            CategoryDTO deletedCategory = new CategoryDTO
            {
                Name = _categoryFallbackService.DeletedCategoryName,
            };

            _logger.LogInformation($"Добавление {deletedCategory} в операции {nameof(InitializeCagetoriesAsync)}" +
                $" в классе {nameof(CasualTaskerDbInitializer)}");
            await _categoriesStore.AddAsync(deletedCategory);

            CategoryDTO newCategory = new CategoryDTO
            {
                Name = _categoryFallbackService.CommonCategoryName,
            };

            _logger.LogInformation($"Добавление {newCategory} в операции {nameof(InitializeCagetoriesAsync)} " +
                $"в классе {nameof(CasualTaskerDbInitializer)}");
            await _categoriesStore.AddAsync(newCategory);
        }

        /// <summary>
        /// Initializes the default tasks in the database if none exist.
        /// </summary>
        /// <returns>A task that represents the asynchronous task initialization operation.</returns>
        private async Task InitializeTasksAsync()
        {
            _logger.LogInformation($"Выполнение операции {nameof(InitializeTasksAsync)} в классе {nameof(CasualTaskerDbInitializer)}");
            if (await _dbContext.Tasks.AnyAsync())
            {
                _logger.LogInformation($"Проверка наличия {nameof(_dbContext.Tasks)}  " +
                    $"в операции {nameof(InitializeTasksAsync)} в классе {nameof(CasualTaskerDbInitializer)}");
                return;
            }

            CategoryDTO category = _categoryFallbackService.CommonCategory;

            TaskDTO newTask = new TaskDTO
            {
                Name = "Начало",
                Description = "Начальная задача созданная при создании БД",
                DueDate = DateTime.Now,
                Status = CasualTaskStatus.InProgress,
                Category = category
            };

            _logger.LogInformation($"Добавление {newTask} в операции {nameof(InitializeCagetoriesAsync)} " +
                $"в классе {nameof(CasualTaskerDbInitializer)}");
            await _tasksStore.AddAsync(newTask);
        }
    }
}
