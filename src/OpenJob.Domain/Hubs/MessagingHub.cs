using Microsoft.Extensions.Logging;
using OpenJob.Apps;
using OpenJob.Core;
using OpenJob.Jobs;
using OpenJob.Model;
using OpenJob.TimingStrategys;
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
    private readonly IJobInfoRepository _jobInfoRepository;

    protected IGuidGenerator GuidGenerator
        => LazyServiceProvider.LazyGetService<IGuidGenerator>(SimpleGuidGenerator.Instance);

    protected TimingStrategyService TimingStrategyService
        => LazyServiceProvider.LazyGetService<TimingStrategyService>();

    public MessagingHub(
        IAppInfoRepository appRepository,
        IRepository<WorkerInfo, Guid> workerRepository,
        IJobInfoRepository jobInfoRepository)
    {
        _appRepository = appRepository;
        _workerRepository = workerRepository;
        _jobInfoRepository = jobInfoRepository;
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

    public async Task WorkerHeartbeat(WorkerHeartbeatDto heartbeatDto)
    {
        var workerInfo = await _workerRepository
            .FirstOrDefaultAsync(p => p.AppId == heartbeatDto.AppId && p.Address == heartbeatDto.WorkerAddress);

        if (workerInfo == null)
        {
            workerInfo = new WorkerInfo(GuidGenerator.Create(), heartbeatDto.AppId)
            {
                Address = heartbeatDto.WorkerAddress,
                LastActiveTime = Clock.Now,
                Client = heartbeatDto.Client,
                ConnectionId = heartbeatDto.ConnectionId,
            };

            await _workerRepository.InsertAsync(workerInfo, autoSave: true);
        }
        else
        {
            workerInfo.LastActiveTime = DateTime.Now;
            workerInfo.Client = heartbeatDto.Client;
            workerInfo.ConnectionId = heartbeatDto.ConnectionId;
            await _workerRepository.UpdateAsync(workerInfo, autoSave: true);
        }

        await Groups.AddToGroupAsync(heartbeatDto.ConnectionId, heartbeatDto.AppName);
    }

    public Task<WrapResult> ReportInstanceStatus(InstanceStatusDto statusDto)
    {
        Logger.LogInformation("{@InstanceStatus}", statusDto);
        return Task.FromResult(WrapResult.OK());
    }

    public Task ReportLog()
    {
        return Task.FromResult(WrapResult.OK());
    }

    public async Task RegisterProcessor(ProcessorReq req)
    {
        foreach (var job in req.Dtls)
        {
            var jobInfo = await _jobInfoRepository
                .FirstOrDefaultAsync(p => p.Name == job.Name && p.AppId == req.AppId);

            if (jobInfo == null)
            {
                jobInfo = new JobInfo(GuidGenerator.Create(), req.AppId, job.Name)
                {
                    ProcessorInfo = job.ProcessorInfo,
                    TimeExpression = Enums.TimeExpression.FixedRate,
                    TimeExpressionValue = "30",
                    JobStatus = Enums.JobStatus.Ready
                };

                TimingStrategyService.CalculateNextTriggerTime(jobInfo);
                await _jobInfoRepository.InsertAsync(jobInfo, autoSave: true);
            }
        }

        Logger.LogInformation("{@ProcessorReq}", req);
    }
}
