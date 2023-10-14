using Domain.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Core
{
    public interface IRepository<TEntity> where TEntity : Entity, IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
        TEntity Add(TEntity entity);
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        TEntity Update(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        bool Remove(Entity entity);
        Task<bool> RemoveAsync(Entity entity);
    }
}
