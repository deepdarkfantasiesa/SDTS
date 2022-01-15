using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDTS.BackEnd.Controllers
{
    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        IMockData mock;
        public UserController(IMockData data)
        {
            mock = data;
        }

        [HttpGet]
        [Route("getuserinfo")]
        public IActionResult GetUserInfo()
        {
            var account = HttpContext.User.Claims.First(p => p.Type.Equals("Account")).Value;
            var user = mock.getuser(account);
            return Ok(user);
        }

        
    }
}
