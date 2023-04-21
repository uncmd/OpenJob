using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenJob.Background;
using OpenJob.Model;
using OpenJob.Processors;

namespace OpenJob.Hosting;

public class WorkerHost : IHostedService
{
    private readonly WorkerHealthReporter _healthReporter;
    private readonly ServerClientProxy _serverClient;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WorkerHost(
        WorkerHealthReporter healthReporter,
        ServerClientProxy serverClient,
        IServiceScopeFactory serviceScopeFactory)
    {
        _healthReporter = healthReporter;
        _serverClient = serverClient;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _serverClient.Connect(cancellationToken);

        while ((await _serverClient.AssertApp(WorkerRuntime.AppName)).Success == false)
        {
            await Task.Delay(10000, cancellationToken);
        }

        await RegisterProcessor();

        await _healthReporter.Start(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 向Server注册Processor
    /// </summary>
    /// <returns></returns>
    private async Task RegisterProcessor()
    {
        using var scop = _serviceScopeFactory.CreateScope();
        var processors = scop.ServiceProvider.GetRequiredService<IEnumerable<IProcessor>>();

        var req = new ProcessorReq
        {
            AppName = WorkerRuntime.AppName,
            AppId = WorkerRuntime.AppId,
            ModuleName = WorkerRuntime.Client,
            Version = WorkerRuntime.Version,
            Dtls = new List<ProcessorReqDtl>()
        };

        foreach (var processor in processors)
        {
            var processorType = processor.GetType();
            req.Dtls.Add(new ProcessorReqDtl
            {
                IsEnabled = ProcessorAttribute.GetProcessorIsEnabled(processorType),
                Name = ProcessorAttribute.GetProcessorName(processorType),
                Description = ProcessorAttribute.GetProcessorDescription(processorType),
                ProcessorInfo = processorType.FullName
            });
        }

        await _serverClient.RegisterProcessor(req);
    }
}
