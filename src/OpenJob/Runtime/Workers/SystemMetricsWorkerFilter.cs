using OpenJob.Entities;

namespace OpenJob.Runtime.Workers;

/// <summary>
/// 根据系统指标过滤 Worker
/// </summary>
public class SystemMetricsWorkerFilter : IWorkerFilter
{
    private readonly ILogger<SystemMetricsWorkerFilter> _logger;

    public SystemMetricsWorkerFilter(ILogger<SystemMetricsWorkerFilter> logger)
    {
        _logger = logger;
    }

    public bool Filter(WorkerInfo worker, JobInfo job)
    {
        var filter = !worker.Available(job.MinCpuCores, job.MinMemory, job.MinDisk);

        if (filter)
        {
            _logger.LogInformation("[Job-{Job}] filter worker[{Worker}] because the {WorkerInfo} do not meet the requirements",
                job, worker, worker.PrintInfo());
        }

        return filter;
    }
}
