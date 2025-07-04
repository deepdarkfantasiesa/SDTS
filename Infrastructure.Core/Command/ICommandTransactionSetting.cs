using System.Data;

namespace Infrastructure.Core.Command
{
    /// <summary>
    /// Command事务设置
    /// </summary>
    public interface ICommandTransactionSetting
    {
        /// <summary>
        /// 事务隔离等级
        /// </summary>
        public IsolationLevel IsolationLevel { get; }

        /// <summary>
        /// 事务超时时间（秒）
        /// </summary>
        public int? Timeout { get; }
    }
}
