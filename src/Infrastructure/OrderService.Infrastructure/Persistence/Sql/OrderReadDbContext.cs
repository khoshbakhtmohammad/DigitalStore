using Microsoft.EntityFrameworkCore;

namespace OrderService.Infrastructure.Persistence.Sql;

public class OrderReadDbContext : DbContext
{
    public OrderReadDbContext(DbContextOptions<OrderReadDbContext> options) : base(options)
    {
    }

    public DbSet<OrderReadModel> Orders { get; set; }
    public DbSet<OrderItemReadModel> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderReadModel>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CustomerId);
            entity.HasMany(e => e.Items)
                  .WithOne()
                  .HasForeignKey(i => i.OrderId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItemReadModel>(entity =>
        {
            entity.ToTable("OrderItems");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.ProductId);
        });

        base.OnModelCreating(modelBuilder);
    }
}

