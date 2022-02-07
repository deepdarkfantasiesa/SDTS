using Microsoft.AspNetCore.Mvc;
using Models;
using SDTS.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SDTS.BackEnd.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly IUserDataRepository _userDatas;
        public TestController(IUserDataRepository userDatas)
        {
            _userDatas = userDatas;
        }


        [HttpGet]
        [Route("testuserDatasadd")]
        public async Task<IActionResult> Add()
        {
            SensorsData userData = new SensorsData();
            userData.Account = "9";
            userData.BarometerData = 9;
            userData.ConnectionId = "9";
            var result = await _userDatas.AddUserDatasAsync(userData);


            return Ok(result);
        }

        [HttpGet]
        [Route("testuserDatasalter")]
        public async Task<IActionResult> Alter()
        {
            SensorsData userData = new SensorsData();
            userData.Account = "9";
            userData.BarometerData = 7;
            userData.ConnectionId = "9";
            var result = await _userDatas.AlterUserDatasAsync(userData);

            return Ok(result);
        }

        [HttpGet]
        [Route("testuserDatasdelete")]
        public async Task<IActionResult> Delete()
        {
            SensorsData userData = new SensorsData();
            userData.Account = "9";
            userData.BarometerData = 7;
            userData.ConnectionId = "9";
            var result = await _userDatas.DeleteUserDatasAsync(userData.ConnectionId);

            return Ok(result);
        }

        [HttpGet]
        [Route("testuserDatasquery")]
        public async Task<IActionResult> Query()
        {
            SensorsData userData = new SensorsData();
            userData.ConnectionId = "9";
            var result = await _userDatas.QueryUserDatasAsync(userData.ConnectionId);
            return Ok(result);
        }
    }
}
