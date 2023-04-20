using Microsoft.Extensions.Logging;
using OpenJob.Core;
using OpenJob.Model;
using OpenJob.Processors;

namespace OpenJob.Hosting;

public class WorkerClient : IWorkerClient
{
    private readonly IProcessorLoader _processorLoader;
    private readonly ILogger<WorkerClient> _logger;
    private CancellationTokenSource cts;

    public WorkerClient(IProcessorLoader processorLoader, ILogger<WorkerClient> logger)
    {
        _processorLoader = processorLoader;
        _logger = logger;
    }

    public async Task RunJob(ServerScheduleJobReq jobReq)
    {
        try
        {
            var processor = _processorLoader.Load(new ProcessorDefinition
            {
                ProcessorType = jobReq.ProcessorType,
                ProcessorInfo = jobReq.ProcessorInfo,
            });

            if (jobReq.TimeoutSecond > 0)
            {
                cts = new CancellationTokenSource(jobReq.TimeoutSecond * 1000);
            }
            else
            {
                cts = new CancellationTokenSource();
            }

            await processor.ExecuteAsync(new ProcessorContext
            {
                JobArgs = jobReq.JobArgs,
                JobId = jobReq.JobId,
                JobName = jobReq.JobName,
                MaxTryCount = jobReq.TaskRetryNum,
                TaskArgs = jobReq.TaskArgs,
                TaskId = jobReq.TaskId,
                TryCount = jobReq.TaskRetryNum
            }, cts.Token);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "RunJob cancel, timeout:{TimeOut}s.", jobReq.TimeoutSecond);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RunJob error.");
        }
    }

    public Task StopJob(Guid taskId)
    {
        _logger.LogInformation("Stop job.");
        cts?.Cancel();
        return Task.CompletedTask;
    }
}
