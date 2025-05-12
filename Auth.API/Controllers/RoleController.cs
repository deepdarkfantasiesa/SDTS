using Auth.API.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class RoleController : ControllerBase
    {
        private readonly ISender _sender;

        public RoleController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("Create")]
        public async Task<bool> Create([FromBody] CreateRoleCommand command)
        {
            return await _sender.Send(command);
        }
    }
}
