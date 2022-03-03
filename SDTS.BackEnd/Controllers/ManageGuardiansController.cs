using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SDTS.DataAccess.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SDTS.BackEnd.Controllers
{
    [Authorize(Policy = "wardspolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class ManageGuardiansController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public ManageGuardiansController(IUserRepository user)
        {
            _user = user;
        }
        private readonly IUserRepository _user;

        [HttpGet]
        [Route("getinvitationcode")]
        public IActionResult GetInvitationCode()
        {
            var account = HttpContext.User.Claims.First(p => p.Type.Equals("Account")).Value;
            Random random = new Random();
            var code = random.Next(1000, 9999);
            var result = _user.Invite(account, code.ToString());
            return Ok(result);

        }

        [HttpGet]
        [Route("getguardians")]
        public async Task<IActionResult> GetGuardians()
        {
            var account = HttpContext.User.Claims.First(p => p.Type.Equals("Account")).Value;
            var result = await _user.GetGuardiansAsync(account);//ssd
            return Ok(result);
        }
    }
}
