using OpenJob.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace OpenJob.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class OpenJobController : AbpControllerBase
{
    protected OpenJobController()
    {
        LocalizationResource = typeof(OpenJobResource);
    }
}
