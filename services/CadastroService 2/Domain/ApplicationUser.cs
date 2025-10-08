using Microsoft.AspNetCore.Identity;

namespace Cadastro.Domain;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string? Password { get; set; }
}

