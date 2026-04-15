using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AUTH.Domain.Entities;
[Table("users")]
public class Users
{
    [Key]
    [Column("id")]
    public Guid  Id { get; set; }
    
    [Column("email")]
    public required string Email { get; set; }
    
    [Column("password_hash")]
    public required string PasswordHash { get; set; }
    
    public required Guid IdTiers { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    [Column("alias")] 
    public required string Alias {get; set;} = string.Empty;
    
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    
}   