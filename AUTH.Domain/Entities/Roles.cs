using System.ComponentModel.DataAnnotations.Schema;

namespace AUTH.Domain.Entities;

[Table("roles")]
public class Roles
{
    [Column("id")]
    public required int Id { get; set; }
    
    [Column("name")]
    public required string Name { get; set; }
}