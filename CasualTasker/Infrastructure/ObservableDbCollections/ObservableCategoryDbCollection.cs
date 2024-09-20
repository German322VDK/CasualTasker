using CasualTasker.Database.StaticData;
using CasualTasker.DTO;
using CasualTasker.Infrastructure.ViewUpdaters;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace CasualTasker.Infrastructure.ObservableDbCollections
{
    public class ObservableCategoryDbCollection : ObservableDbCollection<CategoryDTO>
    {
        public ObservableCategoryDbCollection(ILogger<ObservableDbCollection<CategoryDTO>> logger) : base(logger)
        {
            _viewUpdater = new ViewUpdater<CategoryDTO>(new ObservableCollection<CategoryDTO>(TestData.Categories));
        }

        public event Action<CategoryDTO> OnCategoryUpdated;
        public event Action<CategoryDTO> OnCategoryDeleted;

        public override CategoryDTO DefaultDeletedEntity => 
            First;

        public override bool Delete(CategoryDTO entity)
        {
            _logger.LogInformation($"Выполнение операции {nameof(Delete)} в классе {nameof(ObservableCategoryDbCollection)}");

            if (!EntityExists(entity))
            {
                _logger.LogInformation($"Не найден объект {entity?.ToString()} в операции {nameof(Delete)} в классе {nameof(ObservableCategoryDbCollection)}");
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
