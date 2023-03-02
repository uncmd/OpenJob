using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace OpenJob.Blazor;

[Dependency(ReplaceServices = true)]
public class OpenJobBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "OpenJob";
}
