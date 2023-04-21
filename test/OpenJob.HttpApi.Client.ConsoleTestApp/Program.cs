using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenJob.HttpApi.Client.ConsoleTestApp;

Host.CreateDefaultBuilder(args)
    .AddAppSettingsSecretsJson()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<ConsoleTestAppHostedService>();
    }).RunConsoleAsync();
