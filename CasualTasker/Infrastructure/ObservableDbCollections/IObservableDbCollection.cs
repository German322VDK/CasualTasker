using CasualTasker.DTO.Base;
using System.ComponentModel;

namespace CasualTasker.Infrastructure.ObservableDbCollections
{
    /// <summary>
    /// Represents an observable collection that syncs both with the database and the view layer.
    /// It provides operations to manage entities in the database and automatically updates the view.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity, which must inherit from <see cref="NamedEntity"/>.</typeparam>
    public interface IObservableDbCollection<TEntity> where TEntity : NamedEntity
    {
        /// <summary>
        /// Gets the collection of entities as an <see cref="ICollectionView"/> for data binding in the view layer.
        /// </summary>
        public ICollectionView ViewEntities { get; }

        /// <summary>
        /// Gets an enumerable collection of all entities in the collection.
        /// </summary>
        public IEnumerable<TEntity> Entities { get; }

        /// <summary>
        /// Gets the first entity in the observable collection.
        /// </summary>
        public TEntity First { get; }

        /// <summary>
        /// Gets the default entity used when an entity is deleted. Typically used for assigning deleted entities to a fallback.
        /// </summary>
        public TEntity DefaultDeletedEntity { get; }

        /// <summary>
        /// Adds an entity to the collection and updates both the database and the view.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns><c>true</c> if the entity was successfully added; otherwise, <c>false</c>.</returns>
        public bool Add(TEntity entity);

        /// <summary>
        /// Deletes an entity from the collection and updates both the database and the view.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns><c>true</c> if the entity was successfully deleted; otherwise, <c>false</c>.</returns>
        public bool Delete(TEntity entity);

        /// <summary>
        /// Determines whether an entity exists in the collection.
        /// </summary>
        /// <param name="entity">The entity to check for existence.</param>
        /// <returns><c>true</c> if the entity exists in the collection; otherwise, <c>false</c>.</returns>
        public bool EntityExists(TEntity entity);

        /// <summary>
        /// Retrieves an entity from the collection by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to retrieve.</param>
        /// <returns>The entity with the specified ID, or <c>null</c> if not found.</returns>
        public TEntity Get(int id);

        /// <summary>
        /// Retrieves an entity from the collection by comparing it with an existing entity.
        /// </summary>
        /// <param name="entity">The entity to search for.</param>
        /// <returns>The matching entity from the collection, or <c>null</c> if not found.</returns>
        public TEntity Get(TEntity entity);

        /// <summary>
        /// Updates an entity in both the database and the collection, and refreshes the view.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="isDBUpdated">If set to <c>true</c>, updates the entity in the database; otherwise, only updates the view.</param>
        /// <returns><c>true</c> if the update was successful; otherwise, <c>false</c>.</returns>
        public bool Update(TEntity entity, bool isDBUpdated = true);

        /// <summary>
        /// Refreshes the collection from the database and updates the view accordingly.
        /// </summary>
        public void UpdateFromDB();

        /// <summary>
        /// Batch updates multiple entities in the collection and optionally in the database.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        public void BatchUpdate(IEnumerable<TEntity> entities);
    }
}
