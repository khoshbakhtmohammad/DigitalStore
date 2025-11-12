using Microsoft.EntityFrameworkCore;
using OrderService.Application;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Persistence;
using OpenSleigh.DependencyInjection;
using OpenSleigh.Persistence.SQL;
using OpenSleigh.Persistence.SQLServer;
using OpenSleigh.Transport.RabbitMQ;
using OpenSleigh.Transport;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOpenSleigh(cfg =>
{
    var sqlConnStr = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(sqlConnStr))
    {
        throw new InvalidOperationException("DefaultConnection connection string is not configured in appsettings.json");
    }
    var sqlConfig = new SqlConfiguration(sqlConnStr);

    var rabbitSection = builder.Configuration.GetSection("RabbitMQ");
    var rabbitCfg = new RabbitConfiguration(
        hostName: rabbitSection["HostName"]!,
        vhost: rabbitSection["VHost"] ?? "/",
        userName: rabbitSection["UserName"]!,
        password: rabbitSection["Password"]!);

    cfg.SetPublishOnly()
       .UseRabbitMQTransport(rabbitCfg)
       .UseSqlServerPersistence(sqlConfig);
});
var migrationScope = builder.Services.BuildServiceProvider().CreateScope();
try
{
    var readDbContext = migrationScope.ServiceProvider.GetRequiredService<OrderService.Infrastructure.Persistence.Sql.OrderReadDbContext>();
    var logger = migrationScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Initializing OrderService read database...");
    
    try
    {
        readDbContext.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully.");
    }
    catch
    {
        logger.LogInformation("No migrations found or migration failed. Ensuring database and tables are created...");
        readDbContext.Database.EnsureCreated();
        logger.LogInformation("Database and tables created successfully.");
    }
}
catch (Exception ex)
{
    var logger = migrationScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while initializing the read database.");
    throw;
}
finally
{
    migrationScope.Dispose();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

