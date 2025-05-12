using Microsoft.AspNetCore.Mvc;
using Service.Framework.ServiceRegistry;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController : ControllerBase
    {
        private readonly IRegistryService _consulServices;
        private readonly IConfiguration _configuration;
        public TestController(IRegistryService consulServices, IConfiguration configuration)
        {
            _consulServices = consulServices;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetServices()
        {
            //var urls = await _consulServices.RequestServices();
            var urls = await _consulServices.Discover("pgsql");
            foreach (var url in urls.Response)
            {
                Console.WriteLine($"address {url.Service.Address} port {url.Service.Port}");
            }
            return Ok(urls);
        }

        [HttpGet]
        public async Task<IActionResult> Getcfg(string name)
        {
            var res = _configuration.GetSection(name);
            return Ok(res);
        }
    }
}
