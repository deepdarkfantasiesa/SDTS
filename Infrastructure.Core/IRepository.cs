using Domain.Abstraction;

namespace Infrastructure.Core
{
    public interface IRepository<TEntity,TKey> 
        where TEntity : Entity, IAggregateRoot
        where TKey : IEntityTypeId
    {
        IUnitOfWork UnitOfWork { get; }
        TEntity Add(TEntity entity);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        TEntity Update(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        bool Remove(Entity entity);
        Task<bool> RemoveAsync(Entity entity);

        Task<TEntity> GetAsync(TKey id);
    }
}
