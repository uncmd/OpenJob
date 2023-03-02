using OpenJob.Localization;
using Volo.Abp.Application.Services;

namespace OpenJob;

/* Inherit your application services from this class.
 */
public abstract class OpenJobAppService : ApplicationService
{
    protected OpenJobAppService()
    {
        LocalizationResource = typeof(OpenJobResource);
    }
}
