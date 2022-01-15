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

    //[Authorize(JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "guardianspolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class SecureAreaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        IMockData mock;
        public SecureAreaController(IMockData data)
        {
            mock = data;
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
            Area.id = random.Next(101);//这个实际获取需要通过数据库查询返回
            mock.addarea(Area);
            var newarea = mock.getarea(Area.id);//此处需要执行数据库插入操作，然后查询返回此area返回给客户端
            return Ok(newarea);
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
            var result= mock.alterarea(Area);
            if(result==true)
            {
                var newarea = mock.getarea(Area.id);

                return Ok(newarea);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("deletearea")]
        public IActionResult DeleteArea(SecureArea area)
        {
            //执行数据库操作
            var result= mock.deletearea(area);
            return Ok(result);
        }

        [HttpGet]
        [Route("getareas")]
        public IActionResult GetAreas(int wardid)
        {
            var areas = mock.getareas(wardid);
            return Ok(areas);
        }
    }
}
