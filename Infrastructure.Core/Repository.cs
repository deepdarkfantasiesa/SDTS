using Domain.Abstraction;
using Infrastructure.Core.DatabaseContext;
using Infrastructure.Core.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Core
{
    /// <summary>
    /// 泛型仓储
    /// </summary>
    /// <typeparam name="TDbContext">数据库上下文</typeparam>
    /// <typeparam name="TEntity">聚合根类型</typeparam>
    /// <typeparam name="TKey">聚合根Id类型</typeparam>
    public class Repository<TDbContext, TEntity, TKey> : IRepository<TEntity, TKey>
        where TDbContext : DbContext, IUnitOfWork
        where TEntity : AggregateRoot<TKey>
        where TKey : notnull, IEntityTypeId
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private readonly TDbContext _uow;

        /// <summary>
        /// 泛型仓储
        /// </summary>
        /// <param name="uow">数据库上下文</param>
        public Repository(TDbContext uow)
        {
            _uow = uow;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">聚合根对象</param>
        /// <returns></returns>
        public virtual TEntity Add(TEntity entity)
        {
            return AddAsync(entity).Result;
        }

        /// <summary>
        /// 异步新增
        /// </summary>
        /// <param name="entity">聚合根对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            return (await _uow.AddAsync(entity, cancellationToken)).Entity;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entities">聚合根对象集合</param>
        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            AddRangeAsync(entities).Wait();
        }

        /// <summary>
        /// 异步批量插入
        /// </summary>
        /// <param name="entities">聚合根对象集合</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _uow.AddRangeAsync(entities, cancellationToken);
        }

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="entity">聚合根对象</param>
        /// <param name="autoSetIsDelete">是否自动设置软删字段</param>
        /// <returns></returns>
        public virtual bool Delete(TEntity entity)
        {
            return DeleteAsync(entity).Result;
        }

        /// <summary>
        /// 异步软删除
        /// </summary>
        /// <param name="entity">聚合根对象</param>
        /// <param name="autoSetIsDelete">是否自动设置软删字段</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckIsDeleted();

            await Task.Run(() => { _uow.Update(entity); }, cancellationToken);
            return true;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">聚合根对象</param>
        /// <param name="autoSetUpdateAt">是否自动设置更新时间</param>
        /// <returns></returns>
        public virtual TEntity Update(TEntity entity)
        {
            SetUpdateAt();
            return _uow.Update(entity).Entity;
        }

        /// <summary>
        /// 异步更新
        /// </summary>
        /// <param name="entity">聚合根对象</param>
        /// <param name="autoSetUpdateAt">是否自动设置更新时间</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return await Task.FromResult(Update(entity));
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            SetUpdateAt();
            _uow.UpdateRange(entities);
        }

        /// <summary>
        /// 异步批量更新
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
        {
            await Task.Run(() => UpdateRange(entities), cancellationToken);
        }

        /// <summary>
        /// 通过Id获取
        /// </summary>
        /// <param name="id">聚合根Id</param>
        /// <returns>聚合根对象</returns>
        public virtual TEntity GetById(TKey id)
        {
            return _uow.Set<TEntity>().Find(id);
        }

        /// <summary>
        /// 通过Id异步获取
        /// </summary>
        /// <param name="id">聚合根Id</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>聚合根对象</returns>
        public virtual async Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken)
        {
            return await _uow.Set<TEntity>().FindAsync(id);
        }

        /// <summary>
        /// 设置状态为Modified实体的UpdateAt字段
        /// </summary>
        private void SetUpdateAt()
        {
            var modifiedEntities = _uow.ChangeTracker.Entries<BaseEntity>()
                .Where(p => p.State == EntityState.Modified)
                .Select(p => p.Entity);
            foreach (var modifiedEntity in modifiedEntities)
            {
                modifiedEntity.UpdateAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// 检查所有实体的软删除字段
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private void CheckIsDeleted()
        {
            var hasUnDeleteEntity = _uow.ChangeTracker.Entries<BaseEntity>()
                .Where(p => p.Entity.IsDeleted == false 
                    && p.State == EntityState.Modified)
                .Any();

            if (hasUnDeleteEntity)
                throw new ArgumentException("当前聚合有实体未被标记为已删除");
        }
    }
}
