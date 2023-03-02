using OpenJob.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace OpenJob;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(OpenJobEntityFrameworkCoreModule)
)]
public class OpenJobServerModule : AbpModule
{

}
