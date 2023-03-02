using OpenJob.Localization;
using Volo.Abp.AspNetCore.Components;

namespace OpenJob.Blazor;

public abstract class OpenJobComponentBase : AbpComponentBase
{
    protected OpenJobComponentBase()
    {
        LocalizationResource = typeof(OpenJobResource);
    }
}
