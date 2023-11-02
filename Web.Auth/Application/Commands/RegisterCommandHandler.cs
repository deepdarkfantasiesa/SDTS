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
        private readonly IOptions<JWTOptions> _jwtOptions;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public RegisterCommandHandler(ILogger<RegisterCommandHandler> logger,IOptions<JWTOptions> jwtOptions,
            UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtOptions = jwtOptions;
            _logger = logger;
        }
        public Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            
            return Task.FromResult(true);
        }
    }
}
