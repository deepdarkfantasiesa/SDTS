using Infrastructure.Core;
using StackExchange.Redis;

namespace User.Infrastructure.Caches
{
	/// <summary>
	/// 缓存操作接口
	/// </summary>
	public interface ICacheImpl
	{
		/// <summary>
		/// 向redis获取类型为string的数据
		/// </summary>
		/// <typeparam name="T">返回的类型</typeparam>
		/// <param name="key">缓存键</param>
		/// <param name="preferLocal">优先查本地缓存</param>
		/// <returns></returns>
		Task<QueryCacheResult<T>> GetStringAsync<T>(string key, bool preferLocal = false);

		/// <summary>
		/// 获取redis中类型为string的数据
		/// </summary>
		/// <typeparam name="T">返回的类型</typeparam>
		/// <param name="key">缓存键</param>
		/// <param name="tags">标签</param>
		/// <param name="preferLocal">优先查本地缓存</param>
		/// <returns></returns>
		Task<QueryCacheResult<T>> GetStringAsync<T>(string key, CacheTag[] tags, bool preferLocal = false);

        /// <summary>
        /// 向redis中插入string类型的数据
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expirationTime">过期时间</param>
        /// <returns></returns>
        Task<bool> SetStringAsync(string key, object value, TimeSpan? expirationTime = null);

		/// <summary>
		/// 向redis插入string类型的数据
		/// </summary>
		/// <param name="key">缓存键</param>
		/// <param name="value">缓存值</param>
		/// <param name="tags">标签</param>
		/// <param name="expirationTime">过期时间</param>
		/// <returns></returns>
		Task<bool> SetStringAsync(string key, object value, CacheTag[] tags, TimeSpan? expirationTime);


        /// <summary>
        /// 向redis管道发布消息
        /// </summary>
        /// <param name="channel">管道名称</param>
        ///<param name="message">消息</param>
        /// <param name="dbNum">默认数据库</param>
        /// <returns></returns>
        Task PublishAsync(string channel, object message);

		/// <summary>
		/// 订阅redis的管道
		/// </summary>
		/// <param name="channel">管道名称</param>
		/// <param name="handler">任务</param>
		/// <returns></returns>
		Task SubscribeAsync(string channel, Action<RedisChannel, RedisValue> handler);

		/// <summary>
		/// 开启事务
		/// </summary>
		/// <returns></returns>
		ITransaction BeginTransaction();

		/// <summary>
		/// 提交执行事务
		/// </summary>
		/// <param name="transaction">事务对象</param>
		/// <returns></returns>
		Task<bool> CommitTransactionAsync(ITransaction transaction);

        /// <summary>
        /// 移除set中过期的值
        /// </summary>
        /// <param name="tags">标签</param>
        /// <returns></returns>
        Task RemoveExpireTagValue(CacheTag[] tags);

    }
}
