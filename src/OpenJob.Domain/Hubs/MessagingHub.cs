using Microsoft.Extensions.Logging;
using OpenJob.Apps;
using OpenJob.Core;
using OpenJob.Model;
using OpenJob.Workers;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace OpenJob.Hubs;

/// <summary>
/// /signalr-hubs/messaging
/// </summary>
public class MessagingHub : AbpHub<IWorkerClient>, IServerClient
{
    private readonly IAppInfoRepository _appRepository;
    private readonly IRepository<WorkerInfo, Guid> _workerRepository;

    protected IGuidGenerator GuidGenerator 
        => LazyServiceProvider.LazyGetService<IGuidGenerator>(SimpleGuidGenerator.Instance);

    public MessagingHub(IAppInfoRepository appRepository, IRepository<WorkerInfo, Guid> workerRepository)
    {
        _appRepository = appRepository;
        _workerRepository = workerRepository;
    }

    public async Task<WrapResult> AssertApp(string appName)
    {
        var appInfo = await _appRepository.GetAsync(appName);

        if (appInfo == null)
        {
            var result = WrapResult.Fail($"App {appName} not register, please contact the administrator to register.");
            Logger.LogInformation("{@Result}", result);
            return result;
        }

        if (appInfo.IsEnabled == false)
        {
            return WrapResult.Fail($"App {appName} disabled.");
        }

        return WrapResult.OK(appInfo.Id);
    }

    public async Task<WrapResult> WorkerHeartbeat(WorkerHeartbeatDto heartbeatDto)
    {
        var workerInfo = await _workerRepository
            .GetAsync(p => p.AppId == heartbeatDto.AppId && p.Address == heartbeatDto.WorkerAddress);

        if (workerInfo == null)
        {
            workerInfo = new WorkerInfo(GuidGenerator.Create(), heartbeatDto.AppId)
            {
                Address = heartbeatDto.WorkerAddress,
                LastActiveTime = Clock.Now,
                Client = heartbeatDto.Client,
            };

            await _workerRepository.InsertAsync(workerInfo, autoSave: true);
        }
        else
        {
            workerInfo.LastActiveTime = DateTime.Now;
            workerInfo.Client = heartbeatDto.Client;
            await _workerRepository.UpdateAsync(workerInfo, autoSave: true);
        }

        Logger.LogInformation("{@Heartbeat}", heartbeatDto);

        return WrapResult.OK();
    }

    public Task<WrapResult> ReportInstanceStatus(InstanceStatusDto statusDto)
    {
        Logger.LogInformation("{@InstanceStatus}", statusDto);
        return Task.FromResult(WrapResult.OK());
    }

    public Task<WrapResult> ReportLog()
    {
        return Task.FromResult(WrapResult.OK());
    }
}
