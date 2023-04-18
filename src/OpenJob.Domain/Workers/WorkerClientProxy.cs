using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OpenJob.Core;
using OpenJob.Hubs;
using OpenJob.Model;
using Volo.Abp.DependencyInjection;

namespace OpenJob.Workers;

[ExposeServices(typeof(IWorkerClient))]
public class WorkerClientProxy : IWorkerClient, ITransientDependency
{
    private readonly IHubContext<MessagingHub, IWorkerClient> _messagingHub;
    private readonly ILogger<WorkerClientProxy> _logger;
    public static readonly Dictionary<Guid, string> RuningJobs = new Dictionary<Guid, string>();

    public WorkerClientProxy(
        IHubContext<MessagingHub, IWorkerClient> messagingHub,
        ILogger<WorkerClientProxy> logger)
    {
        _messagingHub = messagingHub;
        _logger = logger;
    }

    public async Task<string> RunJob(ServerScheduleJobReq jobReq)
    {
        foreach (var idAddress in jobReq.ConnectionIdAddress)
        {
            try
            {
                await GetClientProxy(idAddress.Key).RunJob(jobReq);
                RuningJobs.Add(jobReq.TaskId, idAddress.Key);

                _logger.LogDebug("[Dispatcher-{JobName}|{TaskId}] send schedule request to TaskTracker[address:{}] successfully.", jobReq.JobId, jobReq.TaskId, idAddress.Value);

                return idAddress.Value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "{Address} run job failed, try run next address.", idAddress.Value);
            }
        }

        return string.Empty;
    }

    public async Task StopJob(Guid taskId)
    {
        var connectionId = RuningJobs.GetOrDefault(taskId);
        if (!connectionId.IsNullOrEmpty())
        {
            await GetClientProxy(connectionId).StopJob(taskId);
        }
    }

    private IWorkerClient GetClientProxy(string connectionId)
    {
        return _messagingHub.Clients.Client(connectionId);
    }
}
