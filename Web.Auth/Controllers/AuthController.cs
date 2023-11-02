using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Auth.Application.Commands;
using Web.Auth.RepositoriesAndContexts;

namespace Web.Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        public IActionResult Register([FromBody]string account, [FromBody]string password)
        {
            _mediator.Send(new RegisterCommand(account,password));
            return Ok();
        }
    }
}
