using Volo.Abp.Application.Dtos;

namespace OpenJob;

public class GetAndFilterListDto : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }
}
