using Volo.Abp.Application.Dtos;

namespace PowerScheduler.Services.Dtos;

public class GetAndFilterListDto : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }
}
