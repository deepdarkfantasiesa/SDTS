using Domain.Abstraction;

namespace Infrastructure.Core
{
    /// <summary>
    /// 泛型仓储
    /// </summary>
    /// <typeparam name="TEntity">聚合根对象</typeparam>
    /// <typeparam name="TKey">聚合根Id</typeparam>
    public interface IRepository<TEntity, TKey>
        where TEntity : Entity<TKey>, IAggregateRoot
        where TKey : notnull, IEntityTypeId
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">聚合根对象</param>
        /// <returns></returns>
        TEntity Add(TEntity entity);

        /// <summary>
        /// 异步新增
        /// </summary>
        /// <param name="entity">聚合根对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entities">聚合根对象集合</param>
        void AddRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// 异步批量插入
        /// </summary>
        /// <param name="entities">聚合根对象集合</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="entity">聚合根对象</param>
        /// <returns></returns>
        bool Delete(TEntity entity);

        /// <summary>
        /// 异步软删除
        /// </summary>
        /// <param name="entity">聚合根对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">聚合根对象</param>
        /// <returns></returns>
        TEntity Update(TEntity entity);

        /// <summary>
        /// 异步更新
        /// </summary>
        /// <param name="entity">聚合根对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        void UpdateRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// 异步批量更新
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

        /// <summary>
        /// 通过Id获取
        /// </summary>
        /// <param name="id">聚合根Id</param>
        /// <returns>聚合根对象</returns>
        TEntity GetById(TKey id);

        /// <summary>
        /// 通过Id异步获取
        /// </summary>
        /// <param name="id">聚合根Id</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>聚合根对象</returns>
        Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken = default(CancellationToken));
    }
}
