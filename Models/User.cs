using System.ComponentModel.DataAnnotations.Schema;

namespace dofdir_komek.Models;
[Table("users")]
public sealed record User
{
    [Column("id")]
    public int Id { get; set; } = default!;
    [Column("name")]
    public string Name { get; set; } = default!;
    [Column("email")]
    public string Email { get; set; } = default!;
    [Column("password")]
    public string Password { get; set; } = default!;
    [Column("role")]
    public string Role { get; set; } = default!;
}
