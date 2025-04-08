using Infrastructure.Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using User.Infrastructure.Caches.ImMemory;
using User.Infrastructure.Settings;

namespace User.Infrastructure.Caches.Redis
{
    /// <summary>
    /// redis上下文
    /// </summary>
    public class RedisContext : ICacheImpl
    {
        /// <summary>
        /// 数据库
        /// </summary>
        private readonly IDatabase db;

        /// <summary>
        /// redis连接池
        /// </summary>
        private readonly RedisConnectionPool _connectionPool;

        /// <summary>
        /// redis配置类
        /// </summary>
        private RedisSettings _redisSettings;

        /// <summary>
        /// 内存缓存
        /// </summary>
        private readonly InMemoryCacheContext _memoryCache;

        /// <summary>
        /// 事务
        /// </summary>
        public ITransaction? transaction { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        private readonly string defaultKeyPrefix;

        /// <summary>
        /// redis上下文
        /// </summary>
        /// <param name="connectionPool">redis连接池</param>
        /// <param name="redisSettings">redis配置类</param>
        /// <param name="memoryCache">内存缓存</param>
        public RedisContext(RedisConnectionPool connectionPool, IOptionsSnapshot<RedisSettings> redisSettings, InMemoryCacheContext memoryCache)
        {
            _connectionPool = connectionPool;
            _redisSettings = redisSettings.Value;
            _memoryCache = memoryCache;
            defaultKeyPrefix = $"{{{RedisSlot.UserService}}}:";
            db = _connectionPool.GetDatabase(_redisSettings.DefaultDbNumber).WithKeyPrefix(defaultKeyPrefix);
        }

        #region redis相关的操作方法

        /// <summary>
        /// 获取redis中类型为string的数据
        /// </summary>
        /// <typeparam name="T">返回的类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="preferLocal">优先查本地缓存</param>
        /// <returns></returns>
        public async Task<QueryCacheResult<T>> GetStringAsync<T>(string key, bool preferLocal = false)
        {
            if (preferLocal)
            {
                var localCache = _memoryCache.Get<T>(key);

                if (localCache.IsHit)
                    return localCache;
            }

            var redisCache = await db.StringGetAsync(key, flags: CommandFlags.PreferReplica);
            if (redisCache == default)
            {
                return new QueryCacheResult<T>
                {
                    IsHit = false,
                    Value = default(T)
                };
            }
            return new QueryCacheResult<T>
            {
                IsHit = true,
                Value = Deserialize<T>(redisCache)
            };
        }

        /// <summary>
        /// 获取redis中类型为string的数据
        /// </summary>
        /// <typeparam name="T">返回的类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="tags">标签</param>
        /// <param name="preferLocal">优先查本地缓存</param>
        /// <returns></returns>
        public async Task<QueryCacheResult<T>> GetStringAsync<T>(string key, CacheTag[] tags, bool preferLocal = false)
        {
            if (preferLocal)
            {
                var localCache = _memoryCache.Get<T>(key, tags);

                if (localCache.IsHit)
                    return localCache;
            }

            foreach (var tag in tags)
            {
                var isExist = await db.SetContainsAsync(tag.ToString(), key);
                if (!isExist)
                {
                    return new QueryCacheResult<T>
                    {
                        IsHit = false,
                        Value = default(T)
                    };
                }
            }

            var redisCache = await db.StringGetAsync(key, flags: CommandFlags.PreferReplica);
            if (redisCache == default)
            {
                return new QueryCacheResult<T>
                {
                    IsHit = false,
                    Value = default(T)
                };
            }
            return new QueryCacheResult<T>
            {
                IsHit = true,
                Value = Deserialize<T>(redisCache)
            };
        }

        /// <summary>
        /// 向redis插入string类型的数据
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="expirationTime">过期时间</param>
        /// <returns></returns>
        public async Task<bool> SetStringAsync(string key, object value, TimeSpan? expirationTime)
        {
            if (expirationTime == null)
                expirationTime = TimeSpan.FromSeconds(_redisSettings.DefaultExpirationTime);
            var cache = Serialize(value);

            bool result = false;
            if (transaction == null)
            {
                result = await db.StringSetAsync(key, cache, expirationTime);
            }
            else
            {
                transaction.StringSetAsync(key, cache, expirationTime);
            }

            return result;
        }

        /// <summary>
        /// 向redis插入string类型的数据
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="tags">标签</param>
        /// <param name="expirationTime">过期时间</param>
        /// <returns></returns>
        public async Task<bool> SetStringAsync(string key, object value, CacheTag[] tags, TimeSpan? expirationTime)
        {
            if (expirationTime == null)
                expirationTime = TimeSpan.FromSeconds(_redisSettings.DefaultExpirationTime);
            var cache = Serialize(value);

            bool result = false;
            if (transaction == null)
            {
                transaction = BeginTransaction();

                foreach (var tag in tags)
                {
                    transaction.SetAddAsync(tag.ToString(), key);
                }
                transaction.StringSetAsync(key, cache, expirationTime);

                await CommitTransactionAsync(transaction);
            }
            else
            {
                foreach ( var tag in tags)
                {
                    transaction.SetAddAsync(tag.ToString(), key);
                }
                transaction.StringSetAsync(key, cache, expirationTime);
            }

            return result;
        }

        /// <summary>
        /// 向redis管道发布消息
        /// </summary>
        /// <param name="channel">管道名称</param>
        ///<param name="message">消息</param>
        /// <returns></returns>
        public async Task PublishAsync(string channel, object message)
        {
            var messageContent = Serialize(message);

            if (transaction == null)
            {
                await db.PublishAsync(channel, messageContent);
            }
            else
            {
                transaction.PublishAsync(channel, messageContent);
            }
        }

        /// <summary>
        /// 订阅redis的管道
        /// </summary>
        /// <param name="channel">管道名称</param>
        /// <param name="handler">消费消息时的任务</param>
        /// <returns></returns>
        public async Task SubscribeAsync(string channel, Action<RedisChannel, RedisValue> handler)
        {
            var connection = _connectionPool.GetConnection();
            var subscriber = connection.GetSubscriber();
            await subscriber.SubscribeAsync(defaultKeyPrefix + channel, handler);
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="dbNum">数据库编号</param>
        /// <returns></returns>
        public ITransaction BeginTransaction()
        {
            if (transaction == null)
            {
                transaction = db.CreateTransaction();
            }

            return transaction;
        }

        /// <summary>
        /// 提交执行事务
        /// </summary>
        /// <param name="transaction">事务对象</param>
        /// <returns></returns>
        public async Task<bool> CommitTransactionAsync(ITransaction transaction)
        {
            return await transaction.ExecuteAsync();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="value">待序列化的值</param>
        /// <returns></returns>
        private string Serialize(object value)
        {
            return System.Text.Json.JsonSerializer.Serialize(value);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">返回的类型</typeparam>
        /// <param name="cacheData">从redis查询出来的数据</param>
        /// <returns></returns>
        private T Deserialize<T>(RedisValue cacheData)
        {
            if (cacheData.HasValue)
            {
                return JsonConvert.DeserializeObject<T>(cacheData);
            }
            else
            {
                return default(T);
            }
        }

        #endregion
    }
}
