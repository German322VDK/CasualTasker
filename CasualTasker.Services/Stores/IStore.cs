using CasualTasker.DTO.Base;

namespace CasualTasker.Services.Stores
{
    public interface IStore<TEntity> : IStoreSync<TEntity>, IStoreAsync<TEntity> where TEntity : NamedEntity
    {
        IQueryable<TEntity> GetAll();
        TEntity GetById(int id);
    }

    public interface IStoreSync<TEntity> where TEntity : NamedEntity
    {
        TEntity Add(TEntity item);
        TEntity Update(TEntity item);
        bool Delete(int id);
    }

    public interface IStoreAsync<TEntity> where TEntity : NamedEntity
    {
        Task<TEntity> AddAsync(TEntity item);
        Task<TEntity> UpdateAsync(TEntity item);
        Task<bool> DeleteAsync(int id);
    }
}
