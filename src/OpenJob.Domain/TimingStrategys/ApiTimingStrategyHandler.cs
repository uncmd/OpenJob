using OpenJob.Enums;

namespace OpenJob.TimingStrategys;

public class ApiTimingStrategyHandler : TimingStrategyHandlerBase
{
    public override TimeExpression SupportType()
    {
        return TimeExpression.Api;
    }
}
