using CasualTasker.DTO;
using CasualTasker.Services.Fallbacks;
using CasualTasker.Services.Stores;
using Microsoft.Extensions.Logging;

namespace CasualTasker.Infrastructure.ObservableDbCollections
{
    /// <summary>
    /// Repository for managing data access for categories and tasks.
    /// Synchronizes data between the UI and the database.
    /// </summary>
    public class DataRepository
    {
        private CategoryTaskSynchronizer _categoryTaskSynchronizer;
        private readonly ILogger<DataRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the DataRepository class.
        /// </summary>
        /// <param name="categoryStore">Store for category data.</param>
        /// <param name="taskStore">Store for task data.</param>
        /// <param name="categoryFallbackService">Service for handling category fallbacks.</param>
        /// <param name="logger">Logger for tracking operations.</param>
        /// <param name="categoriesLogger">Logger for category collection operations.</param>
        /// <param name="tasksLogger">Logger for task collection operations.</param>
        public DataRepository(
            IStore<CategoryDTO> categoryStore,
            IStore<TaskDTO> taskStore,
            ICategoryFallbackService categoryFallbackService,
            ILogger<DataRepository> logger,
            ILogger<ObservableDbCollection<CategoryDTO>> categoriesLogger,
            ILogger<ObservableDbCollection<TaskDTO>> tasksLogger)
        {
            _logger = logger;

            var categories = new ObservableCategoryDbCollection(categoryStore, categoriesLogger, categoryFallbackService);
            Categories = categories;
            Tasks = new ObservableTaskDbCollection(taskStore, tasksLogger);

            _categoryTaskSynchronizer = new CategoryTaskSynchronizer(Tasks, categories);
            _categoryTaskSynchronizer.Synchronize();
        }

        /// <summary>
        /// Gets the collection of categories.
        /// </summary>
        public IObservableDbCollection<CategoryDTO> Categories { get; private set; }

        /// <summary>
        /// Gets the collection of tasks.
        /// </summary>
        public IObservableDbCollection<TaskDTO> Tasks { get; private set; }

        /// <summary>
        /// Updates the data from the database for both categories and tasks.
        /// </summary>
        public void UpdateDataFromDB()
        {
            _logger.LogInformation($"Выполнение операции {nameof(UpdateDataFromDB)} в классе {nameof(DataRepository)}");

            Categories.UpdateFromDB();
            Tasks.UpdateFromDB();
        }
    }
}
