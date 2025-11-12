using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProductService.Infrastructure.Persistence.Sql;

public class ProductReadDbContextFactory : IDesignTimeDbContextFactory<ProductReadDbContext>
{
    public ProductReadDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProductReadDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=ProductServiceReadDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;");

        return new ProductReadDbContext(optionsBuilder.Options);
    }
}

