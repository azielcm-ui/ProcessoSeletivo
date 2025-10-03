using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Cadastro.Application;
using CadastroService.Domain;

namespace Cadastro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
        var res = await _userManager.CreateAsync(user, dto.Password);
        if (!res.Succeeded) return BadRequest(res.Errors);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) return Unauthorized();
        var res = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!res.Succeeded) return Unauthorized();

        var tokens = await _tokenService.GenerateTokensAsync(user);
        return Ok(new { accessToken = tokens.accessToken, refreshToken = tokens.refreshToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto dto)
    {
        try
        {
            var tokens = await _tokenService.RefreshAsync(dto.RefreshToken);
            return Ok(new { accessToken = tokens.accessToken, refreshToken = tokens.refreshToken });
        }
        catch
        {
            return Unauthorized();
        }
    }
}

public record RegisterDto(string Email, string Password);
public record LoginDto(string Email, string Password);
public record RefreshDto(string RefreshToken);
