using Microsoft.AspNetCore.Mvc;
using Models;
using SDTS.DataAccess.Interface;

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

        public SignController(IUserRepository user)
        {
            _user = user;
        }

        [HttpGet]
        [Route("signin")]
        public IActionResult SignIn(string account, string password)
        {
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
            var cuser = _user.SignUp(user);
            return Ok(cuser);
        }
    }
}
