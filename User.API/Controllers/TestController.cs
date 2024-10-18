using Microsoft.AspNetCore.Mvc;
using Service.Framework.ServiceRegistry;

namespace User.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController : ControllerBase
    {
        private readonly IRegistryService _consulServices;
        private readonly IConfiguration _configuration;
        public TestController(IRegistryService consulServices,IConfiguration configuration)
        {
            _consulServices = consulServices;
            _configuration = configuration;
        }

        [HttpGet] 
        public async Task<IActionResult> GetServices() 
        {
            var urls = await _consulServices.RequestServices();
            foreach (var url in urls)
            {
                Console.WriteLine($"address {url}");
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
