using AutoMapper;
using PowerScheduler.Entities;
using PowerScheduler.Services.Dtos;

namespace PowerScheduler.ObjectMapping;

public class PowerSchedulerAutoMapperProfile : Profile
{
    public PowerSchedulerAutoMapperProfile()
    {
        /* Create your AutoMapper object mappings here */

        CreateMap<SchedulerJob, SchedulerJobDto>();
        CreateMap<CreateUpdateJobDto, SchedulerJob>();

        CreateMap<SchedulerTask, SchedulerTaskDto>();
        CreateMap<CreateUpdateTaskDto, SchedulerTask>();

        CreateMap<SchedulerWorker, SchedulerWorkerDto>();
        CreateMap<CreateUpdateWorkerDto, SchedulerWorker>();
    }
}
