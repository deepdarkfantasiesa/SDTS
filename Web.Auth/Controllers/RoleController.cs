using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.Auth.Application.Commands;

namespace Web.Auth.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IMediator _mediator;
        public RoleController(ILogger<RoleController> logger,IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateRoleCommand createRoleCommand)
        {
            var result = await _mediator.Send(createRoleCommand);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {

            return Ok();
        }
    }

    
}
