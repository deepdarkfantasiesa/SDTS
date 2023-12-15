using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Net;
using System.Text.Json;

namespace User.API.Filters
{
    public class CacheFilter : Attribute, IResourceFilter
    {
        private readonly ConnectionMultiplexer _multiplexer;
        public CacheFilter(ConnectionMultiplexer multiplexer)
        {
            _multiplexer = multiplexer;
        }

        //执行controller之前查询缓存，若有则短路返回结果，若无则继续执行
        //如果查询条件在body里面这里会有bug
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var requestContext = context.HttpContext.Request;
            var cacheKey = requestContext.Path + requestContext.QueryString;
            
            var usercache = _multiplexer.GetDatabase().StringGet(cacheKey);
            if (!usercache.IsNull)
            {
                context.Result = new ContentResult { Content = usercache };
            };
        }

        //执行完controller之后写入缓存
        public async void OnResourceExecuted(ResourceExecutedContext context)
        {
            var requestContext = context.HttpContext.Request;
            var cacheKey = requestContext.Path + requestContext.QueryString;
            
            if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.OK)
            {
                var result = (context.Result as ObjectResult);
                var cache = JsonSerializer.Serialize(result.Value);
                _multiplexer.GetDatabase().StringSetAsync(cacheKey, cache,
                    TimeSpan.FromMinutes(1) + TimeSpan.FromSeconds(new Random().Next(10, 30)));
            }
            else if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.NotFound)
            {
                _multiplexer.GetDatabase().StringSetAsync(cacheKey, "NULL",
                    TimeSpan.FromSeconds(30));
            }
        }
    }
    
}
