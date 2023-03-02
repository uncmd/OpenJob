using AutoMapper;
using OpenJob.Apps;
using OpenJob.Jobs;
using OpenJob.Tasks;
using OpenJob.Workers;

namespace OpenJob;

public class OpenJobApplicationAutoMapperProfile : Profile
{
    public OpenJobApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<AppInfo, AppInfoDto>();
        CreateMap<AppInfoDto, AppInfo>();

        CreateMap<JobInfo, JobInfoDto>();
        CreateMap<CreateUpdateJobDto, JobInfo>();

        CreateMap<TaskInfo, TaskInfoDto>();
        CreateMap<CreateUpdateTaskDto, TaskInfo>();

        CreateMap<WorkerInfo, WorkerInfoDto>();
        CreateMap<CreateUpdateWorkerDto, WorkerInfo>();
    }
}
