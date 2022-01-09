using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Forms.GoogleMaps;

namespace SDTS.BackEnd.Controllers
{

    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class SecureAreaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        int i = 0;
        [HttpPost]
        //[HttpGet]
        [Route("postarea")]
        public IActionResult PostArea( SecureArea Area)
        {
            var userid = HttpContext.User.Claims.First(p => p.Type.Equals("UserID")).Value;
            
            Area.id = i++;
            Area.creater = userid;
            //此处需要执行数据库插入操作，然后查询返回此area返回给客户端
            //area.id = i++;//这个实际获取需要通过数据库查询返回
            return Ok(Area);
        }
    }
}
