using Volo.Abp.Application.Dtos;

namespace OpenJob.Jobs;

public class GetAndFilterListDto : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }
}
