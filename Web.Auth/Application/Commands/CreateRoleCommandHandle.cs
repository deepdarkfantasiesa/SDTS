using MediatR;
using Microsoft.AspNetCore.Identity;
using Web.Auth.RepositoriesAndContexts;

namespace Web.Auth.Application.Commands
{
    public class CreateRoleCommandHandle:IRequestHandler<CreateRoleCommand,bool>
    {
        private readonly ILogger<CreateRoleCommandHandle> _logger;
        private readonly RoleManager<Role> _roleManager;
        public CreateRoleCommandHandle(ILogger<CreateRoleCommandHandle> logger,RoleManager<Role> roleManager)
        {
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task<bool> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByNameAsync(request.Name);
            if (role != null)
            {
                _logger.LogError($"role: {request.Name} 已经存在");
                return false;
            }
            var result = await _roleManager.CreateAsync(new Role() { Name = request.Name });
            if (result.Succeeded)
            {
                _logger.LogInformation($"role: {request.Name} 创建成功");
                return true;
            }
            else
            {
                _logger.LogError($"role: {request.Name} 创建失败");
                return false;
            }
        }
    }
}
