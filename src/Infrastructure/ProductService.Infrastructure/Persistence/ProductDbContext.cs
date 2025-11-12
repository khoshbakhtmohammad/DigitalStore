using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;

namespace ProductService.Infrastructure.Persistence;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Status).HasConversion<string>();
            entity.OwnsOne(e => e.Price, price =>
            {
                price.Property(p => p.Value).HasColumnName("Price").HasPrecision(18, 2);
                price.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3);
            });
        });

        base.OnModelCreating(modelBuilder);
    }
}

