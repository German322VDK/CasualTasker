using CasualTasker.DTO;
using CasualTasker.Infrastructure.Commands;
using CasualTasker.Infrastructure.ObservableDbCollections;
using CasualTasker.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace CasualTasker.ViewModels
{
    public class EditCategoryPageViewModel : ViewModelBase
    {
        private readonly ILogger<EditCategoryPageViewModel> _logger;

        private readonly DataRepository _repository;
        private IObservableDbCollection<CategoryDTO> _categoriesData;

        private CategoryDTO? _selectedCategory;
        private CategoryDTO _editCategory = new();

        private LambdaCommand? _addCategoryCommand;
        private LambdaCommand? _updateCategoryCommand;
        private LambdaCommand? _deleteCategoryCommand;

        public EditCategoryPageViewModel(DataRepository repository, ILogger<EditCategoryPageViewModel> logger)
        {
            _logger = logger;
            _repository = repository;
            _categoriesData = _repository.Categories;
            Categories = _categoriesData.ViewEntities;
        }

        public CategoryDTO SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                Set(ref _selectedCategory, value);
                if (value == null)
                    return;

                EditCategory = (CategoryDTO)SelectedCategory.Clone();
            }
        }
        public CategoryDTO EditCategory
        {
            get => _editCategory;
            set
            {
                Set(ref _editCategory, value);
            }
        }

        public ICollectionView Categories { get; set; }

        public LambdaCommand AddCategoryCommand => _addCategoryCommand ??=
            new LambdaCommand(OnAddCategoryCommandExecuted, CanAddCategoryCommandExecute);

        public LambdaCommand UpdateCategoryCommand => _updateCategoryCommand ??=
            new LambdaCommand(OnUpdateCategoryCommandExecuted, CanUpdateCategoryCommandExecute);

        public LambdaCommand DeleteCategoryCommand => _deleteCategoryCommand ??=
            new LambdaCommand(OnDeleteCategoryCommandExecuted, CanDeleteCategoryCommandExecute);

        private bool CanAddCategoryCommandExecute(object p) => EditCategory != null && !string.IsNullOrWhiteSpace(EditCategory.Name);
        private void OnAddCategoryCommandExecuted(object p)
        {
            _logger.LogInformation($"Выполнение операции {nameof(OnAddCategoryCommandExecuted)} в классе {nameof(EditCategoryPageViewModel)}");

            var category = new CategoryDTO
            {
                Name = EditCategory.Name,
                Color = EditCategory.Color,
            };
            _categoriesData.Add(category);
            EditCategory = new();
        }

        private bool CanUpdateCategoryCommandExecute(object p) => SelectedCategory != null && EditCategory != null && !string.IsNullOrWhiteSpace(EditCategory.Name);
        private void OnUpdateCategoryCommandExecuted(object p)
        {
            _logger.LogInformation($"Выполнение операции {nameof(OnUpdateCategoryCommandExecuted)} в классе {nameof(EditCategoryPageViewModel)}");

            var category = (CategoryDTO)EditCategory.Clone();
            category.Id = SelectedCategory.Id;
            _categoriesData.Update(category);
            EditCategory = new();
        }

        private bool CanDeleteCategoryCommandExecute(object p) => SelectedCategory != null;
        private void OnDeleteCategoryCommandExecuted(object p)
        {
            _logger.LogInformation($"Выполнение операции {nameof(OnDeleteCategoryCommandExecuted)} в классе {nameof(EditCategoryPageViewModel)}");

            var deletedCategory = (CategoryDTO)SelectedCategory.Clone();
            SelectedCategory = _categoriesData.DefaultDeletedEntity;
            _categoriesData.Delete(deletedCategory);
            EditCategory = new();
        }
    }
}
