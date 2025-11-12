using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenSleigh.DependencyInjection;
using OpenSleigh.Persistence.SQL;
using OpenSleigh.Persistence.SQLServer;
using OpenSleigh.Transport.RabbitMQ;
using OrderService.Orchestrator.Sagas;

var hostBuilder = CreateHostBuilder(args);
var host = hostBuilder.Build();

await host.RunAsync();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddLogging(cfg =>
            {
                cfg.AddConsole();
            });

            services.AddOpenSleigh(cfg =>
            {
                var sqlConnStr = hostContext.Configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(sqlConnStr))
                {
                    throw new InvalidOperationException("DefaultConnection connection string is not configured in appsettings.json");
                }
                var sqlConfig = new SqlConfiguration(sqlConnStr);

                var rabbitSection = hostContext.Configuration.GetSection("RabbitMQ");
                var rabbitCfg = new RabbitConfiguration(
                    hostName: rabbitSection["HostName"]!,
                    vhost: rabbitSection["VHost"] ?? "/",
                    userName: rabbitSection["UserName"]!,
                    password: rabbitSection["Password"]!);

                cfg.UseRabbitMQTransport(rabbitCfg)
                   .UseSqlServerPersistence(sqlConfig)
                   .AddSaga<OrderSaga, OrderSagaState>();
            });
        });

