namespace AUTH.Presentation.Dtos;

public class UserLoginDto
{
    
    public required string Alias { get; set; }
    
    public required string Password { get; set; }
}