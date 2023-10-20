using Microsoft.AspNetCore.Mvc;
using user_rpcservices;

namespace Web.User.HttpAggregator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserGrpc.UserGrpcClient _userGrpcClient;
        public UserController(UserGrpc.UserGrpcClient client)
        {
            _userGrpcClient = client;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _userGrpcClient.CreateUser(new CreateUserCommand());
            return Ok();
        }
    }
}
