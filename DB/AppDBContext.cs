using dofdir_komek.Models;
using Microsoft.EntityFrameworkCore;

namespace dofdir_komek.DB;

public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; } = null!;
}