using Microsoft.IdentityModel.Tokens;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
            string key = "f47b558d-7654-458c-99f2-13b190ef0199";
            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            ClaimsPrincipal claimsPrincipal = null;
            validatedToken = null;

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
            try
            {
                //校验并解析token
                claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(securityToken, validateParameter, out validatedToken);//validatedToken:解密后的对象
                var jwtPayload = ((JwtSecurityToken)validatedToken).Payload.SerializeToJson();
                return claimsPrincipal;
            }
            catch (SecurityTokenExpiredException stee)
            {
                //表示过期
                Debug.WriteLine(stee);
                return claimsPrincipal;
            }
            catch (SecurityTokenException ste)
            {
                //表示token错误
                Debug.WriteLine(ste);
                return claimsPrincipal;
            }
        }
    }
}
