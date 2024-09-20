using CasualTasker.DTO;
using CasualTasker.Infrastructure.Commands;
using CasualTasker.Infrastructure.ObservableDbCollections;
using CasualTasker.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace CasualTasker.ViewModels
{
    public class EditTaskPageViewModel : ViewModelBase
    {
        private readonly ILogger<EditTaskPageViewModel> _logger;

        private readonly DataRepository _repository;
        private IObservableDbCollection<TaskDTO> _tasksData;
        private IObservableDbCollection<CategoryDTO> _categoriesData;

        private CategoryDTO? _selectedCategory;
        private TaskDTO? _selectedTask;
        private TaskDTO? _editTask = new();

        private LambdaCommand? _addTaskCommand;
        private LambdaCommand? _updateTaskCommand;
        private LambdaCommand? _deleteTaskCommand;

        public EditTaskPageViewModel(DataRepository repository, ILogger<EditTaskPageViewModel> logger)
        {
            _logger = logger;
            _repository = repository;
            _tasksData = _repository.Tasks;
            _categoriesData = _repository.Categories;
            Tasks = _tasksData.ViewEntities;
            Categories = _categoriesData.ViewEntities;
        }


        public ICollectionView Tasks { get; set; }
        public ICollectionView Categories { get; set; }

        public CategoryDTO SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                Set(ref _selectedCategory, value);

                if (_selectedCategory == null)
                    return;

                if (EditTask != null)
                    EditTask.Category = _selectedCategory;

                if (SelectedTask != null)
                    SelectedTask.Category = _selectedCategory;
            }
        }
        public TaskDTO SelectedTask
        {
            get => _selectedTask;
            set
            {
                Set(ref _selectedTask, value);

                if (_selectedTask == null)
                    return;

                EditTask = (TaskDTO)SelectedTask.Clone();

                var category = _categoriesData.Get(SelectedTask.Category.Id);
                if (category != null)
                    SelectedCategory = category;
            }
        }
        public TaskDTO EditTask
        {
            get => _editTask;
            set => Set(ref _editTask, value);
        }

        public LambdaCommand AddTaskCommand => _addTaskCommand ??=
            new LambdaCommand(OnAddTaskCommandExecuted, CanAddTaskCommandExecute);
        public LambdaCommand UpdateTaskCommand => _updateTaskCommand ??=
            new LambdaCommand(OnUpdateTaskCommandExecuted, CanUpdateTaskCommandExecute);
        public LambdaCommand DeleteTaskCommand => _deleteTaskCommand ??=
            new LambdaCommand(OnDeleteTaskCommandExecuted, CanDeleteTaskCommandExecute);

        private bool CanAddTaskCommandExecute(object p) => EditTask != null && !string.IsNullOrWhiteSpace(EditTask.Name);
        private void OnAddTaskCommandExecuted(object p)
        {
            _logger.LogInformation($"Выполнение операции {nameof(OnAddTaskCommandExecuted)} в классе {nameof(EditTaskPageViewModel)}");

            TaskDTO newTask = new TaskDTO
            {
                Name = EditTask.Name,
                Status = EditTask.Status,
                Description = EditTask.Description,
                DueDate = EditTask.DueDate,
                Category = EditTask.Category,
            };
            _tasksData.Add(newTask);
        }

        private bool CanUpdateTaskCommandExecute(object p) =>
            EditTask != null && !string.IsNullOrWhiteSpace(EditTask.Name) && SelectedTask != null;
        private void OnUpdateTaskCommandExecuted(object p)
        {
            _logger.LogInformation($"Выполнение операции {nameof(OnUpdateTaskCommandExecuted)} в классе {nameof(EditTaskPageViewModel)}");

            _tasksData.Update(EditTask);
        }

        private bool CanDeleteTaskCommandExecute(object p) =>
            SelectedTask != null;
        private void OnDeleteTaskCommandExecuted(object p)
        {
            _logger.LogInformation($"Выполнение операции {nameof(OnDeleteTaskCommandExecuted)} в классе {nameof(EditTaskPageViewModel)}");

            _tasksData.Delete(EditTask);
        }
    }
}
