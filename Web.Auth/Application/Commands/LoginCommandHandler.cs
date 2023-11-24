using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Web.Auth.Options;
using Web.Auth.RepositoriesAndContexts;

namespace Web.Auth.Application.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
    {
        private readonly ILogger _logger;
        private readonly JWTOptions _jwtOptions;
        private readonly UserManager<User> _userManager;
        public LoginCommandHandler(ILogger<RegisterCommandHandler> logger, JWTOptions jwtOptions,
            UserManager<User> userManager)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions;
            _logger = logger;
        }
        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Account);
            if (user == null)
            {
                return "账号不存在";
            }

            //var res = await _userManager.CheckPasswordAsync(user, request.Password);
            if(user.PasswordHash != request.Password)
            {
                return "密码错误";
            }
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            var token = TokenOperations.BuildToken(claims, _jwtOptions);

            return token;
        }
    }
}
