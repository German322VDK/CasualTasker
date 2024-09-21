using CasualTasker.Database.Context;
using CasualTasker.DTO.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace CasualTasker.Services.Stores
{
    public abstract class DbStore<TEntity> : IStore<TEntity> where TEntity : NamedEntity
    {
        private readonly ILogger<DbStore<TEntity>> _logger;
        private readonly DbSet<TEntity> _set;
        private readonly CasualTaskerDbContext _dbContext;

        protected DbStore(CasualTaskerDbContext dbContext, ILogger<DbStore<TEntity>> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            _set = _dbContext.Set<TEntity>();
        }

        public virtual TEntity Add(TEntity item)
        {
            _logger.LogInformation($"Начало работы метода {nameof(Add)} класса {nameof(DbStore<TEntity>)}");

            ValidationEntityWithException(item, nameof(Add), nameof(DbStore<TEntity>));
            TEntity? existingItem = CheckEntityExist(item.Id, nameof(Add), nameof(DbStore<TEntity>));
            if (existingItem != null)
                return existingItem;

            EntityEntry<TEntity>? result = null;
            using (_dbContext.Database.BeginTransaction())
            {
                try
                {
                    result = _dbContext.Entry(item);
                    result.State = EntityState.Added;

                    _dbContext.SaveChanges();
                    _dbContext.Database.CommitTransaction();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Ошибка при добавлении объекта {item} в методе {nameof(Add)} класса {nameof(DbStore<TEntity>)}");
                    throw;
                }
                finally
                {
                    ClearTracker();
                }
            }
            _logger.LogInformation($"Объект {result.Entity} успешно добавлен в БД в методе {nameof(Add)} класса {nameof(DbStore<TEntity>)}");
            return GetById(result.Entity.Id);
        }

        public virtual async Task<TEntity> AddAsync(TEntity item)
        {
            _logger.LogInformation($"Начало работы метода {nameof(AddAsync)} класса {nameof(DbStore<TEntity>)}");

            ValidationEntityWithException(item, nameof(AddAsync), nameof(DbStore<TEntity>));
            TEntity? existingItem = CheckEntityExist(item.Id, nameof(AddAsync), nameof(DbStore<TEntity>));
            if (existingItem != null)
                return existingItem;

            EntityEntry<TEntity>? result = null;
            await using (await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    result = _dbContext.Entry(item);
                    result.State = EntityState.Added;

                    await _dbContext.SaveChangesAsync();
                    await _dbContext.Database.CommitTransactionAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Ошибка при добавлении объекта {item} в методе {nameof(AddAsync)} класса {nameof(DbStore<TEntity>)}");
                    throw;
                }
                finally
                {
                    ClearTracker();
                }
            }
            _logger.LogInformation($"Объект {result.Entity} успешно добавлен в БД в методе {nameof(AddAsync)} класса {nameof(DbStore<TEntity>)}");
            return GetById(result.Entity.Id);
        }

        public virtual bool Delete(int id)
        {
            _logger.LogInformation($"Начало работы метода {nameof(Delete)} класса {nameof(DbStore<TEntity>)}");

            TEntity existingItem = CheckEntityExist(id, nameof(Delete), nameof(DbStore<TEntity>));
            if (existingItem == null)
                return false;

            using (_dbContext.Database.BeginTransaction())
            {
                try
                {
                    _dbContext.Entry(existingItem).State = EntityState.Deleted;
                    _dbContext.SaveChanges();
                    _dbContext.Database.CommitTransaction();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Ошибка при удалении объекта id: {id} в методе {nameof(Delete)} класса {nameof(DbStore<TEntity>)}");
                    throw;
                }
                finally
                {
                    ClearTracker();
                }
            }
            _logger.LogInformation($"Объект id:{id} успешно удалён из БД в методе {nameof(Delete)} класса {nameof(DbStore<TEntity>)}");
            return true;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            _logger.LogInformation($"Начало работы метода {nameof(DeleteAsync)} класса {nameof(DbStore<TEntity>)}");

            TEntity existingItem = CheckEntityExist(id, nameof(Delete), nameof(DbStore<TEntity>));
            if (existingItem == null)
                return false;

            await using (await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    _dbContext.Entry(existingItem).State = EntityState.Deleted;
                    await _dbContext.SaveChangesAsync();
                    await _dbContext.Database.CommitTransactionAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Ошибка при удалении объекта id: {id} в методе {nameof(DeleteAsync)} класса {nameof(DbStore<TEntity>)}");
                    throw;
                }
                finally
                {
                    ClearTracker();
                }
            }
            _logger.LogInformation($"Объект id:{id} успешно удалён из БД в методе {nameof(DeleteAsync)} класса {nameof(DbStore<TEntity>)}");
            return true;
        }

        public virtual IQueryable<TEntity> GetAll() =>
            _set.AsNoTracking();

        public virtual TEntity GetById(int id) =>
            GetAll().FirstOrDefault(el => el.Id == id);

        public virtual TEntity Update(TEntity item)
        {
            _logger.LogInformation($"Начало работы метода {nameof(Update)} класса {nameof(DbStore<TEntity>)}");

            ValidationEntityWithException(item, nameof(Update), nameof(DbStore<TEntity>));
            TEntity? existingItem = CheckEntityExist(item.Id, nameof(Update), nameof(DbStore<TEntity>));
            if (existingItem == null)
                return null;

            EntityEntry<TEntity> entity;
            using (_dbContext.Database.BeginTransaction())
            {
                try
                {
                    _dbContext.Attach(item);
                    entity = _dbContext.Entry(item);
                    entity.State = EntityState.Modified;
                    _dbContext.SaveChanges();
                    _dbContext.Database.CommitTransaction();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Ошибка при обновлении объекта: {item} в методе {nameof(Update)} класса {nameof(DbStore<TEntity>)}");
                    throw;
                }
                finally
                {
                    ClearTracker();
                }

            }
            _logger.LogInformation($"Объект {item} успешно обнавлён в БД в методе {nameof(Update)} класса {nameof(DbStore<TEntity>)}");
            return GetById(item.Id);
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity item)
        {
            _logger.LogInformation($"Начало работы метода {nameof(UpdateAsync)} класса {nameof(DbStore<TEntity>)}");

            ValidationEntityWithException(item, nameof(UpdateAsync), nameof(DbStore<TEntity>));
            TEntity? existingItem = CheckEntityExist(item.Id, nameof(UpdateAsync), nameof(DbStore<TEntity>));
            if (existingItem == null)
                return null;

            EntityEntry<TEntity> entity;
            await using (await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    _dbContext.Attach(item);
                    entity = _dbContext.Entry(item);
                    entity.State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();
                    await _dbContext.Database.CommitTransactionAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Ошибка при обновлении объекта: {item} в методе {nameof(Update)} класса {nameof(DbStore<TEntity>)}");
                    throw;
                }
                finally
                {
                    ClearTracker();
                }
            }
            _logger.LogInformation($"Объект {entity.Entity} успешно обнавлён в БД в методе {nameof(UpdateAsync)} класса {nameof(DbStore<TEntity>)}");
            return GetById(item.Id);
        }

        protected TEntity CheckEntityExist(int id, string methodName, string className)
        {
            TEntity? existingItem = GetById(id);
            if (existingItem == null)
                _logger.LogInformation($"Объект id:{id} не существует в БД в методе {methodName} класса {className}");
            else
                _logger.LogInformation($"Объект {existingItem} существует в БД в методе {methodName} класса {className}");
            return existingItem;
        }

        protected void ValidationEntityWithException(TEntity item, string methodName, string className)
        {
            if (item == null || (item is NamedEntity namedEntity && string.IsNullOrWhiteSpace(namedEntity.Name)))
            {
                _logger.LogError($"Параметр {nameof(item)} равен {null} или item.Name null/empty в методе {methodName} класса {className}");
                throw new ArgumentNullException("", $"Параметр item=null или item.Name=null");
            }
        }

        protected void ClearTracker() =>
            _dbContext.ChangeTracker.Clear();
    }
}
