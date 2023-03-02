using Volo.Abp.Application.Dtos;

namespace OpenJob.Services.Dtos;

public class GetAndFilterListDto : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }
}
