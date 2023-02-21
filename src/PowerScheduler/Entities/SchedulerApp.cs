using Volo.Abp.Domain.Entities.Auditing;

namespace PowerScheduler.Entities;

public sealed class SchedulerApp : AuditedAggregateRoot<Guid>
{
    public string Name { get; private set; }

    public string Description { get; set; }

    private SchedulerApp() { }

    public SchedulerApp(Guid id, string name) : base(id)
    {
        Name = name;
    }
}
