using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDTS.BackEnd.Controllers
{

    //[Authorize(JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "wardspolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class ManageGuardiansController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public ManageGuardiansController(IMockData data)
        {
            mock = data;
        }
        IMockData mock;

        [HttpGet]
        [Route("getinvitationcode")]
        public IActionResult GetInvitationCode()
        {
            var account = HttpContext.User.Claims.First(p => p.Type.Equals("Account")).Value;
            //var user = mock.getuser(account);
            //return Ok(user);
            Random random = new Random();
            var code = random.Next(1000,9999);

            var result = mock.invite(account, code);

            return Ok(result);
        }

        [HttpGet]
        [Route("getguardians")]
        public IActionResult GetGuardians()
        {
            var account = HttpContext.User.Claims.First(p => p.Type.Equals("Account")).Value;
            //以下方法只是模拟数据，真实场景需要查数据库,返回部分需要显示的数据即可，详细数据再GetDetail请求
            //var wards = data.getwards(int.Parse(userid));
            //var wards = mock.getwards(int.Parse(userid));

            //return Ok(wards);
            var result = mock.getguardians(account);
            return Ok(result);
        }
    }
}
