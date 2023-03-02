using Volo.Abp.Application.Dtos;

namespace OpenJob.Jobs;

public class AppInfoDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsEnabled { get; set; }
}
