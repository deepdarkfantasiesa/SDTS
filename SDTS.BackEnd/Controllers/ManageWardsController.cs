using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SDTS.BackEnd.Controllers
{
    //[Authorize(JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class ManageWardsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("getwards")]
        public IActionResult GetWards()
        {
            var userid= HttpContext.User.Claims.First(p => p.Type.Equals("UserID")).Value;
            MockData data = new MockData();
            //以下方法只是模拟数据，真实场景需要查数据库
            var wards= data.getwards(int.Parse(userid));
            return Ok(wards);
        }
    }
}
