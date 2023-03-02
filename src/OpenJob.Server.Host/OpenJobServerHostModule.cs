using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace OpenJob;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(OpenJobServerModule)
)]
public class OpenJobServerHostModule : AbpModule
{
}
