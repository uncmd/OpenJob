using OpenJob.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace OpenJob.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(OpenJobEntityFrameworkCoreModule),
    typeof(OpenJobApplicationContractsModule)
    )]
public class OpenJobDbMigratorModule : AbpModule
{

}
