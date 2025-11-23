using Microsoft.EntityFrameworkCore;

namespace UserService.Infrastructure.Persistence.Sql;

public class UserReadDbContext : DbContext
{
    public UserReadDbContext(DbContextOptions<UserReadDbContext> options) : base(options)
    {
    }

    public DbSet<UserReadModel> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserReadModel>(entity =>
        {
            entity.ToTable("Users", "dbo");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}

