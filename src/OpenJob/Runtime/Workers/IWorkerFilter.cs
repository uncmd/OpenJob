using OpenJob.Entities;
using Volo.Abp.DependencyInjection;

namespace OpenJob.Runtime.Workers;

public interface IWorkerFilter : ITransientDependency
{
    bool Filter(WorkerInfo worker, JobInfo job);
}
