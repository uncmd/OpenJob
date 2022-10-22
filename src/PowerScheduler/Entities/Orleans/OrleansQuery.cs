using Volo.Abp.Domain.Entities;

namespace PowerScheduler.Entities.Orleans;

public class OrleansQuery : Entity
{
    public string QueryKey { get; set; }

    public string QueryText { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { QueryKey };
    }

    protected OrleansQuery() { }

    public OrleansQuery(string queryKey, string queryText)
    {
        QueryKey = queryKey;
        QueryText = queryText;
    }
}
