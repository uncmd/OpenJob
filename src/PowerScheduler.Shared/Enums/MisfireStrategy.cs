﻿namespace PowerScheduler.Enums;

/// <summary>
/// 过期策略
/// </summary>
public enum MisfireStrategy
{
    /// <summary>
    /// 忽略
    /// </summary>
    Ignore,

    /// <summary>
    /// 立即触发补偿一次
    /// </summary>
    FireOnceNow
}
