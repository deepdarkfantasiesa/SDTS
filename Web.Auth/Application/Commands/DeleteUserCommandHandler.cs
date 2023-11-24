using MediatR;
using Microsoft.AspNetCore.Identity;
using Web.Auth.RepositoriesAndContexts;

namespace Web.Auth.Application.Commands
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly ILogger<DeleteUserCommandHandler> _logger;
        private readonly UserManager<User> _userManager;
        public DeleteUserCommandHandler(ILogger<DeleteUserCommandHandler> logger,UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Account);
            if(user == null)
            {
                _logger.LogInformation($"user: {request.Account} 不存在");
                return false;
            }
            else
            {
                _logger.LogInformation($"user: {request.Account} 存在");
                var result = await _userManager.DeleteAsync(user);
                return result.Succeeded;
            }
        }
    }
}
