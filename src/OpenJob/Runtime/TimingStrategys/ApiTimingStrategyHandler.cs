using OpenJob.Enums;

namespace OpenJob.Runtime.TimingStrategys;

public class ApiTimingStrategyHandler : TimingStrategyHandlerBase
{
    public override TimeExpression SupportType()
    {
        return TimeExpression.Api;
    }
}
