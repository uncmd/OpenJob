using Microsoft.Extensions.Logging;
using OpenJob.Core;
using OpenJob.Model;
using OpenJob.Processors;

namespace OpenJob.Hosting;

public class WorkerClient : IWorkerClient
{
    private readonly IProcessorLoader _processorLoader;
    private readonly ILogger<WorkerClient> _logger;
    private readonly CancellationTokenSource manualCts = new CancellationTokenSource();

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

            var cts = manualCts;

            // 注册超时取消
            if (jobReq.TimeoutSecond > 0)
            {
                var timeoutCts = new CancellationTokenSource(jobReq.TimeoutSecond * 1000);
                timeoutCts.Token.Register(async () => await TimeoutHandler(jobReq));
                cts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, manualCts.Token);
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "RunJob error.");
        }
    }

    public Task StopJob(Guid taskId)
    {
        _logger.LogInformation("Stop job.");
        manualCts.Cancel();
        return Task.CompletedTask;
    }

    private Task TimeoutHandler(ServerScheduleJobReq jobReq)
    {
        _logger.LogWarning("RunJob timeout cancel: {TimeOut}s.", jobReq.TimeoutSecond);
        return Task.CompletedTask;
    }
}
