using CadastroService.Domain;

namespace Cadastro.Application;

public interface ITokenService
{
    Task<(string accessToken, string refreshToken)> GenerateTokensAsync(ApplicationUser user);
    Task<(string accessToken, string refreshToken)> RefreshAsync(string refreshToken);
}
