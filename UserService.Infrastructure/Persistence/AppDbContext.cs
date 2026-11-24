namespace UserService.Infrastructure.Persistence;

using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Id)
                .HasConversion(
                    id => id.Value,
                    value => new UserId(value));

            entity.HasIndex(u => u.Login).IsUnique();
        });
    }
}