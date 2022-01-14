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
        IMockData mock;
        public TokenOperation(IMockData data)
        {
            mock = data;
        }

        public string CreateToken(string account,string password)
        {
            //将来连上数据库后需要注释掉
            //MockData data = new MockData();//ccchhh
            var user = mock.FindUser(account,password);
            if (user == null)
                return null;
            string key = "f47b558d-7654-458c-99f2-13b190ef0199";
            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            var claims = new Claim[]
            {
                //new Claim(ClaimTypes.Name, "cqf")
                //new Claim("Age", (DateTime.Now.Year- user.birthday.Year).ToString()),
                //new Claim("Information", user.Information),
                //new Claim("Type",user.type),
                //new Claim("Gender",user.gender),
                //new Claim(JwtRegisteredClaimNames.Email, "2752154844@qq.com"),
                //new Claim(JwtRegisteredClaimNames.Sub, "D21D099B-B49B-4604-A247-71B0518A0B1C"),
                new Claim("Account", user.Account),
                new Claim("Name", user.Name),
                //new Claim(ClaimTypes.Name, user.Name),
                new Claim("UserID", user.UserID.ToString()),
                new Claim("Age", (DateTime.Now.Year- user.Birthday.Year).ToString()),
                new Claim("Information", user.Information),
                new Claim("Type",user.Type),
                new Claim("Gender",user.Gender)
                //new Claim(JwtRegisteredClaimNames.Email, "2752154844@qq.com"),
                //new Claim(JwtRegisteredClaimNames.Sub, "D21D099B-B49B-4604-A247-71B0518A0B1C")
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
