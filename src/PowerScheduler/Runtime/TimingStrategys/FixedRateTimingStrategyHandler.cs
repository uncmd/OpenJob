using PowerScheduler.Entities.Enums;

namespace PowerScheduler.Runtime.TimingStrategys;

public class FixedRateTimingStrategyHandler : TimingStrategyHandlerBase
{
    public override TimeExpression SupportType()
    {
        return TimeExpression.FixedRate;
    }

    public override void Validate(string timeExpression)
    {
        var errorMessage = $"{nameof(TimeExpression.FixedRate)} validate timeExpression: {timeExpression} error.";

        if (!long.TryParse(timeExpression, out long delay))
        {
            throw new PowerSchedulerException($"{errorMessage} invalid timeExpression!");
        }

        if (delay < 0)
        {
            throw new PowerSchedulerException($"{errorMessage} the rate must be greater than 0 ms");
        }

        // 默认 120s ，超过这个限制应该使用考虑使用其他类型以减少资源占用
        var maxInterval = 120000;
        if (delay > maxInterval)
        {
            throw new PowerSchedulerException($"{errorMessage} the rate must be less than {maxInterval} ms");
        }
    }

    public override DateTime? CalculateNextTriggerTime(DateTime preTriggerTime, string timeExpression, DateTime? startTime, DateTime? endTime)
    {
        var nextTriggerTime = startTime.HasValue && startTime.Value > preTriggerTime ?
            startTime.Value :
            preTriggerTime.AddSeconds(int.Parse(timeExpression));

        return endTime.HasValue && endTime.Value < nextTriggerTime ?
            null :
            nextTriggerTime;
    }
}
