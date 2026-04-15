using AUTH.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AUTH.Infrastructure.Data.Configurations;

public class UsersConfiguration : IEntityTypeConfiguration<Users>
{
    public void Configure(EntityTypeBuilder<Users> builder)
    {
        builder.ToTable("users");
        
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id");
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("email");
        
        builder.Property( u=> u.PasswordHash)
            .IsRequired()
            .HasColumnName("password_hash");
        
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("FirstName");
        
        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("LastName");
        
        builder.Property(u => u.IdTiers)
            .IsRequired()
            .HasColumnName("IdTiers");
        
        builder.Property(u => u.Alias)
            .IsRequired()
            .HasMaxLength(15)
            .HasColumnName("alias");
            
        
    }
}