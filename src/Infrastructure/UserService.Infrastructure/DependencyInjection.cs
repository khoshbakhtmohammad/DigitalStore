using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using UserService.Application.Interfaces;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Persistence.Mongo;
using UserService.Infrastructure.Projections;
using UserReadDbContext = UserService.Infrastructure.Persistence.Sql.UserReadDbContext;
using UserReadRepository = UserService.Infrastructure.Persistence.Sql.UserReadRepository;

namespace UserService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var mongoConnectionString = configuration.GetConnectionString("MongoDB");
        var mongoClient = new MongoClient(mongoConnectionString);
        var mongoDatabase = mongoClient.GetDatabase("UserServiceDb");
        services.AddSingleton<IMongoDatabase>(mongoDatabase);
        services.AddSingleton<UserMongoContext>();
        services.AddScoped<IUserRepository, UserMongoRepository>();

        var sqlConnectionString = configuration.GetConnectionString("SqlServer");
        services.AddDbContext<UserReadDbContext>(options =>
            options.UseSqlServer(sqlConnectionString));
        services.AddScoped<Application.Interfaces.IUserReadRepository, UserReadRepository>();
        services.AddScoped<IUserProjectionService, UserProjectionService>();

        return services;
    }
}
