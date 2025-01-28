using AutoMapper;
using FUC.Data.Entities;
using FUC.Service.DTOs.CampusDTO;
using FUC.Service.DTOs.CapstoneDTO;
using FUC.Service.DTOs.MajorDTO;
using FUC.Service.DTOs.MajorGroupDTO;

namespace FUC.Service.Mapper;

public class ServiceProfiles : Profile
{
    public ServiceProfiles()
    {
        // Campus mapping
        CreateMap<Campus, CampusResponse>()
            .ForMember(src => src.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
            .ForMember(src => src.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt));
     
        
        // Capstone mapping
        CreateMap<Capstone,CapstoneResponse>()
            .ForMember(src => src.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
            .ForMember(src => src.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt));

        
        // Major Group mapping
        CreateMap<MajorGroup, MajorGroupResponse>()
            .ForMember(src => src.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
            .ForMember(src => src.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt));

        // Major mapping
        CreateMap<Major, MajorResponse>()
            .ForMember(src => src.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
            .ForMember(src => src.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt));
        
    }
}
