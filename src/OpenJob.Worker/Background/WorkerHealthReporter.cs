using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenJob.Model;

namespace OpenJob.Background;

public class WorkerHealthReporter : IDisposable
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly PeriodicTimer timer;
    private readonly ILogger<WorkerHealthReporter> logger;
    private readonly OpenJobWorkerOptions workerOptions;

    public WorkerHealthReporter(
        IOptions<OpenJobWorkerOptions> options,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<WorkerHealthReporter> logger)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        this.logger = logger;
        workerOptions = options.Value;

        timer = new PeriodicTimer(TimeSpan.FromMicroseconds(options.Value.HealthReportInterval));
    }

    public async Task Start(CancellationToken cancellationToken = default)
    {
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
                    logger.LogError(ex, "");
                }
            }
        }
    }

    public Task Stop(CancellationToken cancellationToken = default)
    {
        timer.Dispose();
        return Task.CompletedTask;
    }

    protected async Task DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        var metricsCollector = serviceProvider.GetRequiredService<ISystemMetricsCollector>();
        var serverClient = serviceProvider.GetRequiredService<ServerClientFactory>();

        var metrics = metricsCollector.Collect();
        var workerHeartbeat = new WorkerHeartbeatDto
        {
            AppId = WorkerRuntime.AppId,
            AppName = workerOptions.AppName,
            SystemMetrics = metrics,
            WorkerAddress = WorkerRuntime.WorkerAddress,
            HeartbeatTime = DateTime.Now,
            WorkerVersion = GetType().Assembly.GetName().Version.ToString(),
            Tag = workerOptions.Tag,
        };

        logger.LogInformation("report health status: {WorkerHeartbeat}", workerHeartbeat);

        await serverClient.WorkerHeartbeat(workerHeartbeat);
    }

    public void Dispose()
    {
        timer?.Dispose();
    }
}
