using OpenJob.Jobs;
using Volo.Abp.DependencyInjection;

namespace OpenJob.Workers;

public interface IWorkerFilter : ITransientDependency
{
    bool Filter(WorkerInfo worker, JobInfo job);
}
