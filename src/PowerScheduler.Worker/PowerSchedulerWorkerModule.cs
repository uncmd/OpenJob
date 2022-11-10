using Volo.Abp.Modularity;

namespace PowerScheduler;

[DependsOn(
    typeof(PowerSchedulerSharedModule)
    )]
public class PowerSchedulerWorkerModule : AbpModule
{

}
