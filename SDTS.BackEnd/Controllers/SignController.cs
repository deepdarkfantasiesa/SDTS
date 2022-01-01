using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SDTS.BackEnd.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignController : Controller
    {
        [Route("index")]
        public IActionResult Index()
        {
            return Ok("index");
        }

        //[HttpPost]
        [HttpGet]
        //[HttpPut]
        //[RequireHttps]
        [Route("signin")]
        public IActionResult SignIn(string username, string password)
        {
            string token;
            //此处需要查找数据库，若查询到，则创建token并返回给客户端
            //if(user!=null)
            //{
            //    TokenOperation createToken = new TokenOperation();
            //    token= createToken.CreateToken(user);
            //}
            //else
            //{
            //    return null;
            //}
            TokenOperation createToken = new TokenOperation();
            token = createToken.CreateToken(null);
            return Ok(token);
        }
    }
}
