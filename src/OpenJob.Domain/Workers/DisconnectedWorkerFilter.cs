using Microsoft.Extensions.Logging;
using OpenJob.Jobs;

namespace OpenJob.Workers;

/// <summary>
/// 过滤连接超时的 Worker
/// </summary>
public class DisconnectedWorkerFilter : IWorkerFilter
{
    private readonly ILogger<DisconnectedWorkerFilter> _logger;

    public DisconnectedWorkerFilter(ILogger<DisconnectedWorkerFilter> logger)
    {
        _logger = logger;
    }

    public bool Filter(WorkerInfo worker, JobInfo job)
    {
        var timeout = worker.Timeout();

        if (timeout)
        {
            _logger.LogInformation("[Job-{Job}] filter worker[{Worker}] due to timeout(lastActiveTime={Time})",
                job, worker, worker.LastActiveTime);
        }

        return timeout;
    }
}
