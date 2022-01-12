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
    [Authorize(JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class ManageWardsController : Controller
    {
        public ManageWardsController(IMockData data)
        {
            mock = data;
        }
        IMockData mock;

        public IActionResult Index()
        {
            return View();
        }


        //MockData data = new MockData();
        //[Authorize(JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("getwards")]
        public IActionResult GetWards()
        {
            var userid= HttpContext.User.Claims.First(p => p.Type.Equals("UserID")).Value;
            //以下方法只是模拟数据，真实场景需要查数据库,返回部分需要显示的数据即可，详细数据再GetDetail请求
            //var wards = data.getwards(int.Parse(userid));
            var wards = mock.getwards(int.Parse(userid));
            
            return Ok(wards);
        }

        [HttpGet]
        [Route("getdetail")]
        public IActionResult GetDetail(int userid)
        {
            //MockData data = new MockData();//ccchhh
            var ward = mock.getdetail(userid);
            return Ok(ward);
        }
    }
}
