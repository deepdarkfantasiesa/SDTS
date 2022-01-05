using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SDTS.BackEnd
{
    public class TokenValidtor : ISecurityTokenValidator
    {
        public bool CanValidateToken => throw new NotImplementedException();

        public int MaximumTokenSizeInBytes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CanReadToken(string securityToken)
        {
            if (!string.IsNullOrEmpty(securityToken))
            {
                return true;
            }
            return false;
        }

        ClaimsPrincipal ISecurityTokenValidator.ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            //var token = new JwtSecurityTokenHandler().Token(securityToken);
            string key = "f47b558d-7654-458c-99f2-13b190ef0199";
            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            ClaimsPrincipal claimsPrincipal = null;

            var validateParameter = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "http://localhost:5200",
                ValidAudience = "api",
                IssuerSigningKey = securityKey
            };
            //try
            //{
            //校验并解析token
            claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(securityToken, validateParameter, out validatedToken);//validatedToken:解密后的对象
            var jwtPayload = ((JwtSecurityToken)validatedToken).Payload.SerializeToJson();

            //var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(securityToken);
            //}
            //catch(Exception ex)
            //{

            //}
            //validatedToken = null;
            return claimsPrincipal;
            //var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
            //identity.AddClaim(new Claim("Age", "18"));
            //var principal = new ClaimsPrincipal(identity);
            //return principal;
        }
    }
}
