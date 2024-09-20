using CasualTasker.DTO;
using Microsoft.Extensions.Logging;

namespace CasualTasker.Infrastructure.ObservableDbCollections
{
    public class DataRepository
    {
        private CategoryTaskSynchronizer _categoryTaskSynchronizer;
        private readonly ILogger<DataRepository> _logger;

        public DataRepository(
            ILogger<DataRepository> logger,
            ILogger<ObservableDbCollection<CategoryDTO>> categoriesLogger,
            ILogger<ObservableDbCollection<TaskDTO>> tasksLogger)
        {
            _logger = logger;

            var categories = new ObservableCategoryDbCollection(categoriesLogger);
            Categories = categories;
            Tasks = new ObservableTaskDbCollection(tasksLogger);

            _categoryTaskSynchronizer = new CategoryTaskSynchronizer(Tasks, categories);
            _categoryTaskSynchronizer.Synchronize();
        }

        public IObservableDbCollection<CategoryDTO> Categories { get; private set; }
        public IObservableDbCollection<TaskDTO> Tasks { get; private set; }

        public void UpdateDataFromDB()
        {
            _logger.LogInformation($"Выполнение операции {nameof(UpdateDataFromDB)} в классе {nameof(DataRepository)}");

            Categories.UpdateFromDB();
            Tasks.UpdateFromDB();
        }
    }
}
