using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AUTH.Domain.Entities; // Adapte selon ton projet

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        // Le nom de la table dans DBeaver
        builder.ToTable("user_roles");

        // La clé primaire composée
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        // Mapping des colonnes
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.RoleId).HasColumnName("role_id");

        // Les relations pour éviter les "RoleId1"
        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        builder.HasOne(ur => ur.Role)
            .WithMany() 
            .HasForeignKey(ur => ur.RoleId);
    }
}