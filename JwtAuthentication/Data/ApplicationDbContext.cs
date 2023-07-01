using JwtAuthentication.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthentication.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
}