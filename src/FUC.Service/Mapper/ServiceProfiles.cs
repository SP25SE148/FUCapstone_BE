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
        CreateMap<Campus, CampusResponse>();
           
     
        
        // Capstone mapping
        CreateMap<Capstone, CapstoneResponse>();
        
        // Major Group mapping
        CreateMap<MajorGroup, MajorGroupResponse>();
        // Major mapping
        CreateMap<Major, MajorResponse>();
    }
}
