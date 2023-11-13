using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Service.Framework.ConsulRegister;
using System.Net.Security;
using user_rpcservices;
using Web.User.HttpAggregator.LoadBalancer;
using static user_rpcservices.UserGrpc;

namespace Web.User.HttpAggregator.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly UserGrpc.UserGrpcClient _userGrpcClient;
        private readonly IConsulServices _consulServices;
        public UserController(UserGrpc.UserGrpcClient client, IConsulServices consulServices)
        {
            _userGrpcClient = client;
            _consulServices = consulServices;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string name, [FromServices] IRoundRobin roundRobin)
        {
            var res = _userGrpcClient.CreateUser(new CreateUserCommand() { UserName = name });
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetByConsul([FromQuery]string name, [FromServices]IRoundRobin roundRobin)
        {
            Console.WriteLine(name);
            var urls = await _consulServices.RequestServicesV2("User");//服务发现
            var url = await roundRobin.Load(urls);
            //return Ok(url);
            var channel = GrpcChannel.ForAddress("https://" + url, new GrpcChannelOptions()
            {
                HttpHandler = new SocketsHttpHandler()
                {
                    SslOptions = new SslClientAuthenticationOptions()
                    {
                        RemoteCertificateValidationCallback = (a, b, c, d) => true
                    }
                }
            });
            var client = new UserGrpcClient(channel);
            var res = client.CreateUser(new CreateUserCommand() { UserName = name });
            return Ok(res);
        }
    }
}
