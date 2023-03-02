namespace OpenJob.Data;

public interface IOpenJobDbSchemaMigrator
{
    Task MigrateAsync();
}
