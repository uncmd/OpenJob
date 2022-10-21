﻿namespace PowerScheduler.Entities.Enums;

/// <summary>
/// 时间表达式类型
/// </summary>
public enum TimeExpression
{
    None,

    /// <summary>
    /// Cron表达式
    /// </summary>
    Cron,

    /// <summary>
    /// Api
    /// </summary>
    Api,

    /// <summary>
    /// 固定频率(只支持60秒以上)
    /// </summary>
    FixedRate,

    /// <summary>
    /// 秒级延迟(1-60秒)
    /// </summary>
    SecondDelay,

    /// <summary>
    /// 延迟任务，到达指定延迟时间后执行一次的任务
    /// </summary>
    Delayed
}
