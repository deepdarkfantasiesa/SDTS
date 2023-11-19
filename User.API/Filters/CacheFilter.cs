using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace User.API.Filters
{
    public class CacheFilter : Attribute, IResourceFilter
    {
        private readonly IDistributedCache _distributedCache;
        public CacheFilter(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        //执行controller之前查询缓存，若有则短路返回结果，若无则继续执行
        //如果查询条件在body里面这里会有bug
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var requestContext = context.HttpContext.Request;
            var cacheKey = context.ActionDescriptor.ToString() + requestContext.QueryString;
            var usercache = _distributedCache.GetString(cacheKey);
            if (usercache != null)
            {
                context.Result = new ContentResult { Content = usercache };
            };
        }

        //执行完controller之后写入缓存
        public async void OnResourceExecuted(ResourceExecutedContext context)
        {
            var cacheKey = context.ActionDescriptor.ToString() + context.HttpContext.Request.QueryString;
            var result = (context.Result as ObjectResult);
            var cache = JsonSerializer.Serialize(result.Value);

            var cacheOptions = new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow + TimeSpan.FromMinutes(1),
                //AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(120), //绝对过期时间不生效，可能是docker内部时间的问题
                SlidingExpiration = TimeSpan.FromSeconds(new Random().Next(10, 20)) 
            };

            await _distributedCache.SetStringAsync(cacheKey, cache, cacheOptions);
        }
    }
    
}
