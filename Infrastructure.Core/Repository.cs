using Domain.Abstraction;
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
            if (!entity.IsDeleted)
                throw new ArgumentException("聚合根未标记为已删除");

            var entry = _uow.Entry(entity);
            await CheckSubEntitiesDeleteStatus(entry);

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
            entity.UpdateAt = DateTime.UtcNow;
            //if (autoSetUpdateAt)
            //{
            //    var entry = _uow.Entry(entity);
            //    SetUpdateAt(entry);
            //}
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
            return _uow.Set<TEntity>()
                .Where(p => p.Id.Equals(id))
                .Single();
        }

        /// <summary>
        /// 通过Id异步获取
        /// </summary>
        /// <param name="id">聚合根Id</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>聚合根对象</returns>
        public virtual async Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken)
        {
            return await _uow.Set<TEntity>()
                .Where(p => p.Id.Equals(id))
                .SingleAsync(cancellationToken);
        }

        /// <summary>
        /// 递归遍历所有导航属性并设置UpdateAt值
        /// </summary>
        /// <param name="entry"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private void SetUpdateAt(EntityEntry entry)
        {
            foreach (var collection in entry.Collections)
            {
                if (collection.CurrentValue == null)
                    continue;

                var subEntities = collection.CurrentValue as IEnumerable<BaseEntity>
                    ?? throw new InvalidOperationException($"{collection.CurrentValue.GetGenericTypeName()}无法转换为Entity");
                foreach (var subEntity in subEntities)
                {
                    subEntity.UpdateAt = DateTime.Now;
                    var subEntry = _uow.Entry(subEntity);
                    SetUpdateAt(subEntry);
                }
            }

            foreach (var navigation in entry.Navigations)
            {
                if (navigation.CurrentValue == null)
                    continue;

                var subEntity = navigation.CurrentValue as BaseEntity
                    ?? throw new InvalidOperationException($"{navigation.CurrentValue.GetGenericTypeName()}无法转换为Entity");
                subEntity.UpdateAt = DateTime.Now;
                var subEntry = _uow.Entry(subEntity);
                SetUpdateAt(subEntry);
            }
        }

        /// <summary>
        /// 检查子实体的删除情况
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private async Task CheckSubEntitiesDeleteStatus(EntityEntry entry)
        {
            foreach (var collection in entry.Collections)
            {
                if (!collection.IsLoaded) // 检查导航属性是否已加载
                    throw new ArgumentNullException($"{collection.GetGenericTypeName()}子实体集未被加载");

                if (collection.CurrentValue == null)
                    continue;

                var subEntities = collection.CurrentValue as IEnumerable<BaseEntity>
                    ?? throw new InvalidOperationException($"{collection.CurrentValue.GetGenericTypeName()}无法转换为Entity");
                foreach (var subEntity in subEntities)
                {
                    if (!subEntity.IsDeleted)
                        throw new ArgumentException("子实体未被标记为已删除");
                    var subEntry = _uow.Entry(subEntity);
                    await CheckSubEntitiesDeleteStatus(subEntry);
                }
            }

            foreach (var navigation in entry.Navigations)
            {
                if (!navigation.IsLoaded)
                    throw new ArgumentNullException($"{navigation.GetGenericTypeName()}子实体未被加载");

                if (navigation.CurrentValue == null)
                    continue;

                var subEntity = navigation.CurrentValue as BaseEntity
                    ?? throw new InvalidOperationException($"{navigation.CurrentValue.GetGenericTypeName()}无法转换为Entity");

                if (!subEntity.IsDeleted)
                    throw new ArgumentException("子实体未被标记为已删除");
                var subEntry = _uow.Entry(subEntity);
                await CheckSubEntitiesDeleteStatus(subEntry);
            }
        }
    }
}
