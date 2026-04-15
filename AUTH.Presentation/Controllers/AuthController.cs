using AUTH.Core.Interfaces.Services;
using AUTH.Core.Services;
using AUTH.Presentation.Dtos;
using AUTH.Presentation.Mapper;
using Microsoft.AspNetCore.Mvc;

namespace AUTH.Presentation.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    #region Register

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegistrationDto dto)
    {
        var userEntity = dto.ToEntity();
        
        var success = await _authService.RegisterAsync(userEntity, dto.RoleId);
    
        if (!success)
            return BadRequest("L'utilisateur n'a pas pu être créé (email déjà utilisé ?)");

        return Ok("Utilisateur créé avec succès");
    }
    

    #endregion

    #region Login

    [HttpPost("login")]
public async Task<IActionResult> Login(UserLoginDto dto)
{
    var result = await _authService.LoginAsync(dto.Alias, dto.Password);

    if (result == null)
    {
        return Unauthorized("Alias ou mot de passe incorrect");
    }

    return Ok(new
    {
        message = "Connexion réussie",
        token = result.Value.Token,
        refreshToken = result.Value.RefreshToken
    });
}

    #endregion

    #region Refresh

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(TokenRequestDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken);

        if (result == null)
            return Unauthorized("Token invalide ou expiré");

        return Ok(new
        {
            token = result.Value.Token,
            refreshToken = result.Value.RefreshToken
        });
    }

    #endregion
    
}