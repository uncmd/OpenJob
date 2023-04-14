using OpenJob.Enums;
using OpenJob.Jobs;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace OpenJob.Workers;

public class WorkerClusterManager : DomainService
{
    private readonly IRepository<WorkerInfo, Guid> _workerRepository;
    private readonly WorkerFilterService _workerFilterService;

    public WorkerClusterManager(
        IRepository<WorkerInfo, Guid> workerRepository,
        WorkerFilterService workerFilterService)
    {
        _workerRepository = workerRepository;
        _workerFilterService = workerFilterService;
    }

    /// <summary>
    /// 获取应用最合适的 worker 列表
    /// </summary>
    /// <param name="appId"></param>
    /// <returns></returns>
    public async Task<List<WorkerInfo>> GetSuitableWorkers(JobInfo job)
    {
        var workers = await _workerRepository.GetListAsync(p => p.AppId == job.AppId);

        // 过滤
        workers.RemoveAll(worker => _workerFilterService.FilterWorker(worker, job));

        if (job.DispatchStrategy == DispatchStrategy.HealthFirst)
        {
            workers.Sort((a, b) => b.CalculateScore() - a.CalculateScore());
        }
        else if (job.DispatchStrategy == DispatchStrategy.Random)
        {
            workers = workers.OrderBy(_ => Guid.NewGuid()).ToList();
        }

        // TODO: 限制集群大小

        return workers;
    }
}
