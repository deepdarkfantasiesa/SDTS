namespace User.Infrastructure.Caches
{
    /// <summary>
	/// 命令类型
	/// </summary>
	public enum CommondType
    {
        /// <summary>
        /// 创建
        /// </summary>
        Create = 0,

        /// <summary>
        /// 通过键删除
        /// </summary>
        DeleteByKey = 1,

        /// <summary>
        /// 通过标签删除
        /// </summary>
        DeleteByTags = 2,
    }
}
