using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDTS.DataAccess.Interface;
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
        private readonly IUserRepository _user;
        IMockData mock;
        public UserController(IMockData data, IUserRepository user)
        {
            mock = data;
            _user = user;
        }

        [HttpGet]
        [Route("getuserinfo")]
        public IActionResult GetUserInfo()
        {
            //var account = HttpContext.User.Claims.First(p => p.Type.Equals("Account")).Value;
            //var user = mock.getuser(account);
            //return Ok(user);

            var account = HttpContext.User.Claims.First(p => p.Type.Equals("Account")).Value;
            var user = _user.GetUser(account);
            return Ok(user);
        }

        
    }
}
