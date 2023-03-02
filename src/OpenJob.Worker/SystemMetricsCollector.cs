using OpenJob.Model;

namespace OpenJob;

public class SystemMetricsCollector : ISystemMetricsCollector
{
    public SystemMetrics Collect()
    {
        var systemMetrics = new SystemMetrics
        {
            CpuProcessors = Environment.ProcessorCount,
            CpuLoad = 0,
            MemoryTotal = Environment.WorkingSet / 1024 / 1024 / 1024,
            MemoryUsed = 0,
            DiskTotal = 0,
            DiskUsed = 0,
        };

        systemMetrics.CalculateScore();

        return systemMetrics;
    }
}
