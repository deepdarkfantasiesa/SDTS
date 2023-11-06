using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Web.Auth.Options;
using Web.Auth.RepositoriesAndContexts;

namespace Web.Auth.Application.Commands
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, bool>
    {
        private readonly ILogger _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public RegisterCommandHandler(ILogger<RegisterCommandHandler> logger,
            UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }
        public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = new User() { CreateTime = DateTime.Now, UserName = request.Account, PasswordHash = request.Password };
            var result = await _userManager.CreateAsync(user);
            if(result.Succeeded)
            {
                _logger.LogInformation("創建用戶{username}成功", user.UserName);
                return true;
            }
            else
            {
                _logger.LogError("創建用戶失敗");
                return false;
            }
            
        }
    }
}
