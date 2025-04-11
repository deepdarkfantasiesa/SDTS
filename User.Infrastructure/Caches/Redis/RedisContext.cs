using Infrastructure.Core;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using System.Text.Json;
using User.Infrastructure.Caches.ImMemory;
using User.Infrastructure.Caches.Models.SyncMemoryCacheCommds;
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
        /// 默认键前缀
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

        #region GetString

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

        #endregion

        #region SetString

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
                    var tagString = tag.ToString();
                    var otherTags = tags.Where(p => p != tag).ToArray();
                    var otherTagsJson = JsonSerializer.Serialize(otherTags);
                    transaction.HashSetAsync(tagString, [new HashEntry(key, otherTagsJson)]);
                    transaction.HashFieldExpireAsync(tagString, [new RedisValue(key)], expirationTime.Value);
                }
                transaction.StringSetAsync(key, cache, expirationTime);

                await CommitTransactionAsync(transaction);
            }
            else
            {
                foreach (var tag in tags)
                {
                    var tagString = tag.ToString();
                    var otherTags = tags.Where(p => p != tag).ToArray();
                    var otherTagsJson = JsonSerializer.Serialize(otherTags);
                    transaction.HashSetAsync(tagString, [new HashEntry(key, otherTagsJson)]);
                    transaction.HashFieldExpireAsync(tagString, [new RedisValue(key)], expirationTime.Value);
                }
                transaction.StringSetAsync(key, cache, expirationTime);
            }

            return result;
        }

        #endregion

        #region hash

        /// <summary>
        /// 插入hash
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="tags">标签</param>
        /// <param name="expirationTime">过期时间</param>
        /// <returns></returns>
        public async Task<bool> SetHashAsync(string key, object value, CacheTag[] tags, TimeSpan? expirationTime)
        {
            if (tags.Count() == 0)
                throw new ArgumentException("请传入tags");

            if (expirationTime == null)
                expirationTime = TimeSpan.FromSeconds(_redisSettings.DefaultExpirationTime);

            var cache = Serialize(value);

            if (transaction == null)
            {
                transaction = BeginTransaction();

                SetHashWithTrans(key, cache, tags, expirationTime.Value);

                await CommitTransactionAsync(transaction);
            }
            else
            {
                SetHashWithTrans(key, cache, tags, expirationTime.Value);
            }

            return true;
        }

        /// <summary>
        /// 插入hash
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expirationTime">过期时间</param>
        /// <returns></returns>
        public async Task<bool> SetHashAsync(string key, object value, TimeSpan? expirationTime)
        {
            if (expirationTime == null)
                expirationTime = TimeSpan.FromSeconds(_redisSettings.DefaultExpirationTime);

            var cache = Serialize(value);

            if (transaction == null)
            {
                transaction = BeginTransaction();

                SetHashWithTrans(key, cache, expirationTime.Value);

                await CommitTransactionAsync(transaction);
            }
            else
            {
                SetHashWithTrans(key, cache, expirationTime.Value);
            }

            return true;
        }

        /// <summary>
        /// 获取hash
        /// </summary>
        /// <typeparam name="T">返回的类型</typeparam>
        /// <param name="key">缓存键</param>
        /// <param name="preferLocal">优先查本地缓存</param>
        /// <returns></returns>
        public async Task<QueryCacheResult<T>> GetHashAsync<T>(string key, bool preferLocal = false)
        {
            if (preferLocal)
            {
                var localCache = _memoryCache.Get<T>(key);

                if (localCache.IsHit)
                    return localCache;
            }

            var redisCache = await db.HashGetAsync(key, RedisHashField.Data, flags: CommandFlags.PreferReplica);
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

        #endregion

        #region Pub/Sub

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

        #endregion

        #region 事务

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
        public async Task<bool> CommitTransactionAsync(ITransaction trans)
        {
            transaction = null;
            return await trans.ExecuteAsync();
        }

        #endregion

        #region 其他方法

        /// <summary>
        /// 移除set中过期的值
        /// </summary>
        /// <param name="tags">标签</param>
        /// <returns></returns>
        public async Task RemoveExpireTagValue(CacheTag[] tags)
        {
            var keys = new List<string>();

            foreach (var tag in tags)
            {
                var cacheKeys = await db.HashGetAllAsync(tag.ToString());

                var expiredKeys = new List<string>();
                foreach (var cacheKey in cacheKeys.Select(p => p.Name))
                {
                    var isExist = await db.KeyExistsAsync(cacheKey.ToString());
                    if (!isExist)
                        expiredKeys.Add(cacheKey);
                }

                foreach (var expiredKey in expiredKeys)
                {
                    await db.HashDeleteAsync(tag.ToString(), expiredKey);
                    //_memoryCache.RemoveExpireTagValue(tag, expiredKey);
                }
            }
        }

        /// <summary>
		/// 通过tag移除缓存
		/// </summary>
		/// <param name="tags">标签</param>
		/// <returns></returns>
        public async Task RemoveByTags(CacheTag[] tags)
        {
            var tagList = tags.Select(p => p.ToString()).ToList();
            var cacheToRemove = new List<string>();//需要删除的缓存
            var tagToRemove = new Dictionary<string, string>();//key为tag，value为缓存键
            foreach (var tag in tagList)
            {
                var subFields = await db.HashGetAllAsync(tag);//获取当前tag下所有的缓存键
                foreach (var subField in subFields)
                {
                    var key = subField.Name;//缓存键
                    var otherTags = subField.Value;//该缓存键的其他标签
                    if (otherTags != default)
                    {
                        var otherTagsArray = Deserialize<CacheTag[]>(otherTags);
                        var otherTagsKeyToRemove = otherTagsArray.Select(p => p.ToString()).Where(p => !tagList.Contains(p)).ToList();
                        foreach (var otherTagKeyToRemove in otherTagsKeyToRemove)
                        {
                            tagToRemove.Add(otherTagKeyToRemove, key);//其他标签下的该键
                        }
                    }
                    tagToRemove.Add(tag, key);//当前标签下的该键
                }
                cacheToRemove.AddRange(subFields.Select(p => p.Name.ToString()).ToList());//当前缓存
            }
            cacheToRemove = cacheToRemove.Distinct().ToList();
            tagToRemove = tagToRemove.Distinct().ToDictionary<string, string>();

            if (cacheToRemove.Count == 0 && tagToRemove.Count == 0)
                return;

            if (transaction != null)
            {
                RemoveTagsWithTrans(cacheToRemove, tagToRemove);
            }
            else
            {
                BeginTransaction();

                RemoveTagsWithTrans(cacheToRemove, tagToRemove);

                await CommitTransactionAsync(transaction);
            }
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
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(cacheData);
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 在redis事务中设置hash
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="cache">值</param>
        /// <param name="tags">标签</param>
        /// <param name="expirationTime">过期时间</param>
        private void SetHashWithTrans(string key, string cache, CacheTag[] tags, TimeSpan expirationTime)
        {
            foreach (var tag in tags)
            {
                var tagString = tag.ToString();
                var otherTags = tags.Where(p => p != tag);
                transaction.HashSetAsync(tagString, [new HashEntry(key, Serialize(otherTags))]);
                transaction.HashFieldExpireAsync(tagString, [new RedisValue(key)], expirationTime);
            }

            transaction.HashSetAsync(key, nameof(RedisHashField.Data), cache);
            transaction.HashSetAsync(key, nameof(RedisHashField.Tags), Serialize(tags));
            transaction.KeyExpireAsync(key, expirationTime);
        }

        /// <summary>
        /// 在redis事务中设置hash
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="cache">值</param>
        /// <param name="expirationTime">过期时间</param>
        private void SetHashWithTrans(string key, string cache, TimeSpan expirationTime)
        {
            transaction.HashSetAsync(key, nameof(RedisHashField.Data), cache);
            transaction.HashSetAsync(key, nameof(RedisHashField.Tags), "");
            transaction.KeyExpireAsync(key, expirationTime);
        }

        /// <summary>
        /// 通过redis事务移除缓存
        /// </summary>
        /// <param name="cacheToRemove">需要被移除的缓存</param>
        /// <param name="tagToRemove">需要被移除tag下的缓存键</param>
        private async void RemoveTagsWithTrans(IEnumerable<string> cacheToRemove, Dictionary<string, string> tagToRemove)
        {
            foreach (var key in cacheToRemove)
            {
                transaction.HashDeleteAsync(key, RedisHashField.Data);
                transaction.HashDeleteAsync(key, RedisHashField.Tags);
                await PublishAsync(CacheKeyPrefix.SyncInMemoryCache, new DeleteCommand()
                {
                    Key = key
                });
            }
            foreach (var tagHash in tagToRemove)
            {
                transaction.HashDeleteAsync(tagHash.Key, tagHash.Value);
            }
        }

        #endregion
    }
}
