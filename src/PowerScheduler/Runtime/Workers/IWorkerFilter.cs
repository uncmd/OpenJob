using PowerScheduler.Entities;
using Volo.Abp.DependencyInjection;

namespace PowerScheduler.Runtime.Workers;

public interface IWorkerFilter : ITransientDependency
{
    bool Filter(SchedulerWorker worker, SchedulerJob job);
}
