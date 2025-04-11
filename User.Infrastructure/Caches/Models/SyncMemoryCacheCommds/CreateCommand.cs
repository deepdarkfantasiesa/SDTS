namespace User.Infrastructure.Caches.Models.SyncMemoryCacheCommds
{
    /// <summary>
    /// 创建命令
    /// </summary>
    public record CreateCommand : BaseCommand
    {
        /// <summary>
        /// 键
        /// </summary>
        public override string Key { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public object? Data 
        {
            get;
            init;
        }

        /// <summary>
        /// 命令类型
        /// </summary>
        public override CommondType Type { get; } = CommondType.Create;

        /// <summary>
        /// 
        /// </summary>
        private string? originDataType;

        /// <summary>
		/// 数据类型
		/// </summary>
		public string DataType 
        {
            get;
            init;
        }

        /// <summary>
        /// 过期时间
        /// </summary>
        public TimeSpan ExpirationTime { get; set; }
    }
}
