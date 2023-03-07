using Volo.Abp.DependencyInjection;

namespace OpenJob.Orleans;

public interface IOrleansDataSeeder : ITransientDependency
{
    Task SeedAsync(string databaseProvider, Guid? tenantId = null);
}
