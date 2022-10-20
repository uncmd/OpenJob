using PowerScheduler.Localization;
using Volo.Abp.AspNetCore.Components;

namespace PowerScheduler;

public abstract class PowerSchedulerComponentBase : AbpComponentBase
{
    protected PowerSchedulerComponentBase()
    {
        LocalizationResource = typeof(PowerSchedulerResource);
    }
}
