﻿using AutoMapper;
using FUC.Data.Entities;
using FUC.Service.DTOs.CampusDTO;
using FUC.Service.DTOs.CapstoneDTO;
using FUC.Service.DTOs.GroupDTO;
using FUC.Service.DTOs.MajorDTO;
using FUC.Service.DTOs.MajorGroupDTO;
using FUC.Service.DTOs.SemesterDTO;

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
        
        // Group mapping
        CreateMap<Group, GroupResponse>()
            .ForMember(dest => dest.SemesterName,
                opt => opt
                    .MapFrom(src => src.Semester.Name))
            
            .ForMember(dest => dest.MajorName,
                opt => opt
                    .MapFrom(src => src.Major.Name))
            
            .ForMember(dest => dest.CampusName,
                opt => opt
                    .MapFrom(src => src.Campus.Name))
            
            .ForMember(dest => dest.CapstoneName,
                opt => opt
                    .MapFrom(src => src.Capstone.Name))
            
            .ForMember(dest => dest.MemberEmailList,
                opt => opt
                    .MapFrom(src =>  src.GroupMembers
                        .Select(gm => gm.Student.Email)
                        .ToList()));
    }
}
