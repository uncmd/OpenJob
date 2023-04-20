using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenJob.Enums;
using OpenJob.Jobs;
using OpenJob.Tasks;
using System.Diagnostics;
using Volo.Abp.Domain.Repositories;

namespace OpenJob.Server.Actors;

public class SchedulerJobActor : ActorBase, ISchedulerJobActor
{
    private readonly OpenJobServerOptions _options;
    private readonly IJobInfoRepository _jobRepository;
    private readonly IRepository<TaskInfo, Guid> _taskRepository;

    public SchedulerJobActor(
        IOptions<OpenJobServerOptions> options,
        IJobInfoRepository jobRepository,
        IRepository<TaskInfo, Guid> taskRepository)
    {
        _options = options.Value;
        _jobRepository = jobRepository;
        _taskRepository = taskRepository;
    }

    public async Task ScheduleJob(Guid appId, string appName)
    {
        using (Logger.BeginScope("AppName:{AppName}({Id})", appName, appId))
        {
            var stopwatch = Stopwatch.StartNew();

            // 查询即将要执行的任务
            var jobs = await _jobRepository.GetPreJobs(appId);
            if (!jobs.Any())
            {
                Logger.LogInformation(L["NoScheduleJob"]);
                return;
            }

            try
            {
                await ScheduleJob(jobs);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "app dispatch job failed");
            }

            stopwatch.Stop();
            Logger.LogInformation("current app schedule finish({Cost}ms)", stopwatch.ElapsedMilliseconds);

            if (stopwatch.Elapsed > _options.SchedulePeriod)
            {
                Logger.LogWarning("The database query is using too much time({Cost}ms), please check if the database load is too high!", stopwatch.ElapsedMilliseconds);
            }
        }
    }

    protected virtual async Task ScheduleJob(List<JobInfo> jobs)
    {
        Logger.LogDebug("These {Count} jobs will be scheduled.", jobs.Count);

        // 批量写任务表
        List<TaskInfo> schedulerTasks = new List<TaskInfo>();
        foreach (var job in jobs)
        {
            var expectedTriggerTime = job.NextTriggerTime.Value;
            // 任务过期
            if (job.NextTriggerTime < Clock.Now)
            {
                Logger.LogWarning("[JobName:{JobName}({JobId})] schedule delay, expect: {NextTriggerTime}, current: {Now}",
                    job.Name, job.Id, job.NextTriggerTime, Clock.Now);

                if (job.MisfireStrategy == MisfireStrategy.Ignore)
                {
                    Logger.LogInformation("[JobName:{JobName}({JobId})] MisfireStrategy is {MisfireStrategy}, continue this job",
                        job.Name, job.Id, job.MisfireStrategy);

                    continue;
                }
                else
                {
                    expectedTriggerTime = Clock.Now;
                }
            }
            schedulerTasks.Add(new TaskInfo(GuidGenerator.Create(), job.Id, expectedTriggerTime));
        }

        await _taskRepository.InsertManyAsync(schedulerTasks, true);

        // 激活Actor等待调度执行
        List<Task> tasks = new List<Task>();
        foreach (var taskInfo in schedulerTasks)
        {
            TimeSpan dueTime = taskInfo.ExpectedTriggerTime - Clock.Now;

            var taskActor = ActorFactory.GetGrain<ISchedulerTaskActor>(taskInfo.Id);
            var task = taskActor.DispatchTask(taskInfo.Id, dueTime);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        // 计算下一次调度时间
        // todo: 间隔内的重复调度？最小的连续执行间隔为OpenJobServerOptions.SchedulePeriod
        await _jobRepository.RefreshNextTriggerTime(jobs);
    }
}
