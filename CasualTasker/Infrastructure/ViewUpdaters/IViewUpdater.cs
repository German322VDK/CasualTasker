using CasualTasker.DTO.Base;
using System.ComponentModel;

namespace CasualTasker.Infrastructure.ViewUpdaters
{
    /// <summary>
    /// Provides an interface for managing and updating the view layer with entities.
    /// The view is updated automatically when entities are added, updated, or deleted.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity, which must inherit from <see cref="NamedEntity"/>.</typeparam>
    public interface IViewUpdater<TEntity> where TEntity : NamedEntity
    {
        /// <summary>
        /// Gets the first entity in the collection.
        /// </summary>
        TEntity First { get; }

        /// <summary>
        /// Gets a collection of entities as an <see cref="ICollectionView"/>, which is used for data binding in the view layer.
        /// </summary>
        ICollectionView ViewEntities { get; }

        /// <summary>
        /// Gets an enumerable collection of all entities.
        /// </summary>
        IEnumerable<TEntity> Entities { get; }

        /// <summary>
        /// Adds a new entity to the collection and updates the view.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Add(TEntity entity);

        /// <summary>
        /// Retrieves an entity by its ID.
        /// </summary>
        /// <param name="id">The unique ID of the entity to retrieve.</param>
        /// <returns>The entity with the specified ID, or <c>null</c> if not found.</returns>
        TEntity Get(int id);

        /// <summary>
        /// Retrieves an entity by comparing it with an existing entity.
        /// </summary>
        /// <param name="entity">The entity to search for.</param>
        /// <returns>The matching entity from the collection, or <c>null</c> if not found.</returns>
        TEntity Get(TEntity entity);

        /// <summary>
        /// Deletes an entity at the specified index and updates the view.
        /// </summary>
        /// <param name="index">The index of the entity to delete.</param>
        void Delete(int index);

        /// <summary>
        /// Updates an existing entity in the collection and refreshes the view.
        /// </summary>
        /// <param name="entity">The entity with updated values.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Updates the entire collection from the provided data set and refreshes the view.
        /// </summary>
        /// <param name="data">The new data set to populate the collection.</param>
        void UpdateFromDB(IEnumerable<TEntity> data);
    }
}
