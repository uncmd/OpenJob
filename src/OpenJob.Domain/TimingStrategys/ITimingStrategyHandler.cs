using OpenJob.Enums;
using Volo.Abp.DependencyInjection;

namespace OpenJob.TimingStrategys;

/// <summary>
/// 定时策略处理
/// </summary>
public interface ITimingStrategyHandler : ITransientDependency
{
    /// <summary>
    /// 校验表达式
    /// </summary>
    /// <param name="timeExpression">时间表达式</param>
    void Validate(string timeExpression);

    /// <summary>
    /// 支持的定时策略
    /// </summary>
    /// <returns></returns>
    TimeExpression SupportType();

    /// <summary>
    /// 计算下次触发时间
    /// </summary>
    /// <param name="preTriggerTime">上次触发时间</param>
    /// <param name="timeExpression">时间表达式</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <returns></returns>
    DateTime? CalculateNextTriggerTime(DateTime preTriggerTime, string timeExpression, DateTime? startTime, DateTime? endTime);
}
