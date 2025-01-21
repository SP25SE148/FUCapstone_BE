using AutoMapper;
using FUC.Common.Contracts;
using Identity.API.Models;

namespace Identity.API.Mapper;

public class ServiceProfiles : Profile
{
    public ServiceProfiles()
    {
        CreateMap<ApplicationUser, UserSync>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));
    }
}
