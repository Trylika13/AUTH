using AUTH.Domain.Entities;
using AUTH.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AUTH.Core.Interfaces.Repositories;

namespace AUTH.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<bool> GetByAliasAsync(string alias)
    {
        return await _context.Users.AnyAsync(u => u.Alias == alias);
    }

    public async Task<bool> AddUserWithRoleAsync(Users users, int roleId)
    {
        // 1. On ajoute d'abord l'utilisateur seul
        await _context.Users.AddAsync(users);
    
        // On fait un premier Save pour que la DB génère l'Id de l'utilisateur
        await _context.SaveChangesAsync(); 

        // 2. Maintenant qu'on a l'Id de l'user, on crée le lien
        var userRole = new UserRole
        {
            UserId = users.Id, // On utilise l'ID qui vient d'être généré
            RoleId = roleId
        };

        await _context.UserRoles.AddAsync(userRole);
    
        // 3. On sauvegarde le lien
        return await _context.SaveChangesAsync() > 0; // Retourne true si au moins une ligne a été modifiée
    }

    public async Task<Users?> GetByAliasWithRoleAsync(string alias)
    {
        if (string.IsNullOrWhiteSpace(alias)) return null;

        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            // On garde le ToLower() car c'est une sécurité indispensable avec Postgres
            .FirstOrDefaultAsync(u => u.Alias.ToLower() == alias.Trim().ToLower());
    }
    
    public async Task<RefreshToken?> GetRefreshTokenWithUserAsync(string token)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task UpdateRefreshTokenAsync(RefreshToken oldToken, RefreshToken newToken)
    {
        // 1. On ajoute toujours le nouveau token
        _context.RefreshTokens.Add(newToken);

        // 2. On ne modifie l'ancien QUE s'il n'est pas null
        if (oldToken != null)
        {
            _context.Entry(oldToken).State = EntityState.Modified;
        }

        // 3. On sauvegarde le tout
        await _context.SaveChangesAsync();
    }

    public Task AddAsync(Users users)
    {
        throw new NotImplementedException();
    }
    

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    
  
}