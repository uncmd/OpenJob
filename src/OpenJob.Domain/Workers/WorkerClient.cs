using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OpenJob.Core;
using OpenJob.Hubs;
using OpenJob.Model;
using Volo.Abp.DependencyInjection;

namespace OpenJob.Workers;

public class WorkerClient : IWorkerClient, ITransientDependency
{
    private readonly IHubContext<MessagingHub, IWorkerClient> _messagingHub;
    private readonly ILogger<WorkerClient> _logger;
    public static readonly Dictionary<Guid, string> RuningJobs = new Dictionary<Guid, string>();

    public WorkerClient(
        IHubContext<MessagingHub,
            IWorkerClient> messagingHub,
        ILogger<WorkerClient> logger)
    {
        _messagingHub = messagingHub;
        _logger = logger;
    }

    public async Task<string> RunJob(ServerScheduleJobReq jobReq)
    {
        foreach (var address in jobReq.AllWorkerAddress)
        {
            try
            {
                await GetClientProxy(address).RunJob(jobReq);
                RuningJobs.Add(jobReq.TaskId, address);

                _logger.LogDebug("[Dispatcher-{JobName}|{TaskId}] send schedule request to TaskTracker[address:{}] successfully.", jobReq.JobId, jobReq.TaskId, address);

                return address;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "{Address} run job failed, try run next address.", address);
            }
        }

        return string.Empty;
    }

    public async Task StopJob(Guid taskId)
    {
        var userId = RuningJobs.GetOrDefault(taskId);
        if (!userId.IsNullOrEmpty())
        {
            await GetClientProxy(userId).StopJob(taskId);
        }
    }

    private IWorkerClient GetClientProxy(string userId)
    {
        return _messagingHub.Clients.User(userId);
    }
}
