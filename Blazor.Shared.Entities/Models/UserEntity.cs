using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.Shared.Entities.Models;

[Table("Users")]
public sealed class UserEntity
{
    [Column("pk_user"), Key] 
    public int Id { get; set; }
}