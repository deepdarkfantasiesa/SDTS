using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using SDTS.DataAccess.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDTS.BackEnd.Controllers
{

    //[Authorize(JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "guardianspolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class SecureAreaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        IMockData mock;
        private readonly ISecureAreaRepository _securearea;
        private readonly IUserRepository _user;
        public SecureAreaController(IMockData data, ISecureAreaRepository securearea,IUserRepository user)
        {
            mock = data;
            _securearea = securearea;
            _user = user;
        }

        //[HttpPost]
        //[Route("postarea")]
        [HttpPut]
        [Route("ctreatearea")]
        public async Task<IActionResult> CreateArea(SecureArea Area)
        {
            //var createrid = HttpContext.User.Claims.First(p => p.Type.Equals("UserID")).Value;
            //var creatername = HttpContext.User.Claims.First(p => p.Type.Equals("Name")).Value;
            //Random random = new Random();

            //Area.createrid = createrid;
            //Area.creatername = creatername;
            //Area.id = random.Next(101);//这个实际获取需要通过数据库查询返回
            //mock.addarea(Area);
            //var newarea = mock.getarea(Area.id);//此处需要执行数据库插入操作，然后查询返回此area返回给客户端
            //return Ok(newarea);

            var createraccount = HttpContext.User.Claims.First(p => p.Type.Equals("Account")).Value;
            var creatername = HttpContext.User.Claims.First(p => p.Type.Equals("Name")).Value;
            Random random = new Random();
            var code = random.Next(10000, 99999);
            StringBuilder lat = new StringBuilder();
            StringBuilder lon = new StringBuilder();
            int temp = 0;
            foreach (var pos in Area.area)
            {

                lat.Append(pos.Latitude.ToString());
                lon.Append(pos.Longitude.ToString());
                temp++;
                if(temp!=Area.area.Count())
                {
                    lat.Append(",");
                    lon.Append(",");
                }
            }
            var areaid= DateTime.Now.ToString("yyyyMMddHHmmss") + code.ToString();
            Area.createraccount = createraccount;
            Area.creatername = creatername;
            Area.areaid = areaid;
            Area.Latitude = lat.ToString();
            Area.Longitude = lon.ToString();
            Area.area.Clear();

            SecureArea selectarea = null;
            if (await _securearea.AddareaAsync(Area) ==true)
            {
                selectarea = await _securearea.FindareaAsync(areaid);
                string lattemp = selectarea.Latitude;
                string lontemp = selectarea.Longitude;
                string[] latgroup=lattemp.Split(",");
                string[] longroup=lontemp.Split(",");
                MyPosition position;
                for (int i = 0; i < latgroup.Length; i++)
                {
                    position = new MyPosition() { Latitude = double.Parse(latgroup[i]), Longitude = double.Parse(longroup[i]) };
                    selectarea.area.Add(position);
                }
                selectarea.Latitude = null;
                selectarea.Longitude = null;
            }
            return Ok(selectarea);
        }

        [HttpPost]
        [Route("alterarea")]
        public async Task<IActionResult> AlterArea(SecureArea Area)
        {
            //Area.createtime = DateTime.Now;

            //var createrid = HttpContext.User.Claims.First(p => p.Type.Equals("UserID")).Value;
            //var creatername = HttpContext.User.Claims.First(p => p.Type.Equals("Name")).Value;
            //Area.createrid = createrid;
            //Area.creatername = creatername;
            //var result= mock.alterarea(Area);
            //if(result==true)
            //{
            //    var newarea = mock.getarea(Area.id);

            //    return Ok(newarea);
            //}
            //return BadRequest();

            Area.createtime = DateTime.Now;

            var createraccount = HttpContext.User.Claims.First(p => p.Type.Equals("Account")).Value;
            var creatername = HttpContext.User.Claims.First(p => p.Type.Equals("Name")).Value;
            Area.creatername = creatername;
            Area.createraccount = createraccount;
            var newarea =await _securearea.AlterareaAsync(Area);

            return Ok(newarea);

        }

        [HttpPost]
        [Route("deletearea")]
        public async Task<IActionResult> DeleteArea(string areaid)
        {
            //执行数据库操作
            //var result= mock.deletearea(area);
            //return Ok(result);

            var result = await _securearea.DeleteareaAsync(areaid);
            return Ok(result);
        }

        [HttpGet]
        [Route("getwardareas")]
        public async Task<IActionResult> GetWardAreas(string wardaccount)
        {
            var areas = await _securearea.FindWardAreasAsync(wardaccount);
            return Ok(areas);
        }

        [HttpGet]
        [Route("getwardsareas")]
        public async Task<IActionResult> GetWardsAreas(string guardianaccount)
        {
            var wards = _user.GetWards(guardianaccount);
            List<SecureArea> secureAreas=null;
            if(wards!=null)
            {
                secureAreas = new List<SecureArea>();
                foreach (var ward in wards)
                {
                    var areas = await _securearea.FindWardAreasAsync(ward.Account);
                    foreach(var area in areas)
                    {
                        secureAreas.Add(area);
                    }
                }
            }
            return Ok(secureAreas);
        }


    }
}
