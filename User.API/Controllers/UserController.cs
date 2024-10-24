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

        [HttpGet("RedisTest")]
        public async Task<IActionResult> RedisTest([FromServices] RedisConnectionPool redisConnectionPool)
        {
            using (var redisConnection= redisConnectionPool.GetConnection())
            {
                var connection = redisConnection.Connection;
                Console.WriteLine(redisConnectionPool.CurrentConnectionCount);
            }
            
            return Ok();
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
                    await client.GetAsync("http://localhost:5002/User/RedisTest/RedisTest");
                }));
			}
            await Task.WhenAll(tasks);
            return Ok();
        }
    }
}
