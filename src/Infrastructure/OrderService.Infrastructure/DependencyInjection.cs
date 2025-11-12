using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OrderService.Application.Interfaces;
using OrderService.Domain.Repositories;
using OrderService.Infrastructure.Persistence.Mongo;
using OrderService.Infrastructure.Projections;
using OrderReadDbContext = OrderService.Infrastructure.Persistence.Sql.OrderReadDbContext;
using OrderReadRepository = OrderService.Infrastructure.Persistence.Sql.OrderReadRepository;
using OrderService.Infrastructure.Services;
using StackExchange.Redis;

namespace OrderService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var mongoConnectionString = configuration.GetConnectionString("MongoDB");
        var mongoClient = new MongoClient(mongoConnectionString);
        var mongoDatabase = mongoClient.GetDatabase("OrderServiceDb");
        
        OrderMongoClassMaps.RegisterClassMaps();
        
        services.AddSingleton<IMongoDatabase>(mongoDatabase);
        services.AddSingleton<OrderMongoContext>();
        services.AddScoped<IOrderRepository, OrderMongoRepository>();

        var sqlConnectionString = configuration.GetConnectionString("SqlServer");
        services.AddDbContext<OrderReadDbContext>(options =>
            options.UseSqlServer(sqlConnectionString));
        services.AddScoped<Application.Interfaces.IOrderReadRepository, Persistence.Sql.OrderReadRepository>();
        services.AddScoped<IOrderProjectionService, OrderProjectionService>();

        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddScoped<IIdempotencyService, IdempotencyService>();

        return services;
    }
}
