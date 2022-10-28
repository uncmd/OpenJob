using PowerScheduler.Entities;
using PowerScheduler.Runtime.Workers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace PowerScheduler.Domain;

public class WorkerClusterManager : DomainService
{
    private readonly IRepository<SchedulerWorker, Guid> _workerRepository;
    private readonly List<IWorkerFilter> _workerFilters;

    public WorkerClusterManager(
        IRepository<SchedulerWorker, Guid> workerRepository,
        IEnumerable<IWorkerFilter> workerFilters)
    {
        _workerRepository = workerRepository;
        _workerFilters = workerFilters.ToList();
    }

    /// <summary>
    /// 获取应用最合适的 worker 列表
    /// </summary>
    /// <param name="appId"></param>
    /// <returns></returns>
    public async Task<List<SchedulerWorker>> GetSuitableWorkers(SchedulerJob job)
    {
        var workers = await _workerRepository.GetListAsync(p => p.AppId == job.AppId);

        // 过滤
        workers.RemoveAll(worker => FilterWorker(worker, job));

        // 排序
        workers.Sort((a, b) => b.CalculateScore() - a.CalculateScore());

        // TODO: 限制集群大小

        return workers;
    }

    private bool FilterWorker(SchedulerWorker worker, SchedulerJob job)
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
