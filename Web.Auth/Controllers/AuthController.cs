using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Auth.Application.Commands;
using Web.Auth.RepositoriesAndContexts;

namespace Web.Auth.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        
        private readonly ILogger<AuthController> _logger;
        private readonly IMediator _mediator;

        public AuthController(ILogger<AuthController> logger,IMediator mediator)
        {
            
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromQuery]string account, [FromQuery] string password)
        {
            var res = await _mediator.Send(new RegisterCommand(account,password));
            if(res)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromQuery] string account, [FromQuery] string password)
        {
            var res = await _mediator.Send(new LoginCommand(account, password));
            return Ok(res);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> auth()
        {
            var user = this.User;
            return Ok("success");
        }
    }
}
