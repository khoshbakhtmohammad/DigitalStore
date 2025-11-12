using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UserService.Infrastructure.Persistence.Sql;

public class UserReadDbContextFactory : IDesignTimeDbContextFactory<UserReadDbContext>
{
    public UserReadDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UserReadDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=UserServiceReadDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;");

        return new UserReadDbContext(optionsBuilder.Options);
    }
}

