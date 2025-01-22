using MediatR;
using Microsoft.AspNetCore.Mvc;
using User.API.Application.Commands;
using User.Domain.AggregatesModel.UserAggregate;
using Dapper;
using User.API.Application.Queries;
using System.Net;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using User.API.Filters;
using User.Infrastructure;
using Microsoft.EntityFrameworkCore;
using User.Infrastructure.Caches.Redis;
using User.Infrastructure.Caches;
using Microsoft.Extensions.Caching.Memory;
using Service.Framework.Models;
using MongoDB.Bson.IO;

namespace User.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        
        [HttpPost("CreateUser")]
        //public async Task<IActionResult> Create(CreateUserCommand command)
        public async Task<IActionResult> Create(string name)
        {
            CreateUserCommand command = new CreateUserCommand(name);
            await _mediator.Send(command);
            return Ok();
        }

        [HttpGet("GetUsers")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices]IUserQueries userQueries)
        {
            try
            {
                
                var res = await userQueries.GetAllUsers();
                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            
        }

        //[Route("{userid:int}")]
        [ServiceFilter(typeof(CacheFilter))]
        [HttpGet("GetById")]
        [ProducesResponseType(typeof(string),200)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById([FromServices]IUserQueries userQueries, int userid)
        {
            try
            {
                var res = await userQueries.GetUserAsync(userid);
                return Ok(res);
            }
            catch(Exception ex)
            {
                return NotFound();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromServices] UserContext dbContext, int id)
        {
            //var user = await dbContext.Users
            //    .Where(p => p.Id == id)
            //    .SingleOrDefaultAsync();

            //dbContext.Users.Remove(user);

            await dbContext.Users.Where(p => p.Id == id).ExecuteDeleteAsync();


			return await dbContext.SaveChangesAsync() > 0 ? Ok() : NotFound();
        }

        [HttpGet("QueryByDbContext")]
        public async Task<IActionResult> QueryByDbContext([FromServices]IDbContextFactory<QueryDbContext> dbContextFactory)
        {
            using (var queryContext=await dbContextFactory.CreateDbContextAsync())
            {
                var users = await queryContext.Users.ToListAsync();
                return Ok(users);
			}
        }

        [HttpGet("TestRedisContext")]
        public async Task<IActionResult> TestRedisContext([FromServices]ICacheImpl _cacheImpl, [FromQuery] string cacheKey, [FromQuery]bool preferInMemory)
        {
            var cacheData = await _cacheImpl.GetStringAsync<IEnumerable<RelationalDatabaseModel>>(cacheKey, preferLocal: preferInMemory);
            return Ok(cacheData);
		}

        [HttpGet("TestRedisPub")]
        public async Task<IActionResult> TestRedisPub([FromServices] ICacheImpl _cacheImpl, [FromQuery] string cacheKey, [FromQuery]string value)
        {
            await _cacheImpl.PublishAsync(cacheKey, value);
            return Ok(null);
		}

        [HttpGet("TestInMemoryCace")]
        public async Task<IActionResult> TestInMemoryCace(IMemoryCache memoryCache)
        {
            var data =  memoryCache.Get<IEnumerable<RelationalDatabaseModel>>(CacheKeyPrefix.PgSqlsConfig);
            return Ok(data);
		}

        [HttpPost("TestPublish")]
        public async Task<IActionResult> TestSyncInMemoryCache1([FromServices] ICacheImpl cacheImpl, string key, string value)
        {
            var result = await cacheImpl.SetStringAsync(key, value, publish: true);
            return Ok(result);
		}

		[HttpGet("TestPublish")]
		public async Task<IActionResult> TestSyncInMemoryCache2(IMemoryCache memoryCache, string key)
		{
			var data = memoryCache.Get<string>(key);
			return Ok(data);
		}

		[HttpGet("TestPublishRedis")]
		public async Task<IActionResult> TestSyncInMemoryCache2(ICacheImpl cacheImpl, string key)
		{
			var data = await cacheImpl.GetStringAsync<string>(key);
			return Ok(data);
		}

		[HttpGet("Test")]
        public async Task<IActionResult> Test([FromQuery] int parrelNum)
        {
            List<Task> tasks = new List<Task>();
            for(int i = 0; i < parrelNum; i++)
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
    }
}
