using MediatR;
using Microsoft.AspNetCore.Mvc;
using User.API.Application.Commands;
using User.Domain.AggregatesModel.UserAggregate;
using Dapper;
using User.API.Application.Queries;
using System.Net;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

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
        [HttpGet("GetById")]
        [ProducesResponseType(typeof(string),200)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById([FromServices]IUserQueries userQueries, [FromServices] IDistributedCache distributedCache, int userid)
        {
            try
            {
                //var res = await userQueries.GetUserAsync(userid);
                //return Ok(res);

                var usercache = await distributedCache.GetStringAsync("GetById:" + userid);
                if (usercache != null)
                {
                    var res = JsonSerializer.Deserialize<User.API.Application.Queries.User>(usercache);
                    return Ok(res);
                }
                else
                {
                    var res = await userQueries.GetUserAsync(userid);
                    var cacheOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1))
                        .SetSlidingExpiration(TimeSpan.FromSeconds(new Random().Next(10, 20)));
                    await distributedCache.SetStringAsync("GetById:" + userid, 
                        JsonSerializer.Serialize<User.API.Application.Queries.User>(res), cacheOptions);
                    return Ok(res);
                }
            }
            catch(Exception ex)
            {
                return NotFound();
            }
        }
    }
}
