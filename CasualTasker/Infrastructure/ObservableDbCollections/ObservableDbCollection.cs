using CasualTasker.DTO.Base;
using CasualTasker.Infrastructure.ViewUpdaters;
using CasualTasker.Services.Stores;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CasualTasker.Infrastructure.ObservableDbCollections
{
    public class ObservableDbCollection<TEntity> : IObservableDbCollection<TEntity> where TEntity : NamedEntity
    {
        protected readonly ILogger<ObservableDbCollection<TEntity>> _logger;
        private IViewUpdater<TEntity> _viewUpdater;
        private readonly IStore<TEntity> _store;
        public ObservableDbCollection(IStore<TEntity> store, ILogger<ObservableDbCollection<TEntity>> logger)
        {
            _logger = logger;
            _store = store;
            _viewUpdater = new ViewUpdater<TEntity>(new ObservableCollection<TEntity>(_store.GetAll()));
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
            TEntity newEntity = _store.Add(namedEntity);
            if (newEntity == null)
            {
                _logger.LogInformation($"БД не смогла добавить объект {namedEntity.ToString()} в операции {nameof(Add)} в классе {nameof(ObservableDbCollection<TEntity>)}");
                return false;
            }
            else
            {
                _logger.LogInformation($"БД успешно добавила объект {namedEntity.ToString()} в операции {nameof(Add)} в классе {nameof(ObservableDbCollection<TEntity>)}");
            }

            _viewUpdater.Add(newEntity);
            _logger.LogInformation($"Операция {nameof(Add)} в классе {nameof(ObservableDbCollection<TEntity>)} выполнена успешно");
            return true;
        }

        public virtual bool Delete(TEntity namedEntity)
        {
            _logger.LogInformation($"Выполнение операции {nameof(Delete)} в классе {nameof(ObservableDbCollection<TEntity>)}");

            if (namedEntity == null || !EntityExists(namedEntity))
            {
                _logger.LogInformation($"Не найден объект {namedEntity?.ToString()} в операции {nameof(Delete)} в классе {nameof(ObservableDbCollection<TEntity>)}");
                return false;
            }

            bool result = _store.Delete(namedEntity.Id);

            if (!result)
            {
                _logger.LogInformation($"Не получилось удалить объект {namedEntity.ToString()} из БД в операции {nameof(Delete)} в классе {nameof(ObservableDbCollection<TEntity>)}");
                return false;
            }
            else
            {
                _logger.LogInformation($"БД успешно удалила объект {namedEntity.ToString()} в операции {nameof(Add)} в классе {nameof(ObservableDbCollection<TEntity>)}");
            }

            _viewUpdater.Delete(namedEntity.Id);
            _logger.LogInformation($"Операция {nameof(Delete)} в классе {nameof(ObservableDbCollection<TEntity>)} выполнена успешно");
            return true;
        }

        public bool EntityExists(TEntity entity) =>
            _store.GetById(entity.Id) != null;

        public TEntity Get(int id) =>
            _viewUpdater.Get(id);
        public TEntity Get(TEntity entity) =>
            _viewUpdater.Get(entity);

        public virtual bool Update(TEntity namedEntity, bool isDBUpdated = true)
        {
            _logger.LogInformation($"Выполнение операции {nameof(Update)} в классе {nameof(ObservableDbCollection<TEntity>)}");

            if (namedEntity == null || !EntityExists(namedEntity) || string.IsNullOrWhiteSpace(namedEntity.Name))
            {
                _logger.LogInformation($"Объект {namedEntity?.ToString()} не существует или у объекта отсутсвует поле Name " +
                    $"в операции {nameof(Update)} в классе {nameof(ObservableDbCollection<TEntity>)}");
                return false;
            }

            if (isDBUpdated)
            {
                _logger.LogInformation($"Обновление представления и БД в операции {nameof(Update)} в классе {nameof(ObservableDbCollection<TEntity>)}");

                TEntity updatedEntity = _store.Update(namedEntity);
                if (updatedEntity == null)
                {
                    _logger.LogInformation($"Не получилось обновить объект {namedEntity?.ToString()} из БД в операции {nameof(Delete)} в классе {nameof(ObservableDbCollection<TEntity>)}");
                    return false;
                }
                else
                {
                    _logger.LogInformation($"БД успешно обновила объект {namedEntity?.ToString()} в операции {nameof(Add)} в классе {nameof(ObservableDbCollection<TEntity>)}");
                }

                _viewUpdater.Update(updatedEntity);
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
            _viewUpdater.UpdateFromDB(_store.GetAll());
            _logger.LogInformation($"Операция {nameof(UpdateFromDB)} в классе {nameof(ObservableDbCollection<TEntity>)} выполнена успешно");
        }
    }
}
