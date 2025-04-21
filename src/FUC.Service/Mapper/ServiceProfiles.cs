using AutoMapper;
using FUC.Data.Entities;
using FUC.Data.Enums;
using FUC.Service.DTOs.CampusDTO;
using FUC.Service.DTOs.CapstoneDTO;
using FUC.Service.DTOs.ConfigDTO;
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
            .ForMember(s => s.Gpa, opt => opt.MapFrom(s => s.GPA))
            .ForMember(s => s.BusinessArea, opt => opt.MapFrom(s => s.BusinessArea.Name))
            .ForMember(s => s.Skills, opt => opt.MapFrom(s => s.Skills))
            .ForMember(s => s.IsHaveBeenJoinGroup,
                opt => opt.MapFrom(s => s.GroupMembers.Any(gm => gm.Status.Equals(GroupMemberStatus.Accepted))));

        // Supervisor mapping
        CreateMap<Supervisor, SupervisorResponseDTO>()
            .ForMember(s => s.MajorName, opt => opt.MapFrom(s => s.Major.Name))
            .ForMember(s => s.CampusName, opt => opt.MapFrom(s => s.Campus.Name));

        CreateMap<UpdateTopicRequest, Topic>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TopicId))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<UpdateTaskRequest, FucTask>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TaskId))
            .ForMember(dest => dest.AssigneeId, opt => opt.Condition(src => !string.IsNullOrEmpty(src.AssigneeId)))
            .ForMember(dest => dest.Description,
                opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.Description)))
            .ForMember(dest => dest.DueDate, opt => opt.Condition(src => src.DueDate.HasValue))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<UpdateProjectProgressWeekRequest, ProjectProgressWeek>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProjectProgressWeekId))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<SummaryProjectProgressWeekRequest, ProjectProgressWeek>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProjectProgressWeekId))
            .ForMember(dest => dest.ProgressWeekSummary, opt => opt.MapFrom(src => src.Summary));

        CreateMap<FucTask, FucTaskResponse>();

        CreateMap<FucTaskHistory, FucTaskHistoryDto>();

        CreateMap<UpdateTimeConfigurationRequest, TimeConfiguration>()
            .ForMember(dest => dest.TeamUpDate, opt => opt.Condition(src => src.TeamUpDate.HasValue))
            .ForMember(dest => dest.TeamUpExpirationDate,
                opt => opt.Condition(src => src.TeamUpExpirationDate.HasValue))
            .ForMember(dest => dest.RegistTopicForSupervisorDate,
                opt => opt.Condition(src => src.RegistTopicForSupervisorDate.HasValue))
            .ForMember(dest => dest.RegistTopicForSupervisorExpiredDate,
                opt => opt.Condition(src => src.RegistTopicForSupervisorExpiredDate.HasValue))
            .ForMember(dest => dest.RegistTopicForGroupDate,
                opt => opt.Condition(src => src.RegistTopicForGroupDate.HasValue))
            .ForMember(dest => dest.RegistTopicForGroupExpiredDate,
                opt => opt.Condition(src => src.RegistTopicForGroupExpiredDate.HasValue))
            .ForMember(dest => dest.ReviewAttemptDate, opt => opt.Condition(src => src.ReviewAttemptDate.HasValue))
            .ForMember(dest => dest.ReviewAttemptExpiredDate,
                opt => opt.Condition(src => src.ReviewAttemptExpiredDate.HasValue))
            .ForMember(dest => dest.DefendCapstoneProjectDate,
                opt => opt.Condition(src => src.DefendCapstoneProjectDate.HasValue))
            .ForMember(dest => dest.DefendCapstoneProjectExpiredDate,
                opt => opt.Condition(src => src.DefendCapstoneProjectExpiredDate.HasValue))
            .ForMember(dest => dest.IsActived, opt => opt.Condition(src => src.IsActived))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<UpdateProjectProgressRequest, ProjectProgress>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
