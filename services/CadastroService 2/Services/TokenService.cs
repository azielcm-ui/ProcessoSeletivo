using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Cadastro.API.Domain;
using Cadastro.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Cadastro.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    public TokenService(IConfiguration config) => _config = config;

    public UserToken GenerateAccessToken(ApplicationUser user)
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
        var aux = new JwtSecurityTokenHandler().WriteToken(token);
        return new UserToken(aux, expires);
    }

    public UserTokenRefresh RefreshToken(string refreshToken)
    {
        string aux = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        return new UserTokenRefresh(aux);
    }
}
