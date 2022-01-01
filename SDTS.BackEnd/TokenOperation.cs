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
        public string CreateToken(User? user)
        {
            string key = "f47b558d-7654-458c-99f2-13b190ef0199";
            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, "cqf")
                //new Claim("Age", (DateTime.Now.Year- user.birthday.Year).ToString()),
                //new Claim("Information", user.Information),
                //new Claim("Type",user.type),
                //new Claim("Gender",user.gender),
                //new Claim(JwtRegisteredClaimNames.Email, "2752154844@qq.com"),
                //new Claim(JwtRegisteredClaimNames.Sub, "D21D099B-B49B-4604-A247-71B0518A0B1C"),
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
