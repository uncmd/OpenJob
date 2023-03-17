using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace OpenJob.Apps;

public class AppInfoDto : FullAuditedEntityDto<Guid>
{
    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsEnabled { get; set; }
}
