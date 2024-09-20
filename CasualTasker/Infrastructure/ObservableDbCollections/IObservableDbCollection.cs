using CasualTasker.DTO.Base;
using System.ComponentModel;

namespace CasualTasker.Infrastructure.ObservableDbCollections
{
    public interface IObservableDbCollection<TEntity> where TEntity : NamedEntity
    {
        public ICollectionView ViewEntities { get; }
        public IEnumerable<TEntity> Entities { get; }

        public TEntity First { get; }
        public TEntity DefaultDeletedEntity { get; }

        public bool Add(TEntity entity);
        public bool Delete(TEntity entity);
        public bool EntityExists(TEntity entity);
        public TEntity Get(int id);
        public TEntity Get(TEntity entity);
        public bool Update(TEntity entity, bool isDBUpdated = true);
        public void UpdateFromDB();
        public void BatchUpdate(IEnumerable<TEntity> entities);
    }
}
