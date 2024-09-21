using CasualTasker.DTO;
using CasualTasker.Services.Fallbacks;
using CasualTasker.Services.Stores;
using Microsoft.Extensions.Logging;

namespace CasualTasker.Infrastructure.ObservableDbCollections
{
    public class ObservableCategoryDbCollection : ObservableDbCollection<CategoryDTO>
    {
        private readonly ICategoryFallbackService _categoryFallbackService;
        public ObservableCategoryDbCollection(
            IStore<CategoryDTO> store, 
            ILogger<ObservableDbCollection<CategoryDTO>> logger, 
            ICategoryFallbackService categoryFallbackService) : base(store, logger)
        {
            _categoryFallbackService = categoryFallbackService;
        }

        public event Action<CategoryDTO> OnCategoryUpdated;
        public event Action<CategoryDTO> OnCategoryDeleted;

        public override CategoryDTO DefaultDeletedEntity =>
            _categoryFallbackService?.DeletedCategory;

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

        public override bool Update(CategoryDTO entity, bool isDBUpdated = true)
        {
            var result = base.Update(entity, isDBUpdated);

            if (result)
                OnCategoryUpdated?.Invoke(entity);

            return result;
        }
    }
}
