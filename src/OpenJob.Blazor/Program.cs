using OpenJob.Blazor;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
#if DEBUG
    .MinimumLevel.Debug()
#else
    .MinimumLevel.Information()
#endif
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .MinimumLevel.Override("OpenIddict", LogEventLevel.Information)
    .MinimumLevel.Override("Volo.Abp", LogEventLevel.Information)
    .MinimumLevel.Override("Orleans", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Async(c => c.File("Logs/logs.txt", rollingInterval: RollingInterval.Day))
    .WriteTo.Async(c => c.Console())
    .CreateLogger();

try
{
    Log.Information("Starting web host.");
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.AddAppSettingsSecretsJson()
        .UseAutofac()
        .UseSerilog()
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
        });

    await builder.AddApplicationAsync<OpenJobBlazorModule>();
    var app = builder.Build();
    await app.InitializeApplicationAsync();

    await app.RunAsync();
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
