using Microsoft.AspNetCore.Identity;

namespace CadastroService.Domain;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
