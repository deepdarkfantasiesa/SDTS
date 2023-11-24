using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.Auth.Application.Commands;
using Web.Auth.Filters;

namespace Web.Auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMediator _mediator;

        public UserController(ILogger<UserController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        //注册
        [HttpPost("Post")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand registry)
        {
            var res = await _mediator.Send(registry);
            if (res)
            {
                _logger.LogInformation($"user {registry.Account} 注册成功");
                return Ok(res);
            }
            else
            {
                _logger.LogError($"user {registry.Account} 注册失败");
                return BadRequest();
            }
        }

        //登录
        [HttpPost("Get")]
        [TypeFilter(typeof(QueryUserFilter))]
        public async Task<IActionResult> Login([FromBody] LoginCommand loginCommand)
        {
            var res = await _mediator.Send(loginCommand);
            return Ok(res);
        }

        [HttpDelete("Delete")]
        [ServiceFilter(typeof(QueryUserFilter))]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserCommand deleteUserCommand)
        {
            var result = await _mediator.Send(deleteUserCommand);
            if(result)
            {
                _logger.LogInformation($"user: {deleteUserCommand.Account} 删除成功");
                return Ok();
            }
            else 
            {
                _logger.LogError($"user: {deleteUserCommand.Account} 删除失败");
                return BadRequest();
            }
        }
    }
}
