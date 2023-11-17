using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Web.Auth.SecurityTokenValidators
{
    public class CustomTokenValidtor : ISecurityTokenValidator
    {
        public bool CanValidateToken
        {
            get
            {
                return true;
            }
        }

        public int MaximumTokenSizeInBytes
        {
            get
            {
                return _maxTokenSizeInBytes;
            }

            set
            {
                _maxTokenSizeInBytes = value;
            }
        }

        private int _maxTokenSizeInBytes = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        private JwtSecurityTokenHandler _tokenHandler;
        private ILogger<CustomTokenValidtor> _logger;
        private IServiceProvider _serviceProvider;
        public CustomTokenValidtor(ILogger<CustomTokenValidtor> logger,IHttpContextAccessor httpContext,IServiceProvider provider)
        {
            _logger = logger;
            _serviceProvider = provider;
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public bool CanReadToken(string securityToken)
        {
            if (!string.IsNullOrEmpty(securityToken))
            {
                return true;
            }
            return false;
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            try
            {
                var httpContext = _serviceProvider.GetService<IHttpContextAccessor>().HttpContext;
                var originRequestIpAndPort = httpContext.Request.Headers["IpAndPort"].FirstOrDefault();
                Console.WriteLine(originRequestIpAndPort);

                var principal = _tokenHandler.ValidateToken(securityToken, validationParameters, out validatedToken);
                return principal;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                validatedToken = null;
                return null;
            }
            //catch (SecurityTokenExpiredException exception)
            //{
            //    _logger.LogError(exception.ToString());
            //    validatedToken = null;
            //    return null;
            //}
            //catch (SecurityTokenException exception)
            //{
            //    _logger.LogError(exception.ToString());
            //    validatedToken = null;
            //    return null;
            //}
        }
    }
}
