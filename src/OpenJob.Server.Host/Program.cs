using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenJob;
using Serilog;
using Serilog.Events;
using Volo.Abp;

Log.Logger = new LoggerConfiguration()
#if DEBUG
    .MinimumLevel.Debug()
#else
    .MinimumLevel.Information()
#endif
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Orleans", LogEventLevel.Information)
    .MinimumLevel.Override("Volo.Abp", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Async(c => c.File("Logs/logs.txt", rollingInterval: RollingInterval.Day))
    .WriteTo.Async(c => c.Console())
    .CreateLogger();

try
{
    Log.Information("Starting openjob server host.");

    var builder = Host.CreateDefaultBuilder(args);

    builder.ConfigureServices(services =>
    {
        services.AddApplicationAsync<OpenJobServerHostModule>(options =>
        {
            options.Services.ReplaceConfiguration(services.GetConfiguration());
        });
    })
    .UseOrleans((context, builder) =>
    {
        // Orleans.Clustering.AdoNet.Storage.AdoNetInvariants
        string invariant = "Microsoft.Data.SqlClient";
        var configuration = builder.Services.GetConfiguration();
        string connectionString = configuration.GetConnectionString("Default");

        builder.UseAdoNetClustering(options =>
        {
            options.Invariant = invariant;
            options.ConnectionString = connectionString;
        })
        .UseAdoNetReminderService(options =>
        {
            options.Invariant = invariant;
            options.ConnectionString = connectionString;
        })
        .UseOpenJob();
    })
    .UseAutofac()
    .UseSerilog()
    .UseConsoleLifetime();

    var host = builder.Build();
    await host.Services.GetRequiredService<IAbpApplicationWithExternalServiceProvider>()
        .InitializeAsync(host.Services);

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