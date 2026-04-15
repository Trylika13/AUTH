using AUTH.Domain.Entities;
using AUTH.Presentation.Dtos;

namespace AUTH.Presentation.Mapper;

public static class UsersMapper
{
    public static UserLoginDto ToDto(this Users userEntity)
    {
        return new UserLoginDto
        {
            Alias = userEntity.Alias,
            Password = userEntity.PasswordHash
        };
    }

    public static Users ToEntity(this UserRegistrationDto dto)
    {
        return new Users
        {
            Email = dto.Email,
            PasswordHash = dto.Password,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IdTiers = dto.IdTiers,
            Alias = dto.Alias
        };
    }
}