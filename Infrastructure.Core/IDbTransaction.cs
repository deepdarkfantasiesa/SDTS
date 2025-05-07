namespace Infrastructure.Core
{
    /// <summary>
    /// 数据库事务接口
    /// </summary>
    public interface IDbTransaction: IUnitOfWork
    {
        /// <summary>
        /// 是否开启事务
        /// </summary>
        bool HasActiveTransaction { get; }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>事务对象</returns>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        Task RollbackTransaction(CancellationToken cancellationToken = default);
    }
}
