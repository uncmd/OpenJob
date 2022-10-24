using Microsoft.Extensions.Logging.Abstractions;
using System.Diagnostics;
using Volo.Abp.DependencyInjection;

namespace PowerScheduler.Runtime.Processors;

public abstract class ProcessorBase : IProcessor
{
    public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

    protected ILoggerFactory LoggerFactory => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();

    protected ILogger Logger => LazyServiceProvider.LazyGetService<ILogger>(provider => LoggerFactory?.CreateLogger(GetType().FullName) ?? NullLogger.Instance);

    public async Task<ProcessorResult> ExecuteAsync(ProcessorContext context)
    {
        string status = "unknown";
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await DoWorkAsync(context);
            status = result.Success ? "success" : "failed";
            return result;
        }
        catch (Exception ex)
        {
            status = "exception";
            Logger.LogError(ex, "execute failed!");
            return ProcessorResult.ErrorMessage(ex.Message);
        }
        finally
        {
            stopwatch.Stop();
            Logger.LogInformation("{context} ==> {Status}|{Elapsed}", context, status, stopwatch.Elapsed);
            await DisposeAsync();
            Dispose();
        }
    }

    public virtual ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public virtual void Dispose()
    {

    }

    /// <summary>
    /// 核心处理逻辑
    /// </summary>
    /// <param name="context">任务上下文，可通过 JobArgs 和 InstanceArgs 分别获取控制台参数和OpenAPI传递的任务实例参数</param>
    /// <returns>处理结果，Message有长度限制，超长会被裁剪，不允许返回 null</returns>
    protected abstract Task<ProcessorResult> DoWorkAsync(ProcessorContext context);
}
