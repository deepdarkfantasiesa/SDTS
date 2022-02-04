using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDTS.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SDTS.BackEnd.Controllers
{
    //[Authorize(JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "guardianspolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class ManageWardsController : Controller
    {
        public ManageWardsController(IMockData data, IUserRepository user)
        {
            mock = data;
            _user = user;
        }
        IMockData mock;
        private readonly IUserRepository _user;

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
            //var userid = HttpContext.User.Claims.First(p => p.Type.Equals("UserID")).Value;
            //var wards = mock.getwards(int.Parse(userid));
            //return Ok(wards);

            var useraccount = HttpContext.User.Claims.First(p => p.Type.Equals("Account")).Value;
            var wards = _user.GetWards(useraccount);
            return Ok(wards);

        }

        [HttpGet]
        [Route("getdetail")]
        public IActionResult GetDetail(int userid)
        {
            var ward = mock.getdetail(userid);
            return Ok(ward);


        }

        [HttpGet]
        [Route("getdetailwithaccount")]
        public IActionResult GetDetailWithAccount(string account)
        {
            var ward= _user.GetWardDetail(account);
            return Ok(ward);


        }

        [HttpPut]
        [Route("addward")]
        public IActionResult AddWard(int code)
        {
            var guardianid = HttpContext.User.Claims.First(p => p.Type.Equals("UserID")).Value;
            var result= mock.addward(int.Parse(guardianid),code);
            return Ok(result);
        }

        [HttpPut]
        [Route("removeward")]
        public IActionResult RemoveWard(int code,int wardid)
        {
            var wardaccount = mock.getdetail(wardid).Account;
            var guardianid = HttpContext.User.Claims.First(p => p.Type.Equals("UserID")).Value;
            var result = mock.removeward(int.Parse(guardianid), code, wardaccount);
            return Ok(result);
        }
    }
}
