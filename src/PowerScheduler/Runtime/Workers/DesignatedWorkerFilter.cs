using PowerScheduler.Entities;

namespace PowerScheduler.Runtime.Workers;

/// <summary>
/// 指定 Worker 标签
/// </summary>
public class DesignatedWorkerFilter : IWorkerFilter
{
    public bool Filter(SchedulerWorker worker, SchedulerJob job)
    {
        // 未指定则不过滤
        if (job.Labels.IsNullOrWhiteSpace())
            return false;

        var separators = new char[] { ',', ';' };
        var labels = job.Labels.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        List<string> workerLabels = new List<string>();
        if (!worker.Labels.IsNullOrWhiteSpace())
        {
            workerLabels = worker.Labels.Split(separators, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        foreach (var label in labels)
        {
            if (label == worker.Address || workerLabels.Contains(label))
                return false;
        }

        return true;
    }
}
