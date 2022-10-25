using PowerScheduler.Enums;
using PowerScheduler.Runtime.Crons;

namespace PowerScheduler.Runtime.TimingStrategys;

public class CronTimingStrategyHandler : TimingStrategyHandlerBase
{
    public override TimeExpression SupportType()
    {
        return TimeExpression.Cron;
    }

    public override void Validate(string timeExpression)
    {
        Cron.Parse(timeExpression);
    }

    public override DateTime? CalculateNextTriggerTime(DateTime preTriggerTime, string timeExpression, DateTime? startTime, DateTime? endTime)
    {
        var baseTime = preTriggerTime;
        if (startTime.HasValue && startTime.Value > DateTime.Now && preTriggerTime < startTime.Value)
        {
            baseTime = preTriggerTime > startTime.Value ?
                preTriggerTime :
                startTime.Value;
        }

        var cron = Cron.Parse(timeExpression);
        var nextTriggerTime = cron.GetNextOccurrence(baseTime);

        return endTime.HasValue && endTime.Value < nextTriggerTime ?
            null :
            nextTriggerTime;
    }
}
