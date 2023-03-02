using Microsoft.Extensions.Hosting;
using OpenJob;

var host = new HostBuilder()
    .ConfigureServices(services =>
    {
        services.AddOpenJobWorker();
    })
    .UseConsoleLifetime()
    .Build();

await host.RunAsync();
