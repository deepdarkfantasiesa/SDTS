using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDTS.BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        [Route("index")]
        public IActionResult Index()
        {
            return Ok("welcome");
        }
    }
}
