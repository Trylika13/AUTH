using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AUTH.Core.Interfaces.Repositories;
using AUTH.Core.Interfaces.Services;
using AUTH.Domain.Entities;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
namespace AUTH.Core.Services;


public class AuthService : IAuthService
{
    
    private readonly IConfiguration _config;
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _config = config;
    }

    
    
    #region JWT

    private string GenerateJSONWebToken(Users user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("name", $"{user.FirstName} {user.LastName}"),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("guid_representant", user.IdTiers.ToString()),
        };

        foreach (var userRole in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole.Role!.Name));
        }
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials

        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    #endregion
    
    #region RefreshToken

    private RefreshToken GenerateRefreshToken(Guid userId)
    {
        var randomNumber = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            UserId = userId
        };
    }

    #endregion

    public async Task<bool> RegisterAsync(Users user, int roleId)
    {
        // Vérifier si l'user existe déjà
        if (await _userRepository.GetByAliasAsync(user.Alias))
            return false;

        // Hasher le mot de passe
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        
        return await _userRepository.AddUserWithRoleAsync(user, roleId);
    }   

    public async Task<(string Token, string RefreshToken)?> LoginAsync(string alias, string password)
    {   
        Console.WriteLine($"--- DEBUG LOGIN ---");
        Console.WriteLine($"Email cherché : '{alias}'");
        Console.WriteLine($"Password saisi : '{password}'");
        
        var user = await _userRepository.GetByAliasWithRoleAsync(alias);
        if (user == null) return null;
        
        // Vérification Password
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!isPasswordValid) return null;

        // Génération (Appelle des méthodes privées dans le service)
        var token = GenerateJSONWebToken(user); 
        var refreshTokenEntity = GenerateRefreshToken(user.Id);
    
        // Persistence
        await _userRepository.UpdateRefreshTokenAsync(null!, refreshTokenEntity);
    
        return (token, refreshTokenEntity.Token);    }
    
    public async Task<(string Token, string RefreshToken)?> RefreshTokenAsync(string refreshToken)
    {
        var savedRefreshToken = await _userRepository.GetRefreshTokenWithUserAsync(refreshToken);

        // Vérifications de sécurité
        if (savedRefreshToken == null || 
            savedRefreshToken.Expires < DateTime.UtcNow || 
            savedRefreshToken.Revoked != null)
        {
            return null;
        }

        // Génération des nouveaux tokens
        var newAccessToken = GenerateJSONWebToken(savedRefreshToken.User);
        var newRefreshTokenEntity = GenerateRefreshToken(savedRefreshToken.UserId);

        // Révocation de l'ancien
        savedRefreshToken.Revoked = DateTime.UtcNow;
        await  _userRepository.UpdateRefreshTokenAsync(savedRefreshToken, newRefreshTokenEntity);

        return (newAccessToken, newRefreshTokenEntity.Token);    }
}