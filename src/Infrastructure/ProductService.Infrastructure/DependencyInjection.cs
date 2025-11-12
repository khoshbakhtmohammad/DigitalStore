using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ProductService.Application.Interfaces;
using ProductService.Domain.Repositories;
using ProductService.Infrastructure.Persistence.Mongo;
using ProductService.Infrastructure.Projections;
using ProductReadDbContext = ProductService.Infrastructure.Persistence.Sql.ProductReadDbContext;
using ProductReadRepository = ProductService.Infrastructure.Persistence.Sql.ProductReadRepository;

namespace ProductService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var mongoConnectionString = configuration.GetConnectionString("MongoDB");
        var mongoClient = new MongoClient(mongoConnectionString);
        var mongoDatabase = mongoClient.GetDatabase("ProductServiceDb");
        services.AddSingleton<IMongoDatabase>(mongoDatabase);
        services.AddSingleton<ProductMongoContext>();
        services.AddScoped<IProductRepository, ProductMongoRepository>();

        var sqlConnectionString = configuration.GetConnectionString("SqlServer");
        services.AddDbContext<ProductReadDbContext>(options =>
            options.UseSqlServer(sqlConnectionString));
        services.AddScoped<Application.Interfaces.IProductReadRepository, ProductReadRepository>();
        services.AddScoped<IProductProjectionService, ProductProjectionService>();

        return services;
    }
}
