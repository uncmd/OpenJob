using OpenJob.Entities;
using OpenJob.Runtime.Workers;
using Volo.Abp.DependencyInjection;

namespace OpenJob.Domain;

public class WorkerFilterService : ISingletonDependency
{
    private readonly List<IWorkerFilter> _workerFilters;

    public WorkerFilterService(
        IEnumerable<IWorkerFilter> workerFilters,
        ILogger<TimingStrategyService> logger)
    {
        _workerFilters = workerFilters.ToList();

        foreach (var workerFilter in _workerFilters)
        {
            logger.LogInformation("register worker filter: {WorkerFilter}", workerFilter);
        }
    }

    public bool FilterWorker(WorkerInfo worker, JobInfo job)
    {
        foreach (var workerFilter in _workerFilters)
        {
            if (workerFilter.Filter(worker, job))
            {
                return true;
            }
        }
        return false;
    }
}
