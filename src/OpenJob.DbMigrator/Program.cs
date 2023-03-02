using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenJob.DbMigrator;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Volo.Abp", LogEventLevel.Warning)
#if DEBUG
    .MinimumLevel.Override("OpenJob", LogEventLevel.Debug)
#else
    .MinimumLevel.Override("OpenJob", LogEventLevel.Information)
#endif
    .Enrich.FromLogContext()
    .WriteTo.Async(c => c.File("Logs/logs.txt"))
    .WriteTo.Async(c => c.Console())
    .CreateLogger();

await Host.CreateDefaultBuilder(args)
    .AddAppSettingsSecretsJson()
    .ConfigureLogging((context, logging) => logging.ClearProviders())
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<DbMigratorHostedService>();
    }).RunConsoleAsync();
