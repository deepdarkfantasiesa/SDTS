using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Collections.Generic;
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

        [HttpPost]
        //[HttpGet]
        //[HttpPut]
        //[RequireHttps]
        [Route("signin")]
        public HttpResponseMessage SignIn([FromBody] User user)
        {
            string token;
            //此处需要查找数据库，若查询到，则创建token并返回给客户端
            if(user!=null)
            {
                TokenOperation createToken = new TokenOperation();
                token= createToken.CreateToken(user);
            }
            else
            {
                return null;
            }

            HttpResponseMessage result = new HttpResponseMessage();
            result.Content.Headers.Add("token", token);
            
            return result;
        }
    }
}
