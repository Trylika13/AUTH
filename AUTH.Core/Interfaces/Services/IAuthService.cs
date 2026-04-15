using AUTH.Domain.Entities;

namespace AUTH.Core.Interfaces.Services;

public interface IAuthService
{
    // Inscription d'un nouvel utilisateur
    Task<bool> RegisterAsync(Users user, int roleId);

    // Connexion initiale (Retourne le duo de tokens)
    // On utilise un tuple (string, string) pour Access et Refresh
    Task<(string Token, string RefreshToken)?> LoginAsync(string email, string password);

    // Renouvellement du token
    Task<(string Token, string RefreshToken)?> RefreshTokenAsync(string refreshToken);
}