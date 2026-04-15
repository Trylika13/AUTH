namespace AUTH.Presentation.Dtos;

public class UserRegistrationDto
{
    public required string Email { get; set; }
    
    public required string Password { get; set; }
    
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public required Guid IdTiers {get; set;}
    
    public int RoleId { get; set; }
    
    public required string Alias { get; set; }
}