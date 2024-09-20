using CasualTasker.DTO.Base;
using System.ComponentModel;

namespace CasualTasker.Infrastructure.ViewUpdaters
{
    public interface IViewUpdater<TEntity> where TEntity : NamedEntity
    {
        TEntity First { get; }
        ICollectionView ViewEntities { get; }
        IEnumerable<TEntity> Entities { get; }
        void Add(TEntity entity);
        TEntity Get(int id);
        TEntity Get(TEntity entity);
        void Delete(int index);
        void Update(TEntity entity);
        void UpdateFromDB(IEnumerable<TEntity> data);
    }
}
