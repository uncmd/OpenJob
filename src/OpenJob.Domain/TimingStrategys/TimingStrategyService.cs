using Microsoft.Extensions.Logging;
using OpenJob.Enums;
using OpenJob.Jobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace OpenJob.TimingStrategys;

public class TimingStrategyService : ISingletonDependency
{
    private readonly Dictionary<TimeExpression, ITimingStrategyHandler> _strategyContainer;

    protected IClock Clock { get; }

    public TimingStrategyService(
        IEnumerable<ITimingStrategyHandler> timingStrategyHandlers,
        ILogger<TimingStrategyService> logger,
        IClock clock)
    {
        Clock = clock;
        _strategyContainer = new Dictionary<TimeExpression, ITimingStrategyHandler>();
        foreach (var timingStrategyHandler in timingStrategyHandlers)
        {
            logger.LogInformation("register timing strategy handler: {TimingStrategyHandler}", timingStrategyHandler);
            _strategyContainer[timingStrategyHandler.SupportType()] = timingStrategyHandler;
        }
    }

    public void CalculateNextTriggerTime(JobInfo jobInfo)
    {
        // 阻塞状态并没有实际执行，此时忽略次数递增和最近运行时间赋值
        if (jobInfo.JobStatus != JobStatus.Blocked)
        {
            jobInfo.LastTriggerTime = jobInfo.NextTriggerTime;
            jobInfo.NumberOfRuns++;
        }

        // 计算任务下一次调度时间
        jobInfo.NextTriggerTime = CalculateNextTriggerTime(jobInfo, Clock.Now);

        // 检查下一次执行信息
        CheckAndFixNextOccurrence(jobInfo);
    }

    public DateTime? CalculateNextTriggerTime(JobInfo schedulerJob, DateTime startAt)
    {
        // 如果不是正常的触发器状态，则返回 null
        if (schedulerJob.JobStatus != JobStatus.Ready
            && schedulerJob.JobStatus != JobStatus.ErrorToReady
            && schedulerJob.JobStatus != JobStatus.Running
            && schedulerJob.JobStatus != JobStatus.Blocked)
            return null;

        // 如果已经设置了 NextTriggerTime 且其值大于当前时间，则返回当前 NextTriggerTime（可能因为其他方式修改了改值导致触发时间不是精准计算的时间）
        if (schedulerJob.NextTriggerTime != null && schedulerJob.NextTriggerTime.Value > startAt)
            return schedulerJob.NextTriggerTime;

        return CalculateNextTriggerTime(
            schedulerJob.NextTriggerTime,
            schedulerJob.TimeExpression,
            schedulerJob.TimeExpressionValue,
            schedulerJob.BeginTime,
            schedulerJob.EndTime);
    }

    /// <summary>
    /// 计算下次的调度时间
    /// </summary>
    /// <param name="preTriggerTime">上次触发时间</param>
    /// <param name="timeExpressionType">定时表达式类型</param>
    /// <param name="timeExpression">表达式</param>
    /// <param name="startTime">起始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <returns>下次的调度时间</returns>
    public DateTime? CalculateNextTriggerTime(DateTime? preTriggerTime, TimeExpression timeExpressionType, string timeExpression, DateTime? startTime, DateTime? endTime)
    {
        if (!preTriggerTime.HasValue || preTriggerTime < DateTime.Now)
        {
            preTriggerTime = DateTime.Now;
        }

        return GetHandler(timeExpressionType).CalculateNextTriggerTime(preTriggerTime.Value, timeExpression, startTime, endTime);
    }

    /// <summary>
    /// 计算接下来几次的调度时间
    /// </summary>
    /// <param name="timeExpressionType">定时表达式类型</param>
    /// <param name="timeExpression">表达式</param>
    /// <param name="startTime">起始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <param name="times">次数，默认值为：5</param>
    /// <returns>调度时间列表</returns>
    public List<string> CalculateNextTriggerTimes(TimeExpression timeExpressionType, string timeExpression, DateTime startTime, DateTime endTime, int times = 5)
    {
        var timingStrategyHandler = GetHandler(timeExpressionType);
        List<DateTime> triggerTimeList = new List<DateTime>(times);
        DateTime? nextTriggerTime = DateTime.Now;
        do
        {
            nextTriggerTime = timingStrategyHandler.CalculateNextTriggerTime(nextTriggerTime.Value, timeExpression, startTime, endTime);
            if (nextTriggerTime == null)
            {
                break;
            }
            triggerTimeList.Add(nextTriggerTime.Value);
        } while (triggerTimeList.Count < times);

        if (triggerTimeList.Count == 0)
        {
            return new List<string> { "It is valid, but has not trigger time list!" };
        }

        return triggerTimeList.Select(p => p.ToString("yyyy-MM-dd HH:mm:ss")).ToList();
    }

    public DateTime? CalculateNextTriggerTimeWithInspection(TimeExpression timeExpressionType, string timeExpression, DateTime startTime, DateTime endTime)
    {
        DateTime? nextTriggerTime = CalculateNextTriggerTime(null, timeExpressionType, timeExpression, startTime, endTime);

        // 首次计算触发时间时必须计算出一个有效值
        if (timeExpressionType == TimeExpression.Cron && nextTriggerTime == null)
        {
            throw new OpenJobException("time expression is out of date: " + timeExpression);
        }

        return nextTriggerTime;
    }

    public void Validate(TimeExpression timeExpressionType, string timeExpression, DateTime? startTime, DateTime? endTime)
    {
        if (endTime.HasValue)
        {
            if (endTime <= DateTime.Now)
            {
                throw new OpenJobException("lifecycle is out of date!");
            }
            if (startTime.HasValue && startTime > endTime)
            {
                throw new OpenJobException("lifecycle is invalid! start time must earlier then end time.");
            }
        }
        GetHandler(timeExpressionType).Validate(timeExpression);
    }

    private ITimingStrategyHandler GetHandler(TimeExpression timeExpressionType)
    {
        if (!_strategyContainer.ContainsKey(timeExpressionType))
        {
            throw new OpenJobException($"No matching TimingStrategyHandler for this TimeExpressionType: {timeExpressionType}");
        }

        return _strategyContainer[timeExpressionType];
    }

    /// <summary>
    /// 检查下一次执行信息并修正 <see cref="NextTriggerTime"/> 和 <see cref="JobStatus"/>
    /// </summary>
    /// <returns></returns>
    internal static bool CheckAndFixNextOccurrence(JobInfo jobInfo)
    {
        // 检查作业执行信息
        if (jobInfo.ProcessorInfo.IsNullOrEmpty())
        {
            jobInfo.JobStatus = JobStatus.Unhandled;
            jobInfo.NextTriggerTime = null;
            return false;
        }

        // 开始时间检查
        if (jobInfo.BeginTime != null && jobInfo.NextTriggerTime != null && jobInfo.BeginTime.Value > jobInfo.NextTriggerTime.Value)
        {
            jobInfo.JobStatus = JobStatus.Backlog;
            jobInfo.NextTriggerTime = null;
            return false;
        }

        // 结束时间检查
        if (jobInfo.EndTime != null && jobInfo.NextTriggerTime != null && jobInfo.EndTime.Value < jobInfo.NextTriggerTime.Value)
        {
            jobInfo.JobStatus = JobStatus.Archived;
            jobInfo.NextTriggerTime = null;
            return false;
        }

        // 最大次数判断
        if (jobInfo.MaxNumberOfRuns > 0 && jobInfo.NumberOfRuns >= jobInfo.MaxNumberOfRuns)
        {
            jobInfo.JobStatus = JobStatus.Overrun;
            jobInfo.NextTriggerTime = null;
            return false;
        }

        // 最大错误数判断
        if (jobInfo.MaxNumberOfErrors > 0 && jobInfo.NumberOfErrors >= jobInfo.MaxNumberOfErrors)
        {
            jobInfo.JobStatus = JobStatus.Panic;
            jobInfo.NextTriggerTime = null;
            return false;
        }

        // 状态检查
        if (!jobInfo.IsNormalStatus())
        {
            return false;
        }

        // 下一次运行时间空判断
        if (jobInfo.NextTriggerTime == null)
        {
            if (jobInfo.IsNormalStatus())
                jobInfo.JobStatus = JobStatus.Unoccupied;
            return false;
        }

        return true;
    }
}
