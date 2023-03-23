using OpenJob.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace OpenJob;

[DependsOn(
    typeof(OpenJobEntityFrameworkCoreTestModule)
    )]
public class OpenJobDomainTestModule : AbpModule
{

}
