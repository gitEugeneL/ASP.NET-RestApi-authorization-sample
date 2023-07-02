using JwtAuthentication.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JwtAuthentication.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(ConfigureUserTable);
    }

    private void ConfigureUserTable(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(record => record.Username)
            .IsUnique();
    }
}
