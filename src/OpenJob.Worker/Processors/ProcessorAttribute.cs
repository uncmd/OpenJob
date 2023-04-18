using System.Reflection;

namespace OpenJob.Processors;

[AttributeUsage(AttributeTargets.Class)]
public class ProcessorAttribute : Attribute
{
    /// <summary>
    /// Default: true.
    /// </summary>
    public bool IsEnabled { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public ProcessorAttribute(string name, bool isEnabled = true, string description = "")
    {
        IsEnabled = isEnabled;
        Name = name;
        Description = description;
    }

    public static string GetProcessorName(Type type)
    {
        var processorAttr = type.GetTypeInfo().GetCustomAttribute<ProcessorAttribute>();
        if (processorAttr != null)
        {
            return processorAttr.Name;
        }

        return type.Name;
    }

    public static bool GetProcessorIsEnabled(Type type)
    {
        var processorAttr = type.GetTypeInfo().GetCustomAttribute<ProcessorAttribute>();
        if (processorAttr != null)
        {
            return processorAttr.IsEnabled;
        }

        return true;
    }

    public static string GetProcessorDescription(Type type)
    {
        var processorAttr = type.GetTypeInfo().GetCustomAttribute<ProcessorAttribute>();
        if (processorAttr != null)
        {
            return processorAttr.Description;
        }

        return string.Empty;
    }
}
