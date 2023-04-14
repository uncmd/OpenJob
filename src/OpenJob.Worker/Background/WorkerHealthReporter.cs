using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenJob.Hosting;
using OpenJob.Model;

namespace OpenJob.Background;

public class WorkerHealthReporter : IDisposable
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly PeriodicTimer timer;
    private readonly ILogger<WorkerHealthReporter> logger;
    private readonly SingalRClient _singalRClient;

    public WorkerHealthReporter(
        IOptions<OpenJobWorkerOptions> options,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<WorkerHealthReporter> logger,
        SingalRClient singalRClient)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        this.logger = logger;
        _singalRClient = singalRClient;

        timer = new PeriodicTimer(TimeSpan.FromSeconds(options.Value.HealthReportInterval));
    }

    public async Task Start(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Heartbeat started.");

        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                try
                {
                    await DoWorkAsync(scope.ServiceProvider, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Health reporter error!");
                }
            }
        }
    }

    protected async Task DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        var metricsCollector = serviceProvider.GetRequiredService<ISystemMetricsCollector>();

        var metrics = metricsCollector.Collect();
        var workerHeartbeat = new WorkerHeartbeatDto
        {
            AppName = WorkerRuntime.AppName,
            AppId = WorkerRuntime.AppId,
            SystemMetrics = metrics,
            WorkerAddress = WorkerRuntime.Address,
            HeartbeatTime = DateTime.Now,
            WorkerVersion = WorkerRuntime.Version,
            Tag = WorkerRuntime.Tag,
            Client = WorkerRuntime.Client
        };

        await _singalRClient.WorkerHeartbeat(workerHeartbeat, cancellationToken);
    }

    public void Dispose() => timer?.Dispose();
}
