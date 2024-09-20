using CasualTasker.DTO.Base;
using CasualTasker.Infrastructure.WPFExtension;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace CasualTasker.Infrastructure.ViewUpdaters
{
    public class ViewUpdater<TEntity> : IViewUpdater<TEntity> where TEntity : NamedEntity
    {
        protected readonly ObservableCollection<TEntity> _observableEntities;

        public ViewUpdater(ObservableCollection<TEntity> observableEntities)
        {
            _observableEntities = observableEntities;
        }

        public TEntity First => _observableEntities.FirstOrDefault();
        public ICollectionView ViewEntities =>
            CollectionViewSource.GetDefaultView(_observableEntities);
        public IEnumerable<TEntity> Entities => _observableEntities;

        public void Add(TEntity entity)
        {
            _observableEntities.Add(entity);
            NotifyUpdated();
        }

        public void Delete(int id)
        {
            var index = _observableEntities.FindItem(id);
            _observableEntities.RemoveAt(index);
            NotifyUpdated();
        }

        public TEntity Get(int id) =>
            _observableEntities.FirstOrDefault(el => el.Id == id);
        public TEntity Get(TEntity entity) =>
           _observableEntities.FirstOrDefault(el => el.Id == entity.Id);

        public void Update(TEntity entity)
        {
            var index = _observableEntities.FindItem(entity.Id);
            _observableEntities[index].UpdateFrom(entity);
            NotifyUpdated();
        }

        public void UpdateFromDB(IEnumerable<TEntity> data)
        {
            _observableEntities.AddRangeWithClear(data);
            NotifyUpdated();
        }

        private void NotifyUpdated() =>
            _observableEntities.UpdateView();
    }
}
