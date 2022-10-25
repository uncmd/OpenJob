using PowerScheduler.Enums;

namespace PowerScheduler.Runtime.TimingStrategys;

public class ApiTimingStrategyHandler : TimingStrategyHandlerBase
{
    public override TimeExpression SupportType()
    {
        return TimeExpression.Api;
    }
}
