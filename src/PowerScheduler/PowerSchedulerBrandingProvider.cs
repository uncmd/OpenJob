using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace PowerScheduler;

[Dependency(ReplaceServices = true)]
public class PowerSchedulerBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "PowerScheduler";
}
