using System.Data;

namespace Infrastructure.Core.DatabaseContext
{
    /// <summary>
    /// 数据库事务设置
    /// </summary>
    public interface ITransactionSetting
    {
        /// <summary>
        /// 隔离等级
        /// </summary>
        public IsolationLevel IsolationLevel { get; set; }

        /// <summary>
        /// 超时时间（秒）
        /// </summary>
        public int? Timeout { get; set; }

        /// <summary>
        /// 是否已设置
        /// </summary>
        public bool IsSet { get; }
    }
}
