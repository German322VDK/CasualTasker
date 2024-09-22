using CasualTasker.DTO.Base;

namespace CasualTasker.Services.Stores
{
    /// <summary>
    /// Defines a storage interface that supports both synchronous and asynchronous operations for entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity, which must be derived from <see cref="NamedEntity"/>.</typeparam>
    public interface IStore<TEntity> : IStoreSync<TEntity>, IStoreAsync<TEntity> where TEntity : NamedEntity
    {
        /// <summary>
        /// Retrieves all entities from the store as a queryable collection.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> of entities.</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>The entity with the specified identifier.</returns>
        TEntity GetById(int id);
    }

    /// <summary>
    /// Defines synchronous operations for a storage interface for entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity, which must be derived from <see cref="NamedEntity"/>.</typeparam>
    public interface IStoreSync<TEntity> where TEntity : NamedEntity
    {
        /// <summary>
        /// Adds a new entity to the store.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        /// <returns>The added entity.</returns>
        TEntity Add(TEntity item);

        /// <summary>
        /// Updates an existing entity in the store.
        /// </summary>
        /// <param name="item">The entity to update.</param>
        /// <returns>The updated entity.</returns>
        TEntity Update(TEntity item);

        /// <summary>
        /// Deletes an entity from the store by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns><c>true</c> if the entity was successfully deleted; otherwise, <c>false</c>.</returns>
        bool Delete(int id);
    }

    /// <summary>
    /// Defines asynchronous operations for a storage interface for entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity, which must be derived from <see cref="NamedEntity"/>.</typeparam>
    public interface IStoreAsync<TEntity> where TEntity : NamedEntity
    {
        /// <summary>
        /// Asynchronously adds a new entity to the store.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
        Task<TEntity> AddAsync(TEntity item);

        /// <summary>
        /// Asynchronously updates an existing entity in the store.
        /// </summary>
        /// <param name="item">The entity to update.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity.</returns>
        Task<TEntity> UpdateAsync(TEntity item);

        /// <summary>
        /// Asynchronously deletes an entity from the store by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains <c>true</c> if the entity was successfully deleted; otherwise, <c>false</c>.</returns>
        Task<bool> DeleteAsync(int id);
    }
}
