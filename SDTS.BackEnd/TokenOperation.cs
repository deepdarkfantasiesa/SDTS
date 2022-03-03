using Microsoft.IdentityModel.Tokens;
using Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SDTS.BackEnd
{
    public class TokenOperation
    {
        public TokenOperation()
        {

        }

        public string CreateTokenToUser(User user)
        {
            string key = "f47b558d-7654-458c-99f2-13b190ef0199";
            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            var claims = new Claim[]
            {
                new Claim("Account", user.Account),
                new Claim("Name", user.Name),
                new Claim("UserID", user.UserID.ToString()),
                new Claim("Age", (DateTime.Now.Year- user.Birthday.Year).ToString()),
                new Claim("Information", user.Information),
                new Claim("Type",user.Type),
                new Claim("Gender",user.Gender)
            };

            var token = new JwtSecurityToken(
                issuer: "http://localhost:5200",
                audience: "api",
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return jwtToken;
        }
    }
}
