using IF133.Tugas1.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace IF133.Tugas1.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.Property(p => p.Name).IsRequired().HasMaxLength(200);
            e.Property(p => p.Description).HasMaxLength(1000);
            e.Property(p => p.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Keyboard Mechanical", Description = "Keyboard 75% hot-swap", Price = 350000, Stock = 5 },
            new Product { Id = 2, Name = "Mouse Gaming", Description = "Mouse wireless 2.4GHz", Price = 250000, Stock = 10 },
            new Product { Id = 3, Name = "Monitor 24 Inch", Description = "Monitor IPS 75Hz", Price = 1500000, Stock = 3 }
        );
    }
}
