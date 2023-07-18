using JwtAuthentication.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthentication.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(eb =>
        {
            eb.HasIndex(u => u.Username).IsUnique();
            eb.Property(u => u.Username).HasMaxLength(150).IsRequired();
            eb.Property(u => u.PwdHash).IsRequired();
            eb.Property(u => u.PwdSalt).IsRequired();
            eb.Property(u => u.RoleId).IsRequired();
            
            eb.HasOne<Role>(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);
        });
    }
}
