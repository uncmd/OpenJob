using Volo.Abp.Domain.Entities.Auditing;

namespace PowerScheduler.Entities;

public class SchedulerApp : AuditedAggregateRoot<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }
}
