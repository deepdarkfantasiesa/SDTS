using Auth.API.Application.Commands.RoleAggregate;
using Auth.API.Application.Queries.Role.Page;
using Infrastructure.Core.Query.Page;
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
        /// 新建角色
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
        /// <returns></returns>
        [HttpGet]
        public async Task<PageResponse<QueryResult>> Page([FromBody]PageRequest<QueryCondition> request)
        {
            var query = new PageRoleQuery
            {
                Params = request,
                UseReplica = true
            };
            return await _sender.Send<PageResponse<QueryResult>>(query);
        }
    }
}
