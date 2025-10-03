using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CadastroService.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CadastroService.Services;

public class TokenService
{
    private readonly IConfiguration _config;
    public TokenService(IConfiguration config) => _config = config;

    public (string token, DateTime expiresAt) GenerateAccessToken(ApplicationUser user)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var minutes = int.Parse(jwtSettings["AccessTokenMinutes"] ?? "15");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(minutes);

        var token = new JwtSecurityToken(issuer: issuer, audience: audience, claims: claims, expires: expires, signingCredentials: creds);
        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
}
