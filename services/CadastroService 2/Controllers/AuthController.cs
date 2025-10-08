using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Cadastro.Services;
using Cadastro.Domain;

namespace Cadastro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly TokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, TokenService tokenService)
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

        var tokens = _tokenService.GenerateAccessToken(user);
        return Ok( new { accessToken = tokens.Token, refreshToken = tokens.DtExpira });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto dto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized();
            var tokens = _tokenService.RefreshToken(dto.RefreshToken);
            return Ok( new { accessToken = tokens.Token});
        }
        catch
        {
            return Unauthorized();
        }
    }
}

public record RegisterDto(string Email, string Password);
public record LoginDto(string Email, string Password);
public record RefreshDto(string RefreshToken,string Email);
