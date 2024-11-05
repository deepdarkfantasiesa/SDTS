namespace User.Infrastructure.Caches
{
	/// <summary>
	/// 缓存操作接口
	/// </summary>
	public interface ICacheImpl
	{
		/// <summary>
		/// 获取redis中类型为string的数据
		/// </summary>
		/// <typeparam name="T">返回的类型</typeparam>
		/// <param name="cacheKey">缓存键</param>
		/// <param name="databaseNumber">数据库编号</param>
		/// <returns></returns>
		public Task<T> GetStringAsync<T>(string cacheKey, int? databaseNumber = null);
	}
}
