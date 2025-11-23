using Microsoft.EntityFrameworkCore;
using UserService.Application;
using UserService.Infrastructure;
using UserService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var migrationScope = builder.Services.BuildServiceProvider().CreateScope();
try
{
    var readDbContext = migrationScope.ServiceProvider.GetRequiredService<UserService.Infrastructure.Persistence.Sql.UserReadDbContext>();
    var logger = migrationScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Initializing UserService read database...");
    
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

// app.UseHttpsRedirection(); // Disabled for Docker
app.UseAuthorization();
app.MapControllers();

app.Run();

