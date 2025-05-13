using Auth.API.Application.Commands.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController: ControllerBase
    {
        private readonly ISender _sender;

        public UserController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<bool> Create([FromBody] CreateUserCommand command)
        {
            return await _sender.Send(command);
        }
    }
}
