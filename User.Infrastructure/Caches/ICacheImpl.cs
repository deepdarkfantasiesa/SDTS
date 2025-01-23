using StackExchange.Redis;

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
		/// <param name="dbNum">数据库编号</param>
		/// <param name="preferLocal">优先查本地缓存</param>
		/// <returns></returns>
		Task<T> GetStringAsync<T>(string cacheKey, int dbNum = -1, bool preferLocal = false);

		/// <summary>
		/// 向redis插入string类型的数据
		/// </summary>
		/// <param name="cacheKey">缓存键</param>
		/// <param name="value">缓存值</param>
		/// <param name="expirationTime">过期时间</param>
		/// <param name="dbNum">数据库编号</param>
		/// <returns></returns>
		Task<bool> SetStringAsync(string cacheKey, object value, TimeSpan? expirationTime = null, int dbNum = -1);

		/// <summary>
		/// 向redis管道发布消息
		/// </summary>
		/// <param name="channel">管道名称</param>
		/// <param name="dbNum">默认数据库</param>
		///<param name="value">值</param>
		/// <returns></returns>
		Task PublishAsync(string channel, object value, int dbNum = -1);

		/// <summary>
		/// 订阅redis的管道
		/// </summary>
		/// <param name="channel">管道名称</param>
		/// <param name="handler">任务</param>
		/// <returns></returns>
		Task SubscribeAsync(string channel, Action<RedisChannel, RedisValue> handler);

		ITransaction BeginTransaction(int dbNum = -1);

		Task<bool> CommitTransactionAsync(ITransaction transaction);

		ITransaction? GetTransaction(int databaseNumber = -1);

		Task<bool> Test(string cacheKey, object value, TimeSpan? expirationTime, int dbNum = -1);
	}
}
