using Volo.Abp.Domain.Entities.Auditing;

namespace OpenJob.Entities;

public sealed class AppInfo : AuditedAggregateRoot<Guid>
{
    public string Name { get; private set; }

    public string Description { get; set; }

    public bool IsEnabled { get; set; }

    private AppInfo() { }

    public AppInfo(Guid id, string name) : base(id)
    {
        Name = name;
        IsEnabled = true;
    }

    public override string ToString()
    {
        return $"{base.ToString()}, Name = {Name}";
    }
}
