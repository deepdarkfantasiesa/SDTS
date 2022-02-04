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

        public ManageGuardiansController(IMockData data, IUserRepository user)
        {
            mock = data;
            _user = user;
        }
        IMockData mock;
        private readonly IUserRepository _user;

        [HttpGet]
        [Route("getinvitationcode")]
        public IActionResult GetInvitationCode()
        {
            //var account = HttpContext.User.Claims.First(p => p.Type.Equals("Account")).Value;
            //Random random = new Random();
            //var code = random.Next(1000,9999);
            //var result = mock.invite(account, code);
            //return Ok(result);

            var account = HttpContext.User.Claims.First(p => p.Type.Equals("Account")).Value;
            Random random = new Random();
            var code = random.Next(1000, 9999);
            var result = _user.Invite(account, code.ToString());
            return Ok(result);

        }

        [HttpGet]
        [Route("getguardians")]
        public IActionResult GetGuardians()
        {
            //var account = HttpContext.User.Claims.First(p => p.Type.Equals("Account")).Value;
            //var result = mock.getguardians(account);
            //return Ok(result);
            var account = HttpContext.User.Claims.First(p => p.Type.Equals("Account")).Value;
            var result = _user.GetGuardians(account);
            return Ok(result);
        }
    }
}
