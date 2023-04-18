using Microsoft.Extensions.Hosting;
using OpenJob;
using Serilog.Events;
using Serilog;
using Microsoft.Extensions.DependencyInjection;

Log.Logger = new LoggerConfiguration()
#if DEBUG
    .MinimumLevel.Debug()
#else
    .MinimumLevel.Information()
#endif
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Async(c => c.File("Logs/logs.txt", rollingInterval: RollingInterval.Day))
    .WriteTo.Async(c => c.Console())
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
