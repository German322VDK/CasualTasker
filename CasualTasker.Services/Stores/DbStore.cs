using CasualTasker.Database.Context;
using CasualTasker.DTO.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace CasualTasker.Services.Stores
{
    /// <summary>
    /// Abstract base class for a database storage implementation for entities of type <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity, which must be derived from <see cref="NamedEntity"/>.</typeparam>
    public abstract class DbStore<TEntity> : IStore<TEntity> where TEntity : NamedEntity
    {
        private readonly ILogger<DbStore<TEntity>> _logger;
        private readonly DbSet<TEntity> _set;
        private readonly CasualTaskerDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbStore{TEntity}"/> class.
        /// </summary>
        /// <param name="dbContext">The database context to be used.</param>
        /// <param name="logger">The logger for logging operations.</param>
        protected DbStore(CasualTaskerDbContext dbContext, ILogger<DbStore<TEntity>> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            _set = _dbContext.Set<TEntity>();
        }

        /// <summary>
        /// Asynchronously adds a new entity to the store.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        /// <returns>The added entity.</returns>
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

        /// <summary>
        /// Adds a new entity to the store.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
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

        /// <summary>
        /// Deletes an entity from the store by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns><c>true</c> if the entity was successfully deleted; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Asynchronously deletes an entity from the store by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains <c>true</c> if the entity was successfully deleted; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Retrieves all entities from the store as a queryable collection.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> of entities.</returns>
        public virtual IQueryable<TEntity> GetAll() =>
            _set.AsNoTracking();

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>The entity with the specified identifier.</returns>
        public virtual TEntity GetById(int id) =>
            GetAll().FirstOrDefault(el => el.Id == id);

        /// <summary>
        /// Updates an existing entity in the store.
        /// </summary>
        /// <param name="item">The entity to update.</param>
        /// <returns>The updated entity.</returns>
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

        /// <summary>
        /// Asynchronously updates an existing entity in the store.
        /// </summary>
        /// <param name="item">The entity to update.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity.</returns>
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

        /// <summary>
        /// Checks if an entity exists in the store by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <param name="methodName">The name of the method calling this method.</param>
        /// <param name="className">The name of the class calling this method.</param>
        /// <returns>The existing entity if found; otherwise, <c>null</c>.</returns>
        protected TEntity CheckEntityExist(int id, string methodName, string className)
        {
            TEntity? existingItem = GetById(id);
            if (existingItem == null)
                _logger.LogInformation($"Объект id:{id} не существует в БД в методе {methodName} класса {className}");
            else
                _logger.LogInformation($"Объект {existingItem} существует в БД в методе {methodName} класса {className}");
            return existingItem;
        }

        /// <summary>
        /// Validates the entity and throws an exception if it is invalid.
        /// </summary>
        /// <param name="item">The entity to validate.</param>
        /// <param name="methodName">The name of the method calling this method.</param>
        /// <param name="className">The name of the class calling this method.</param>
        protected void ValidationEntityWithException(TEntity item, string methodName, string className)
        {
            if (item == null || (item is NamedEntity namedEntity && string.IsNullOrWhiteSpace(namedEntity.Name)))
            {
                _logger.LogError($"Параметр {nameof(item)} равен {null} или item.Name null/empty в методе {methodName} класса {className}");
                throw new ArgumentNullException("", $"Параметр item=null или item.Name=null");
            }
        }

        /// <summary>
        /// Clears the change tracker to prevent memory issues.
        /// </summary>
        protected void ClearTracker() =>
            _dbContext.ChangeTracker.Clear();
    }
}
