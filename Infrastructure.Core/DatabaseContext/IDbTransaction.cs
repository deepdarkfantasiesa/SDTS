using DotNetCore.CAP;

namespace Infrastructure.Core.DatabaseContext
{
    /// <summary>
    /// 数据库事务接口
    /// </summary>
    public interface IDbTransaction : IUnitOfWork, ITransactionSetting
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
        void BeginTransaction(ICapPublisher capPublisher, CancellationToken cancellationToken = default);

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
