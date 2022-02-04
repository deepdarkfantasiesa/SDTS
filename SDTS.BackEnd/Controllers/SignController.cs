using Microsoft.AspNetCore.Mvc;
using Models;
using SDTS.DataAccess.Interface;
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
        private readonly IUserRepository _user;


        [Route("index")]
        public IActionResult Index()
        {
            return Ok("index");
        }

        IMockData mock;
        public SignController(IMockData data, IUserRepository user)
        {
            mock = data;
            _user = user;
        }

        [HttpGet]
        [Route("signin")]
        public IActionResult SignIn(string account, string password)
        {

            //此处需要查找数据库，若查询到，则创建token并返回给客户端
            //if(user!=null)
            //{
            //
            //}
            //else
            //{
            //    return null;
            //}

            //string token;
            //TokenOperation createToken = new TokenOperation(mock);
            //token = createToken.CreateToken(account, password);
            //if(token!=null)
            //{

            //    return Ok(token);
            //}
            //else
            //{
            //    return BadRequest();
            //}

            var user = _user.SignIn(account, password);
            if(user!=null)
            {
                string token;
                TokenOperation createToken = new TokenOperation();
                token = createToken.CreateTokenToUser(user);
                return Ok(token);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("signup")]
        public IActionResult Signup(User user)
        {
            //var result = mock.signup(user);
            //if (result.Equals("注册成功"))
            //{

            //    return Ok(result);
            //}
            //else
            //{

            //    return BadRequest(result);
            //}
            var cuser = _user.SignUp(user);
            return Ok(cuser);

        }
    }
}
