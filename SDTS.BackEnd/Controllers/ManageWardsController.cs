using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDTS.DataAccess.Interface;
using System.Linq;
using System.Threading.Tasks;

namespace SDTS.BackEnd.Controllers
{
    [Authorize(Policy = "guardianspolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class ManageWardsController : Controller
    {
        public ManageWardsController(IUserRepository user,IComplexRepository complex)
        {
            _user = user;
            _complex = complex;
        }
        private readonly IUserRepository _user;
        private readonly IComplexRepository _complex;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("getwards")]
        public IActionResult GetWards()
        {
            var useraccount = HttpContext.User.Claims.First(p => p.Type.Equals("Account")).Value;
            var wards = _user.GetWards(useraccount);
            return Ok(wards);

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
        public async Task<IActionResult> AddWard(int code)
        {
            var guardianaccount = HttpContext.User.Claims.First(p => p.Type.Equals("Account")).Value;
            var result =await _complex.AddWard(guardianaccount, code.ToString());
            return Ok(result);
        }

        [HttpPut]
        [Route("removeward")]
        public async Task<IActionResult> RemoveWard(int code,string wardaccount)
        {
            var guardianaccount = HttpContext.User.Claims.First(p => p.Type.Equals("Account")).Value;
            var result =await _complex.RemoveWard(guardianaccount, wardaccount, code.ToString());
            return Ok(result);
        }
    }
}
