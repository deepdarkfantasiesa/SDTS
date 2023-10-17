using MediatR;
using Microsoft.AspNetCore.Mvc;
using User.API.Application.Commands;
using User.Domain.AggregatesModel.UserAggregate;

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
    }
}
