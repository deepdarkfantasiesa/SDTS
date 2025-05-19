using Domain.Abstraction;

namespace Infrastructure.Core
{
    /// <summary>
    /// 泛型仓储
    /// </summary>
    /// <typeparam name="TAggregateRoot">聚合根对象</typeparam>
    /// <typeparam name="TKey">聚合根Id</typeparam>
    public interface IRepository<TAggregateRoot, TKey>
        where TAggregateRoot : AggregateRoot<TKey>
        where TKey : notnull, IEntityTypeId
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">聚合根对象</param>
        /// <returns></returns>
        TAggregateRoot Add(TAggregateRoot entity);

        /// <summary>
        /// 异步新增
        /// </summary>
        /// <param name="entity">聚合根对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<TAggregateRoot> AddAsync(TAggregateRoot entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="entities">聚合根对象集合</param>
        void AddRange(IEnumerable<TAggregateRoot> entities);

        /// <summary>
        /// 异步批量插入
        /// </summary>
        /// <param name="entities">聚合根对象集合</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task AddRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 软删除
        /// </summary>
        /// <param name="entity">聚合根对象</param>
        /// <returns></returns>
        bool Delete(TAggregateRoot entity);

        /// <summary>
        /// 异步软删除
        /// </summary>
        /// <param name="entity">聚合根对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(TAggregateRoot entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">聚合根对象</param>
        /// <returns></returns>
        TAggregateRoot Update(TAggregateRoot entity);

        /// <summary>
        /// 异步更新
        /// </summary>
        /// <param name="entity">聚合根对象</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<TAggregateRoot> UpdateAsync(TAggregateRoot entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        void UpdateRange(IEnumerable<TAggregateRoot> entities);

        /// <summary>
        /// 异步批量更新
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task UpdateRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 通过Id获取
        /// </summary>
        /// <param name="id">聚合根Id</param>
        /// <returns>聚合根对象</returns>
        TAggregateRoot GetById(TKey id);

        /// <summary>
        /// 通过Id异步获取
        /// </summary>
        /// <param name="id">聚合根Id</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>聚合根对象</returns>
        Task<TAggregateRoot> GetByIdAsync(TKey id, CancellationToken cancellationToken = default(CancellationToken));
    }
}
