using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Linq;
using System;
using System.Text;

namespace datecounter.Services{
    public interface IJwtService
    {
        string generateToken(string userId);
        string getUserId(string token);
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration config;

        public JwtService(IConfiguration _config)
        {
            config = _config;
        }


        public string generateToken(string userId)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new Claim[]{
                        new Claim(JwtRegisteredClaimNames.Sub,userId.ToString())
                    };

            JwtSecurityToken token = new JwtSecurityToken(
                config["JWT:Issuer"],
                config["JWT:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string getUserId(string token)
        {
            JwtSecurityToken decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return decodedToken.Payload.Sub;
        }
    }
}