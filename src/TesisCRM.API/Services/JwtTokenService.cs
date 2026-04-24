using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TesisCRM.API.Models.Users;

namespace TesisCRM.API.Services;

public class JwtTokenService
{
    private readonly IConfiguration _configuration;
    public JwtTokenService(IConfiguration configuration) => _configuration = configuration;

    public string Generate(UserAuthDto user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.RoleCode),
            new Claim("fullName", user.FullName)
        };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
