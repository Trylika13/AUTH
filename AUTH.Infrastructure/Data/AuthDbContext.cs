using AUTH.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AUTH.Infrastructure.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) {}
    
    public DbSet<Users> Users => Set<Users>();
    
    public DbSet<Roles> Roles => Set<Roles>();
    
    public DbSet<UserRole> UserRoles { get; set; }
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    
        // Cette ligne cherche les fichiers "Configuration" s'ils existent
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
        
    }
}