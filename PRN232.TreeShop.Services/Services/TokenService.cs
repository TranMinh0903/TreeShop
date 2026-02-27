using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PRN232.LaptopShop.Repo.Entities;

namespace PRN232.LaptopShop.Services.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(Account account);
    }
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateAccessToken(Account account)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));
            var expiryMinutes = _configuration["JwtSettings:ExpTime"];

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      
            var claims = new[]
            {
                new Claim("Id", account.Id.ToString()),
                new Claim("Username", account.Username ?? string.Empty),
                new Claim(ClaimTypes.Role, account.Role?.ToString() ?? "User")
            };

            var token = new JwtSecurityToken
            (
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(expiryMinutes!)),
                signingCredentials: credentials,
                audience: _configuration["JwtSettings:Audience"],
                issuer: _configuration["JwtSettings:Issuer"]
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
