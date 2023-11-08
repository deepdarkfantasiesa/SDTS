using Microsoft.AspNetCore.Mvc;
using Service.Framework.ConsulRegister;

namespace User.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IConsulServices _consulServices;
        public TestController(IConsulServices consulServices)
        {
            _consulServices = consulServices;
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
    }
}
