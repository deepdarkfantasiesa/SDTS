using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace Web.Gateway.Middlewares
{
    public class JwtSafeMiddleware
    {
        private readonly RequestDelegate _next;
        public IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        public JwtSafeMiddleware(RequestDelegate next, IConfiguration configuration,IHttpClientFactory httpClientFactory)
        {
            _next = next;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method == "GET" || context.Request.Method == "POST")
            {
                string myToken = context.Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(myToken))
                {
                    context.Response.StatusCode = 401; //401未授权
                    await context.Response.WriteAsync("token is null");
                    return;
                }
                //校验auth的正确性
                var url = _configuration.GetSection("AuthUrl").Value;

                var remoteConnection = context.Request.HttpContext.Connection;
                var IpAndPort = remoteConnection.RemoteIpAddress + ":" + remoteConnection.RemotePort;
                Console.WriteLine(IpAndPort);
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", myToken);
                client.DefaultRequestHeaders.Add("IpAndPort", IpAndPort);
                var res = await client.GetStringAsync(url + "/Auth/auth");//https://localhost:7284 http://localhost:5041
                if(res == "success")
                {
                    await _next.Invoke(context);
                }
                
            }
        }
    }
}
