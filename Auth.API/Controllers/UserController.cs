using Auth.API.Application.Commands.UserAggregate;
using Domain.Abstraction.Mediator;


//using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController: ControllerBase
    {
        private readonly IMediator _sender;

        public UserController(IMediator sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<bool> Create([FromBody] CreateUserCommand command)
        {
            return await _sender.SendAsync(command);
        }
    }
}
