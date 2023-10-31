using MediatR;
using Microsoft.AspNetCore.Mvc;
using User.API.Application.Commands;
using User.Domain.AggregatesModel.UserAggregate;
using Dapper;
using User.API.Application.Queries;
using System.Net;

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
        public async Task<IActionResult> Create(CreateUserCommand command)
        {
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
        public async Task<IActionResult> GetById([FromServices]IUserQueries userQueries,int userid)
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
    }
}
