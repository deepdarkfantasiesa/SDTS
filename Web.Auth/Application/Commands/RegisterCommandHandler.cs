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

        public RegisterCommandHandler(ILogger<RegisterCommandHandler> logger,
            UserManager<User> userManager)
        {
            _userManager = userManager;
            _logger = logger;
        }
        public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var userfromdb = await _userManager.FindByNameAsync(request.Account);
            if(userfromdb != null)
            {
                _logger.LogError($"user: {request.Account} 已经存在");
                return false;
            }
            var user = new User() { CreateTime = DateTime.Now, UserName = request.Account, PasswordHash = request.Password };
            var result = await _userManager.CreateAsync(user);

            await _userManager.AddToRoleAsync(user, request.RoleId.ToString());

            if (result.Succeeded)
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
