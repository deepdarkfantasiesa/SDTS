using Auth.API.Application.Commands;
using Auth.API.Application.Queries.User;
using Auth.API.Filters;
using Auth.Domain.AggregatesModel.UserAggregate;
using Auth.Infrastructure;
using Auth.Infrastructure.Caches;
using Auth.Infrastructure.Repositories;
using Infrastructure.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Service.Framework.Models;
using StackExchange.Redis;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;


namespace Auth.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private string Check(CacheKeyContext keyContexts)
        {
            if (keyContexts.Count == 0)
                throw new ArgumentNullException("缓存键上下文为空");

            StringBuilder cacheKeyBuilder = new StringBuilder();

            foreach (var kvp in keyContexts)
            {
                cacheKeyBuilder.Append($"?{kvp.Key}={kvp.Value}");
            }
            return cacheKeyBuilder.ToString();
        }

        [HttpDelete]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task DeleteByTags([FromServices] ICacheImpl cache, [FromBody] CacheTag[] tags)
        {
            await cache.RemoveByTags(tags);
        }

        [HttpGet("{userid}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> QueryGetById([FromRoute] string userid, [FromHeader] QueryCacheLevel queryCacheLevel, [FromHeader] bool useReplica, [FromServices] ISender _sender, [FromServices] ICacheImpl cache)
        {
            try
            {
                //var result = await _mediator.Send(new GetUserByIdQuery { Id = userid });

                var res = await _sender.Send(new CheckUserExistsByQuery()
                {
                    Params = new CheckUserExistParams { UserName = userid.ToString() },
                    PreferCacheLevel = queryCacheLevel,
                    Generator = Check,
                    KeyContext = new CacheKeyContext
                    {
                        { "UserId",userid.ToString()}
                    },
                    Tags = new CacheTag[]
                    {
                        CacheTag.Query,
                        CacheTag.CheckIsExist
                    },
                    UseReplica = useReplica
                });

                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }

        [HttpGet("{userid}")]
        public async Task<IActionResult> DbContextGetById([FromServices] UserContext dbContext, [FromRoute] UserId userid, [FromServices] IUserRepo userRepo, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var res = await userRepo.GetByIdAsync(userid, cancellationToken);
            return Ok(res);
        }

        [HttpGet("RedisContext")]
        public async Task<IActionResult> TestRedisContext([FromServices] ICacheImpl _cacheImpl, [FromQuery] string cacheKey, [FromQuery] CacheLevel cacheLevel)
        {
            var cacheData = await _cacheImpl.GetStringAsync<IEnumerable<RelationalDatabaseModel>>(cacheKey, cacheLevel);
            return Ok(cacheData);
        }

        [HttpPost("RedisContext")]
        public async Task<IActionResult> TestRedisContext([FromServices] ICacheImpl _cacheImpl, [FromServices] ConnectionMultiplexer connection)
        {
            var db = connection.GetDatabase();
            var trans = db.CreateTransaction();
            var db1 = connection.GetDatabase(1);
            var trans1 = db1.CreateTransaction();
            await _cacheImpl.CommitTransactionAsync(trans);
            return Ok();
        }

        [HttpGet("TestRedisPub")]
        public async Task<IActionResult> TestRedisPub([FromServices] ICacheImpl _cacheImpl, [FromQuery] string cacheKey, [FromQuery] string value)
        {
            await _cacheImpl.PublishAsync(cacheKey, value);
            return Ok(null);
        }

        [HttpGet("TestCache")]
        public async Task<IActionResult> TestCache(ICacheImpl cache, [FromQuery] CacheLevel cacheLevel)
        {
            var data = await cache.GetStringAsync<IEnumerable<RelationalDatabaseModel>>(CacheKeyPrefix.PgSqlsConfig, cacheLevel);
            return Ok(data);
        }

        [HttpPost("TestPublish")]
        public async Task<IActionResult> TestSyncInMemoryCache1([FromServices] ICacheImpl cacheImpl, string key, string value)
        {
            var result = await cacheImpl.SetStringAsync(key, value);
            return Ok(result);
        }

        [HttpGet("TestPublish")]
        public async Task<IActionResult> TestSyncInMemoryCache2(IMemoryCache memoryCache, string key)
        {
            var data = memoryCache.Get<string>(key);
            return Ok(data);
        }

        [HttpGet("Test")]
        public async Task<IActionResult> Test([FromQuery] int parrelNum)
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < parrelNum; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    HttpClient client = new HttpClient();
                    //await client.GetAsync("http://localhost:5002/User/TestRedisConnectionPool/TestRedisConnectionPool");
                    await client.GetAsync("http://localhost:5002/User/TestRedisContext/TestRedisContext?cacheKey=CHGateway:Administrator:2");
                }));
            }
            await Task.WhenAll(tasks);
            return Ok();
        }

        public record lockDto
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }

        public record LockDto1
        {
            public string? Name { get; set; }
        }

        [HttpPost("TestDistributedLock/{name}")]
        [DistributedLockFilter(source: ParameterSource.Header, timeout: 100, lockProperties: [nameof(lockDto.Age)])]
        public async Task<IActionResult> TestDistributedLock([FromBody] lockDto dto, [FromQuery] LockDto1 dto1, [FromHeader] int age)
        {
            return Ok();
        }
    }
}
