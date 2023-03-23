using Volo.Abp.Modularity;

namespace OpenJob;

[DependsOn(
    typeof(OpenJobApplicationModule),
    typeof(OpenJobDomainTestModule)
    )]
public class OpenJobApplicationTestModule : AbpModule
{

}
