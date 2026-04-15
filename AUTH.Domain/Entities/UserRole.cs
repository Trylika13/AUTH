namespace AUTH.Domain.Entities;

public class UserRole
{
    public Guid UserId { get; set; }
    public Users? User { get; set; } // Relation vers Users

    public int RoleId { get; set; }
    public Roles? Role { get; set; } // Relation vers Role
}