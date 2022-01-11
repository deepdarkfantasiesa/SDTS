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

        //[HttpPost]
        //[Route("postarea")]
        [HttpPut]
        [Route("ctreatearea")]
        public IActionResult CreateArea( SecureArea Area)
        {
            var createrid = HttpContext.User.Claims.First(p => p.Type.Equals("UserID")).Value;
            var creatername = HttpContext.User.Claims.First(p => p.Type.Equals("Name")).Value;
            Random random = new Random();

            Area.createrid = createrid;
            Area.creatername = creatername;
            //此处需要执行数据库插入操作，然后查询返回此area返回给客户端
            Area.id = random.Next(101);//这个实际获取需要通过数据库查询返回
            return Ok(Area);
        }

        [HttpPost]
        [Route("alterarea")]
        public IActionResult AlterArea(SecureArea Area)
        {
            Area.createtime = DateTime.Now;

            var createrid = HttpContext.User.Claims.First(p => p.Type.Equals("UserID")).Value;
            var creatername = HttpContext.User.Claims.First(p => p.Type.Equals("Name")).Value;
            Area.createrid = createrid;
            Area.creatername = creatername;

            return Ok(Area);
        }

        [HttpDelete]
        [Route("deletearea")]
        public IActionResult DeleteArea(int areaid)
        {
            //执行数据库操作
            
            return Ok(true);
        }
    }
}
