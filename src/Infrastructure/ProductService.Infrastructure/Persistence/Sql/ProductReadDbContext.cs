using Microsoft.EntityFrameworkCore;

namespace ProductService.Infrastructure.Persistence.Sql;

public class ProductReadDbContext : DbContext
{
    public ProductReadDbContext(DbContextOptions<ProductReadDbContext> options) : base(options)
    {
    }

    public DbSet<ProductReadModel> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductReadModel>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name);
            entity.Property(e => e.Name).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000);
        });

        base.OnModelCreating(modelBuilder);
    }
}

