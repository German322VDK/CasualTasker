using CasualTasker.DTO;
using CasualTasker.Infrastructure.Commands;
using CasualTasker.Infrastructure.ObservableDbCollections;
using CasualTasker.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace CasualTasker.ViewModels
{
    /// <summary>
    /// ViewModel for the main page of the application.
    /// Provides filtering functionality for tasks by category,
    /// status, and date, as well as managing task selection and deletion.
    /// </summary>
    public class MainPageViewModel : ViewModelBase
    {
        private readonly ILogger<MainPageViewModel> _logger;

        private readonly DataRepository _repository;
        private readonly IObservableDbCollection<CategoryDTO> _categoriesData;
        private readonly IObservableDbCollection<TaskDTO> _tasksData;

        private bool _isUsedCategoryFilter;
        private bool _isUsedStatusFilter;
        private bool _isUsedDateFilter;

        private CasualTaskStatus _searchStatus;
        private string _searchPhrase = string.Empty;

        private DateTime _selectedDate = DateTime.Now;
        private CategoryDTO? _selectedCategory;
        private TaskDTO? _selectedTask;

        private LambdaCommand? _deleteTaskCommand;
        private LambdaCommand? _downloadDataCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageViewModel"/> class.
        /// </summary>
        /// <param name="dataRepository">Instance of DataRepository for data access.</param>
        /// <param name="logger">Logger for tracking actions.</param>
        public MainPageViewModel(DataRepository dataRepository, ILogger<MainPageViewModel> logger)
        {
            _repository = dataRepository;
            _logger = logger;
            _categoriesData = _repository.Categories;
            _tasksData = _repository.Tasks;

            Tasks = _tasksData.ViewEntities;
            Categories = _categoriesData.ViewEntities;

            Tasks.Filter = TasksFilter;
        }

        /// <summary>
        /// Gets the collection of tasks for display in the UI.
        /// </summary>
        public ICollectionView Tasks { get; private set; }
        /// <summary>
        /// Gets the collection of categories for display in the UI.
        /// </summary>
        public ICollectionView Categories { get; private set; }

        /// <summary>
        /// Selected category for filtering tasks.
        /// </summary>
        public CategoryDTO? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                Set(ref _selectedCategory, value);
                Tasks.Refresh();
            }
        }
        /// <summary>
        /// Selected task for operations like deletion or editing.
        /// </summary>
        public TaskDTO? SelectedTask
        {
            get => _selectedTask;
            set => Set(ref _selectedTask, value);
        }
        /// <summary>
        /// Selected date for filtering tasks by due date.
        /// </summary>
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                Set(ref _selectedDate, value);

                Tasks.Refresh();
            }
        }

        /// <summary>
        /// Search phrase for filtering tasks by name.
        /// </summary>
        public string SearchPhrase
        {
            get => _searchPhrase;
            set
            {
                Set(ref _searchPhrase, value);

                Tasks.Refresh();
            }
        }
        /// <summary>
        /// Status used for filtering tasks.
        /// </summary>
        public CasualTaskStatus SearchStatus
        {
            get => _searchStatus;
            set
            {
                Set(ref _searchStatus, value);
                Tasks.Refresh();
            }
        }

        /// <summary>
        /// Indicates whether the category filter is being used.
        /// </summary>
        public bool IsUsedCategoryFilter
        {
            get => _isUsedCategoryFilter;
            set
            {
                Set(ref _isUsedCategoryFilter, value);
                Tasks.Refresh();
            }
        }
        public bool IsUsedDateFilter
        {
            get => _isUsedDateFilter;
            set
            {
                Set(ref _isUsedDateFilter, value);
                Tasks.Refresh();
            }
        }
        /// <summary>
        /// Indicates whether the date filter is being used.
        /// </summary>
        public bool IsUsedStatusFilter
        {
            get => _isUsedStatusFilter;
            set
            {
                Set(ref _isUsedStatusFilter, value);
                Tasks.Refresh();
            }
        }

        /// <summary>
        /// Gets the command for deleting a selected task.
        /// </summary>
        public LambdaCommand DeleteTaskCommand => _deleteTaskCommand ??=
           new LambdaCommand(OnDeleteTaskCommandExecuted, CanDeleteTaskCommandExecute);
        /// <summary>
        /// Gets the command for download data from db.
        /// </summary>
        public LambdaCommand DownloadDataCommand => _downloadDataCommand ??=
            new LambdaCommand(OnDownloadDataCommandExecuted, CanDownloadDataCommandExecute);

        private bool CanDeleteTaskCommandExecute(object p) => SelectedTask != null;
        private void OnDeleteTaskCommandExecuted(object p)
        {
            _logger.LogInformation($"Выполнение операции {nameof(OnDeleteTaskCommandExecuted)} в классе {nameof(MainWindowViewModel)}");

            _tasksData.Delete(SelectedTask);
            SelectedTask = _tasksData.First;
        }

        private bool CanDownloadDataCommandExecute(object p) => true;
        private void OnDownloadDataCommandExecuted(object p)
        {
            _logger.LogInformation($"Выполнение операции {nameof(OnDownloadDataCommandExecuted)} в классе {nameof(MainWindowViewModel)}");

            _repository.UpdateDataFromDB();
        }

        private bool TasksFilter(object el)
        {
            if (el is TaskDTO task)
            {
                bool checkPrase = string.IsNullOrWhiteSpace(SearchPhrase) || task.Name.Contains(SearchPhrase, StringComparison.InvariantCultureIgnoreCase);
                bool checkDate = !IsUsedDateFilter || task.DueDate.Date == SelectedDate.Date;
                bool checkCategory = !IsUsedCategoryFilter || task.Category.Id == SelectedCategory.Id;
                bool checkStatus = !IsUsedStatusFilter || task.Status == SearchStatus;
                return checkPrase && checkDate && checkCategory && checkStatus;
            }

            return false;
        }
    }
}
