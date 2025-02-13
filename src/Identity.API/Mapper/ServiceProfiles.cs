using AutoMapper;
using FUC.Common.Contracts;
using Identity.API.Models;
using Identity.API.Payloads.Responses;

namespace Identity.API.Mapper;

public class ServiceProfiles : Profile
{
    public ServiceProfiles()
    {
        CreateMap<ApplicationUser, UserSync>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.FullName));

        CreateMap<ApplicationUser, UserResponseDTO>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.FullName));

    }
}
