using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OpenJob.Core;
using OpenJob.Hubs;
using OpenJob.Model;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace OpenJob.Workers;

[ExposeServices(typeof(IWorkerClient))]
public class WorkerClientProxy : IWorkerClient, ITransientDependency
{
    private readonly IHubContext<MessagingHub, IWorkerClient> _messagingHub;
    private readonly ILogger<WorkerClientProxy> _logger;
    private readonly IDistributedCache<TaskReqCacheItem, Guid> _cache;

    public WorkerClientProxy(
        IHubContext<MessagingHub, IWorkerClient> messagingHub,
        ILogger<WorkerClientProxy> logger,
        IDistributedCache<TaskReqCacheItem, Guid> cache)
    {
        _messagingHub = messagingHub;
        _logger = logger;
        _cache = cache;
    }

    public async Task RunJob(ServerScheduleJobReq jobReq)
    {
        foreach (var idAddress in jobReq.ConnectionIdAddress)
        {
            try
            {
                await GetClientProxy(idAddress.Key).RunJob(jobReq);

                _cache.GetOrAdd(jobReq.TaskId,
                    () => new TaskReqCacheItem { TaskId = jobReq.TaskId, ConnectionId = idAddress.Key },
                    () => new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(10),
                    });

                _logger.LogDebug("send schedule request to worker(Address) successfully.", idAddress.Value);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "{Address} run job failed, try run next address.", idAddress.Value);
            }
        }
    }

    public async Task StopJob(Guid taskId)
    {
        var cacheValue = _cache.Get(taskId);
        if (cacheValue != null)
        {
            await GetClientProxy(cacheValue.ConnectionId).StopJob(taskId);
        }
    }

    private IWorkerClient GetClientProxy(string connectionId)
    {
        return _messagingHub.Clients.Client(connectionId);
    }
}
