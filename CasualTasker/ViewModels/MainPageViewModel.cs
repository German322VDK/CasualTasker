using CasualTasker.DTO;
using CasualTasker.Infrastructure.Commands;
using CasualTasker.Infrastructure.ObservableDbCollections;
using CasualTasker.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace CasualTasker.ViewModels
{
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
        public LambdaCommand? _downloadDataCommand;

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

        public ICollectionView Tasks { get; private set; }
        public ICollectionView Categories { get; private set; }

        public CategoryDTO? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                Set(ref _selectedCategory, value);
                Tasks.Refresh();
            }
        }
        public TaskDTO? SelectedTask
        {
            get => _selectedTask;
            set => Set(ref _selectedTask, value);
        }
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                Set(ref _selectedDate, value);

                Tasks.Refresh();
            }
        }

        public string SearchPhrase
        {
            get => _searchPhrase;
            set
            {
                Set(ref _searchPhrase, value);

                Tasks.Refresh();
            }
        }
        public CasualTaskStatus SearchStatus
        {
            get => _searchStatus;
            set
            {
                Set(ref _searchStatus, value);
                Tasks.Refresh();
            }
        }

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
        public bool IsUsedStatusFilter
        {
            get => _isUsedStatusFilter;
            set
            {
                Set(ref _isUsedStatusFilter, value);
                Tasks.Refresh();
            }
        }

        public LambdaCommand DeleteTaskCommand => _deleteTaskCommand ??=
           new LambdaCommand(OnDeleteTaskCommandExecuted, CanDeleteTaskCommandExecute);
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
