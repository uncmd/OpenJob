using OpenJob.Model;

namespace OpenJob;

public interface ISystemMetricsCollector
{
    SystemMetrics Collect();
}
