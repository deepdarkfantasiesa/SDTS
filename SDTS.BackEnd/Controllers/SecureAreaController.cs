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
    [Authorize(Policy = "guardianspolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class SecureAreaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly ISecureAreaRepository _securearea;
        private readonly IUserRepository _user;
        public SecureAreaController(ISecureAreaRepository securearea, IUserRepository user)
        {
            _securearea = securearea;
            _user = user;
        }

        [HttpPut]
        [Route("ctreatearea")]
        public async Task<IActionResult> CreateArea(SecureArea Area)
        {
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
