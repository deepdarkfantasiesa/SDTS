using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Service.Framework.ConsulRegister;
using System.Net.Security;
using user_rpcservices;
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
        public async Task<IActionResult> Get([FromQuery]string name)
        {
            Console.WriteLine(name);
            var urls = await _consulServices.RequestServicesV2("User");//服务发现
            return Ok();
            //var channel = GrpcChannel.ForAddress("https://192.168.18.107:5020", new GrpcChannelOptions()
            //{
            //    HttpHandler = new SocketsHttpHandler()
            //    {
            //        SslOptions = new SslClientAuthenticationOptions() 
            //        { 
            //            RemoteCertificateValidationCallback = (a, b, c, d) => true 
            //        }
            //    }
            //});

            //var client = new UserGrpcClient(channel);
            //var res = client.CreateUser(new CreateUserCommand() { UserName = name });
            //return Ok(res);

            //var res = _userGrpcClient.CreateUser(new CreateUserCommand() { UserName = name });
            //return Ok(res);

        }
    }
}
