using PowerScheduler.Entities;
using PowerScheduler.Enums;
using PowerScheduler.Runtime.TimingStrategys;
using Volo.Abp.DependencyInjection;

namespace PowerScheduler.Domain;

public class TimingStrategyService : ISingletonDependency
{
    private readonly Dictionary<TimeExpression, ITimingStrategyHandler> _strategyContainer;

    public TimingStrategyService(
        IEnumerable<ITimingStrategyHandler> timingStrategyHandlers,
        ILogger<TimingStrategyService> logger)
    {
        _strategyContainer = new Dictionary<TimeExpression, ITimingStrategyHandler>();
        foreach (var timingStrategyHandler in timingStrategyHandlers)
        {
            logger.LogInformation("register timing strategy handler: {TimingStrategyHandler}", timingStrategyHandler);
            _strategyContainer[timingStrategyHandler.SupportType()] = timingStrategyHandler;
        }
    }

    public DateTime? CalculateNextTriggerTime(SchedulerJob schedulerJob, DateTime startAt)
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
            throw new PowerSchedulerException("time expression is out of date: " + timeExpression);
        }

        return nextTriggerTime;
    }

    public void Validate(TimeExpression timeExpressionType, string timeExpression, DateTime? startTime, DateTime? endTime)
    {
        if (endTime.HasValue)
        {
            if (endTime <= DateTime.Now)
            {
                throw new PowerSchedulerException("lifecycle is out of date!");
            }
            if (startTime.HasValue && startTime > endTime)
            {
                throw new PowerSchedulerException("lifecycle is invalid! start time must earlier then end time.");
            }
        }
        GetHandler(timeExpressionType).Validate(timeExpression);
    }

    private ITimingStrategyHandler GetHandler(TimeExpression timeExpressionType)
    {
        if (!_strategyContainer.ContainsKey(timeExpressionType))
        {
            throw new PowerSchedulerException($"No matching TimingStrategyHandler for this TimeExpressionType: {timeExpressionType}");
        }

        return _strategyContainer[timeExpressionType];
    }
}
