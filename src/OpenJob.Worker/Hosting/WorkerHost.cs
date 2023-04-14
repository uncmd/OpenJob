using Microsoft.Extensions.Hosting;
using OpenJob.Background;

namespace OpenJob.Hosting;

public class WorkerHost : IHostedService
{
    private readonly WorkerHealthReporter _healthReporter;
    private readonly SingalRClient _singalRClient;

    public WorkerHost(
        WorkerHealthReporter healthReporter,
        SingalRClient singalRClient)
    {
        _healthReporter = healthReporter;
        _singalRClient = singalRClient;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _singalRClient.Connect(cancellationToken);

        while (await _singalRClient.AssertApp(cancellationToken) == false)
        {
            await Task.Delay(10000, cancellationToken);
        }

        await _healthReporter.Start(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
