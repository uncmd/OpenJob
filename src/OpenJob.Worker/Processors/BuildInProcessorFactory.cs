using Microsoft.Extensions.Logging;
using OpenJob.Enums;

namespace OpenJob.Processors;

/// <summary>
/// 内建的默认处理器工厂，通过全限定类名加载处理器，但无法享受 IOC 框架的 DI 功能
/// </summary>
public class BuildInProcessorFactory : ProcessorFactoryBase
{
    public BuildInProcessorFactory(ILoggerFactory loggerFactory) : base(loggerFactory) { }

    public override List<ProcessorType> SupportTypes()
    {
        return new List<ProcessorType> { ProcessorType.BuildIn };
    }

    public override IProcessor Build(ProcessorDefinition processorDefinition)
    {
        var typeName = processorDefinition.ProcessorInfo;

        try
        {
            return CreateInstance<IProcessor>(typeName);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "load local Processor({ProcessorDefinition}) failed.", processorDefinition);
        }

        return null;
    }

    /// <summary>
    /// 创建对象实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="typeName">命名空间.类型名,程序集</param>
    /// <returns></returns>
    public static T CreateInstance<T>(string typeName)
    {
        object obj = Activator.CreateInstance(Type.GetType(typeName, true), true);
        return (T)obj;
    }
}
