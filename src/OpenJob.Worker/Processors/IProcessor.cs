﻿namespace OpenJob.Processors;

/// <summary>
/// 表示一个任务处理器
/// </summary>
public interface IProcessor : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// 执行任务
    /// </summary>
    /// <param name="context">处理器上下文</param>
    /// <param name="cancellationToken"></param>
    /// <returns>处理结果</returns>
    Task<ProcessorResult> ExecuteAsync(ProcessorContext context, CancellationToken cancellationToken = default);
}
