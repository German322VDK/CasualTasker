using CasualTasker.DTO;
using CasualTasker.Services.Fallbacks;
using CasualTasker.Services.Stores;
using Microsoft.Extensions.Logging;

namespace CasualTasker.Infrastructure.ObservableDbCollections
{
    /// <summary>
    /// Represents a collection of category entities that synchronizes the database with the view.
    /// Extends <see cref="ObservableDbCollection{CategoryDTO}"/> to provide specific logic for categories,
    /// including handling fallback categories and firing events when categories are updated or deleted.
    /// </summary>
    public class ObservableCategoryDbCollection : ObservableDbCollection<CategoryDTO>
    {
        private readonly ICategoryFallbackService _categoryFallbackService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCategoryDbCollection"/> class.
        /// </summary>
        /// <param name="store">The store that handles category-related operations with the database.</param>
        /// <param name="logger">The logger used to log operations within the category collection.</param>
        /// <param name="categoryFallbackService">Service responsible for managing fallback categories.</param>
        public ObservableCategoryDbCollection(
            IStore<CategoryDTO> store, 
            ILogger<ObservableDbCollection<CategoryDTO>> logger, 
            ICategoryFallbackService categoryFallbackService) : base(store, logger)
        {
            _categoryFallbackService = categoryFallbackService;
        }

        /// <summary>
        /// Event triggered when a category is updated.
        /// </summary>
        public event Action<CategoryDTO> OnCategoryUpdated;

        /// <summary>
        /// Event triggered when a category is deleted.
        /// </summary>
        public event Action<CategoryDTO> OnCategoryDeleted;

        /// <summary>
        /// Gets the default category used for tasks when a category is deleted.
        /// </summary>
        public override CategoryDTO DefaultDeletedEntity =>
            _categoryFallbackService?.DeletedCategory;

        /// <summary>
        /// Deletes the specified category from the collection and the database.
        /// Triggers the <see cref="OnCategoryDeleted"/> event upon successful deletion.
        /// </summary>
        /// <param name="entity">The category to delete.</param>
        /// <returns><see langword="true"/> if the category was deleted successfully; otherwise, <see langword="false"/>.</returns>
        public override bool Delete(CategoryDTO entity)
        {
            _logger.LogInformation($"Выполнение операции {nameof(Delete)} в классе {nameof(ObservableCategoryDbCollection)}");

            if (entity == null || !EntityExists(entity))
            {
                _logger.LogInformation($"Не найден объект {entity?.ToString()} в операции {nameof(Delete)} в классе {nameof(ObservableCategoryDbCollection)}");
                return false;
            }

            if (_categoryFallbackService != null && _categoryFallbackService.DeletedCategory.Id == entity.Id)
            {
                _logger.LogInformation($"Попытка удалить:{nameof(_categoryFallbackService.DeletedCategory)} " +
                    $"в операции {nameof(Delete)} в классе {nameof(ObservableCategoryDbCollection)}");
                return false;
            }

            OnCategoryDeleted?.Invoke(entity);

            return base.Delete(entity);
        }

        /// <summary>
        /// Updates the specified category in the collection and the database.
        /// Triggers the <see cref="OnCategoryUpdated"/> event upon successful update.
        /// </summary>
        /// <param name="entity">The category to update.</param>
        /// <param name="isDBUpdated">Indicates whether the database should be updated.</param>
        /// <returns><see langword="true"/> if the update was successful; otherwise, <see langword="false"/>.</returns>
        public override bool Update(CategoryDTO entity, bool isDBUpdated = true)
        {
            var result = base.Update(entity, isDBUpdated);

            if (result)
                OnCategoryUpdated?.Invoke(entity);

            return result;
        }
    }
}
