using Cadastro.API.Domain;
using Cadastro.Domain;

namespace Cadastro.Services;

public interface ITokenService
{
    public UserToken GenerateAccessToken(ApplicationUser user);
    public UserTokenRefresh RefreshToken(string refreshToken);

}

