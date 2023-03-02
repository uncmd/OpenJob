using AutoMapper;
using OpenJob.Entities;
using OpenJob.Services.Dtos;

namespace OpenJob.ObjectMapping;

public class OpenJobAutoMapperProfile : Profile
{
    public OpenJobAutoMapperProfile()
    {
        /* Create your AutoMapper object mappings here */

        CreateMap<JobInfo, SchedulerJobDto>();
        CreateMap<CreateUpdateJobDto, JobInfo>();

        CreateMap<TaskInfo, SchedulerTaskDto>();
        CreateMap<CreateUpdateTaskDto, TaskInfo>();

        CreateMap<WorkerInfo, SchedulerWorkerDto>();
        CreateMap<CreateUpdateWorkerDto, WorkerInfo>();
    }
}
