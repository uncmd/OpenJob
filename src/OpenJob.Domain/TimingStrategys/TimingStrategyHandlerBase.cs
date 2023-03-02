using OpenJob.Enums;

namespace OpenJob.TimingStrategys;

public abstract class TimingStrategyHandlerBase : ITimingStrategyHandler
{
    public virtual void Validate(string timeExpression)
    {
        // do nothing
    }

    public abstract TimeExpression SupportType();

    public virtual DateTime? CalculateNextTriggerTime(DateTime preTriggerTime, string timeExpression, DateTime? startTime, DateTime? endTime)
    {
        // do nothing
        return null;
    }

    public override string ToString()
    {
        return $"{SupportType()}-{GetType().Name}";
    }
}
