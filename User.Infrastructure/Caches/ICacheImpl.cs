namespace User.Infrastructure.Caches
{
	/// <summary>
	/// 缓存操作接口
	/// </summary>
	public interface ICacheImpl
	{
		/// <summary>
		/// 获取缓存中类型为string的数据
		/// </summary>
		/// <typeparam name="T">返回的类型</typeparam>
		/// <param name="cacheKey">缓存键</param>
		/// <param name="databaseNumber">数据库编号</param>
		/// <returns></returns>
		Task<T> GetStringAsync<T>(string cacheKey, int? databaseNumber = null);

		/// <summary>
		/// 向redis插入string类型的数据
		/// </summary>
		/// <param name="cacheKey">缓存键</param>
		/// <param name="value">缓存值</param>
		/// <param name="expirationTime">过期时间</param>
		/// <param name="databaseNumber">数据库编号</param>
		/// <returns></returns>
		Task<bool> SetStringAsync(string cacheKey, object value, TimeSpan? expirationTime = null, int? databaseNumber = null);
	}
}
