using AUTH.Domain.Entities;

namespace AUTH.Core.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool> GetByAliasAsync(string alias);
    Task<bool> AddUserWithRoleAsync(Users users, int roleId);
    Task<Users?>GetByAliasWithRoleAsync(string alias);
    Task<RefreshToken?> GetRefreshTokenWithUserAsync(string token);
    Task UpdateRefreshTokenAsync(RefreshToken oldToken,  RefreshToken newToken);
    Task AddAsync(Users users);
    Task SaveChangesAsync();
}