using CasualTasker.DTO.Base;
using CasualTasker.Infrastructure.WPFExtension;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace CasualTasker.Infrastructure.ViewUpdaters
{
    /// <summary>
    /// Provides functionality to manage and update a collection of entities in the view layer.
    /// This class synchronizes the view with changes in the underlying data.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity, which must inherit from <see cref="NamedEntity"/>.</typeparam>
    public class ViewUpdater<TEntity> : IViewUpdater<TEntity> where TEntity : NamedEntity
    {
        private readonly ObservableCollection<TEntity> _observableEntities;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewUpdater{TEntity}"/> class.
        /// </summary>
        /// <param name="observableEntities">The collection of entities to manage in the view.</param>
        public ViewUpdater(ObservableCollection<TEntity> observableEntities)
        {
            _observableEntities = observableEntities;
        }

        /// <summary>
        /// Gets the first entity in the collection.
        /// </summary>
        public TEntity First => _observableEntities.FirstOrDefault();

        /// <summary>
        /// Gets a view of the entities as an <see cref="ICollectionView"/> for data binding in the view layer.
        /// </summary>
        public ICollectionView ViewEntities =>
            CollectionViewSource.GetDefaultView(_observableEntities);

        /// <summary>
        /// Gets an enumerable collection of all entities.
        /// </summary>
        public IEnumerable<TEntity> Entities => _observableEntities;

        /// <summary>
        /// Adds an entity to the collection and updates the view.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void Add(TEntity entity)
        {
            _observableEntities.Add(entity);
            NotifyUpdated();
        }

        /// <summary>
        /// Deletes an entity from the collection by its ID and updates the view.
        /// </summary>
        /// <param name="id">The ID of the entity to delete.</param>
        public void Delete(int id)
        {
            var index = _observableEntities.FindItem(id);
            _observableEntities.RemoveAt(index);
            NotifyUpdated();
        }

        /// <summary>
        /// Retrieves an entity from the collection by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to retrieve.</param>
        /// <returns>The entity with the specified ID, or <c>null</c> if not found.</returns>
        public TEntity Get(int id) =>
            _observableEntities.FirstOrDefault(el => el.Id == id);

        /// <summary>
        /// Retrieves an entity from the collection by comparing it with an existing entity.
        /// </summary>
        /// <param name="entity">The entity to search for.</param>
        /// <returns>The matching entity from the collection, or <c>null</c> if not found.</returns>
        public TEntity Get(TEntity entity) =>
           _observableEntities.FirstOrDefault(el => el.Id == entity.Id);

        /// <summary>
        /// Updates an entity in the collection and refreshes the view.
        /// </summary>
        /// <param name="entity">The entity with updated values.</param>
        public void Update(TEntity entity)
        {
            var index = _observableEntities.FindItem(entity.Id);
            _observableEntities[index].UpdateFrom(entity);
            NotifyUpdated();
        }

        /// <summary>
        /// Updates the entire collection from the provided data and refreshes the view.
        /// </summary>
        /// <param name="data">The new data to populate the collection.</param>
        public void UpdateFromDB(IEnumerable<TEntity> data)
        {
            _observableEntities.AddRangeWithClear(data);
            NotifyUpdated();
        }

        /// <summary>
        /// Notifies the view that the collection has been updated.
        /// </summary>
        private void NotifyUpdated() =>
            _observableEntities.UpdateView();
    }
}
