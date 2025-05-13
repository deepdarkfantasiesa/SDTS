using Auth.API.Application.Commands.RoleAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers
{
    /// <summary>
    /// 角色接口
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class RoleController : ControllerBase
    {
        private readonly ISender _sender;

        public RoleController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// 新建
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> Create([FromBody] CreateRoleCommand command)
        {
            return await _sender.Send(command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<bool> UpdatePermission([FromBody] UpdateRolePermissionCommand command)
        {
            return await _sender.Send(command);
        }
    }
}
