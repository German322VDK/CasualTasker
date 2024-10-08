﻿using CasualTasker.DTO;
using CasualTasker.Infrastructure.Commands;
using CasualTasker.Infrastructure.ObservableDbCollections;
using CasualTasker.ViewModels.Base;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace CasualTasker.ViewModels
{
    /// <summary>
    /// ViewModel for editing categories in the application.
    /// </summary>
    public sealed class EditCategoryPageViewModel : ViewModelBase
    {
        private readonly ILogger<EditCategoryPageViewModel> _logger;

        private readonly DataRepository _repository;
        private IObservableDbCollection<CategoryDTO> _categoriesData;

        private CategoryDTO? _selectedCategory;
        private CategoryDTO _editCategory = new();

        private LambdaCommand? _addCategoryCommand;
        private LambdaCommand? _updateCategoryCommand;
        private LambdaCommand? _deleteCategoryCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditCategoryPageViewModel"/> class.
        /// </summary>
        /// <param name="repository">The data repository for accessing categories.</param>
        /// <param name="logger">Logger for logging operations.</param>
        public EditCategoryPageViewModel(DataRepository repository, ILogger<EditCategoryPageViewModel> logger)
        {
            _logger = logger;
            _repository = repository;
            _categoriesData = _repository.Categories;
            Categories = _categoriesData.ViewEntities;
        }

        /// <summary>
        /// Gets or sets the currently selected category for editing.
        /// </summary>
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
        /// <summary>
        /// Gets or sets the category being edited.
        /// </summary>
        public CategoryDTO EditCategory
        {
            get => _editCategory;
            set
            {
                Set(ref _editCategory, value);
            }
        }

        /// <summary>
        /// Gets the collection of categories for display in the UI.
        /// </summary>
        public ICollectionView Categories { get; set; }

        /// <summary>
        /// Gets the command for adding a new category.
        /// </summary>
        public LambdaCommand AddCategoryCommand => _addCategoryCommand ??=
            new LambdaCommand(OnAddCategoryCommandExecuted, CanAddCategoryCommandExecute);
        /// <summary>
        /// Gets the command for updating the selected category.
        /// </summary>
        public LambdaCommand UpdateCategoryCommand => _updateCategoryCommand ??=
            new LambdaCommand(OnUpdateCategoryCommandExecuted, CanUpdateCategoryCommandExecute);
        /// <summary>
        /// Gets the command for deleting the selected category.
        /// </summary>
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
