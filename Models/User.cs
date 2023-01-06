using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace dofdir_komek.Models;
[Table("users")]
[Index(nameof(Email), IsUnique = true)]
public sealed record User
{
    [Column("id")]
    public int Id { get; init; } = default!;
    [Column("name")]
    public string Name { get; init; } = default!;
    [Column("email")]
    public string Email { get; init; } = default!;
    [Column("password")]
    public string Password { get; init; } = default!;
    [Column("role")]
    public string Role { get; init; } = default!;
}
