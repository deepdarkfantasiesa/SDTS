namespace Infrastructure.Core
{
    /// <summary>
    /// 工作单元
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// 先分发领域事件再持久化实体（未显示开启事务时调用）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 先持久化实体到数据库并分发领域事件（已显示开启事务时调用）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    }
}
