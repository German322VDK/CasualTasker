using CasualTasker.DTO.Base;
using CasualTasker.Infrastructure.ViewUpdaters;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace CasualTasker.Infrastructure.ObservableDbCollections
{
    public class ObservableDbCollection<TEntity> : IObservableDbCollection<TEntity> where TEntity : NamedEntity
    {
        protected readonly ILogger<ObservableDbCollection<TEntity>> _logger;
        protected IViewUpdater<TEntity> _viewUpdater;
        public ObservableDbCollection(ILogger<ObservableDbCollection<TEntity>> logger)
        {
            _logger = logger;
        }

        public ICollectionView ViewEntities =>
            _viewUpdater.ViewEntities;
        public IEnumerable<TEntity> Entities => _viewUpdater.Entities;

        public TEntity First => _viewUpdater.First;

        public virtual TEntity DefaultDeletedEntity => null;


        public virtual bool Add(TEntity namedEntity)
        {
            _logger.LogInformation($"Выполнение операции {nameof(Add)} в классе {nameof(ObservableDbCollection<TEntity>)}");

            if (namedEntity == null || EntityExists(namedEntity) || string.IsNullOrWhiteSpace(namedEntity.Name))
            {
                _logger.LogInformation($"Объект {namedEntity?.ToString()} не существует или у объекта отсутсвует поле Name " +
                    $"или объект уже есть в БД в операции {nameof(Add)} в классе {nameof(ObservableDbCollection<TEntity>)}");
                return false;
            }

            _viewUpdater.Add(namedEntity);
            _logger.LogInformation($"Операция {nameof(Add)} в классе {nameof(ObservableDbCollection<TEntity>)} выполнена успешно");
            return true;
        }

        public virtual bool Delete(TEntity entity)
        {
            _logger.LogInformation($"Выполнение операции {nameof(Delete)} в классе {nameof(ObservableDbCollection<TEntity>)}");

            if (!EntityExists(entity))
            {
                _logger.LogInformation($"Не найден объект {entity?.ToString()} в операции {nameof(Delete)} в классе {nameof(ObservableDbCollection<TEntity>)}");
                return false;
            }

            _viewUpdater.Delete(entity.Id);
            _logger.LogInformation($"Операция {nameof(Delete)} в классе {nameof(ObservableDbCollection<TEntity>)} выполнена успешно");
            return true;
        }

        public bool EntityExists(TEntity entity)
        {
            return Get(entity) != null;
        }


        public TEntity Get(int id) =>
            _viewUpdater.Get(id);
        public TEntity Get(TEntity entity) =>
            _viewUpdater.Get(entity);

        public virtual bool Update(TEntity namedEntity, bool isDBUpdated = true)
        {
            _logger.LogInformation($"Выполнение операции {nameof(Update)} в классе {nameof(ObservableDbCollection<TEntity>)}");

            if (!EntityExists(namedEntity) || string.IsNullOrWhiteSpace(namedEntity.Name))
            {
                _logger.LogInformation($"Объект {namedEntity?.ToString()} не существует или у объекта отсутсвует поле Name " +
                    $"в операции {nameof(Add)} в классе {nameof(ObservableDbCollection<TEntity>)}");
                return false;
            }

            if (isDBUpdated)
            {
                _logger.LogInformation($"Обновление представления и БД в операции {nameof(Update)} в классе {nameof(ObservableDbCollection<TEntity>)}");

                

                _viewUpdater.Update(namedEntity);
            }
            else
            {
                _logger.LogInformation($"Обновление представления без БД в операции {nameof(Update)} в классе {nameof(ObservableDbCollection<TEntity>)}");
                _viewUpdater.Update(namedEntity);
            }
            _logger.LogInformation($"Операция {nameof(Update)} в классе {nameof(ObservableDbCollection<TEntity>)} выполнена успешно");
            return true;
        }

        public void BatchUpdate(IEnumerable<TEntity> entities)
        {
            _logger.LogInformation($"Выполнение операции {nameof(BatchUpdate)} в классе {nameof(ObservableDbCollection<TEntity>)}");

            foreach (var entity in entities)
            {
                Update(entity, false);
            }
            _logger.LogInformation($"Операция {nameof(Update)} в классе {nameof(ObservableDbCollection<TEntity>)} выполнена успешно");
        }

        public void UpdateFromDB()
        {
            _logger.LogInformation($"Выполнение операции {nameof(UpdateFromDB)} в классе {nameof(ObservableDbCollection<TEntity>)}");
            _viewUpdater.UpdateFromDB(null);
            _logger.LogInformation($"Операция {nameof(UpdateFromDB)} в классе {nameof(ObservableDbCollection<TEntity>)} выполнена успешно");
        }
    }
}
