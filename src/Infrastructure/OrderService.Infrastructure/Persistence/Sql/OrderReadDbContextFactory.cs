using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrderService.Infrastructure.Persistence.Sql;

public class OrderReadDbContextFactory : IDesignTimeDbContextFactory<OrderReadDbContext>
{
    public OrderReadDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrderReadDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=OrderServiceReadDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;");

        return new OrderReadDbContext(optionsBuilder.Options);
    }
}

