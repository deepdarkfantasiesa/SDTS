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

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> auth()
        {
            _logger.LogInformation($"{nameof(auth)}");
            var user = this.User;
            return Ok("success");
        }
    }
}
