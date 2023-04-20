using Microsoft.Extensions.Hosting;
using OpenJob;
using Serilog.Events;
using Serilog;
using Microsoft.Extensions.DependencyInjection;

const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Scope} {Message:lj}{NewLine}{Exception}";
const string ConsoleOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Scope} {Message:lj}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
#if DEBUG
    .MinimumLevel.Debug()
#else
    .MinimumLevel.Information()
#endif
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Async(c => c.File("Logs/logs.txt", rollingInterval: RollingInterval.Day, outputTemplate: DefaultOutputTemplate))
    .WriteTo.Async(c => c.Console(outputTemplate: ConsoleOutputTemplate))
    .CreateLogger();

try
{
    Log.Information("Starting openjob worker host.");

    var builder = Host.CreateDefaultBuilder(args);
    
    builder
        .ConfigureServices(services =>
        {
            services.Configure<OpenJobWorkerOptions>(options =>
            {
                options.AppName = "Default";
                options.ServerAddress = "https://localhost:44327";
            });
            services.AddOpenJobWorker();
        })
        .UseSerilog()
        .UseConsoleLifetime();

    var host = builder.Build();

    await host.RunAsync();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly!");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
