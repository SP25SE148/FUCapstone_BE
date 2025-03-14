using AutoMapper;
using FUC.Data.Entities;
using FUC.Data.Enums;
using FUC.Service.DTOs.CampusDTO;
using FUC.Service.DTOs.CapstoneDTO;
using FUC.Service.DTOs.MajorDTO;
using FUC.Service.DTOs.MajorGroupDTO;
using FUC.Service.DTOs.ProjectProgressDTO;
using FUC.Service.DTOs.SemesterDTO;
using FUC.Service.DTOs.StudentDTO;
using FUC.Service.DTOs.SupervisorDTO;
using FUC.Service.DTOs.TopicDTO;

namespace FUC.Service.Mapper;

public class ServiceProfiles : Profile
{
    public ServiceProfiles()
    {
        // Campus mapping
        CreateMap<Campus, CampusResponse>();

        // Capstone mapping
        CreateMap<Capstone, CapstoneResponse>();

        // Major Group mapping
        CreateMap<MajorGroup, MajorGroupResponse>();

        // Major mapping
        CreateMap<Major, MajorResponse>();

        // Semester mapping
        CreateMap<Semester, SemesterResponse>();


        // Student mapping
        CreateMap<Student, StudentResponseDTO>()
            .ForMember(s => s.MajorName, opt => opt.MapFrom(s => s.Major.Name))
            .ForMember(s => s.CapstoneName, opt => opt.MapFrom(s => s.Capstone.Name))
            .ForMember(s => s.CampusName, opt => opt.MapFrom(s => s.Campus.Name))
            .ForMember(s => s.Status, opt => opt.MapFrom(s => s.Status.ToString()))
            .ForMember(s => s.BusinessArea, opt => opt.MapFrom(s => s.BusinessArea.Name))
            .ForMember(s => s.Gpa, opt => opt.MapFrom(s => s.GPA))
            .ForMember(s => s.IsHaveBeenJoinGroup,
                opt => opt.MapFrom(s => s.GroupMembers.Any(gm => gm.Status.Equals(GroupMemberStatus.Accepted))));


        // Supervisor mapping
        CreateMap<Supervisor, SupervisorResponseDTO>()
            .ForMember(s => s.MajorName, opt => opt.MapFrom(s => s.Major.Name))
            .ForMember(s => s.CampusName, opt => opt.MapFrom(s => s.Campus.Name));

        CreateMap<UpdateTopicRequest, Topic>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TopicId))
            .ForMember(dest => dest.BusinessAreaId, opt => opt.Condition(src => src.BusinessAreaId != null))
            .ForMember(dest => dest.BusinessAreaId, opt => opt.MapFrom((src, dest) =>
                src.BusinessAreaId != null ? Guid.Parse(src.BusinessAreaId) : dest.BusinessAreaId))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<UpdateTaskRequest, FucTask>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TaskId))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<UpdateProjectProgressWeekRequest, ProjectProgressWeek>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProjectProgressWeekId))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<SummaryProjectProgressWeekRequest, ProjectProgressWeek>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProjectProgressWeekId))
            .ForMember(dest => dest.ProgressWeekSummary, opt => opt.MapFrom(src => src.Summary));
    }
}
