using Auth.API.Application.Commands.RoleAggregate;
using Auth.API.Application.Queries.Role.Page;
//using Domain.Abstraction.Mediator;
using Infrastructure.Core.Cache;
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
        private readonly IMediator _sender;

        public RoleController(IMediator sender)
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
            //return await _sender.Send<bool>(command);
            return false;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<bool> Update([FromBody] UpdateRoleCommand command)
        {
            //return await _sender.Send<bool>(command);
            return false;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<PageResponse<QueryResult>> Page([FromHeader] bool? useReplica, [FromHeader] QueryCacheLevel? cacheLevel, [FromBody] PageRequest<QueryCondition> request)
        {
            var query = new PageRoleQuery
            {
                Params = request,
                UseReplica = useReplica.HasValue ? useReplica.Value : true,
                PreferCacheLevel = cacheLevel.HasValue ? cacheLevel.Value : QueryCacheLevel.None,
                KeyContext = request.GetCacheKeyContext()
            };
            return await _sender.Send<PageResponse<QueryResult>>(query);
        }
    }
}
