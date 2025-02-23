﻿using FUC.Data.Enums;

namespace FUC.Service.DTOs.StudentDTO;

public sealed record StudentResponseDTO
{
    public string Id { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string MajorId { get; init; } = string.Empty;
    public string MajorName { get; init; } = string.Empty;
    public string CapstoneId { get; init; } = string.Empty;
    public string CapstoneName { get; init; } = string.Empty;
    public string CampusId { get; init; } = string.Empty;
    public string CampusName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsEligible { get; init; }
    public string Status { get; init; } = string.Empty;
    public float Mark { get; set; }
    public string BusinessArea { get; set; } = string.Empty;
    
    public bool IsHaveBeenJoinGroup { get; init; }

    // Constructor mặc định
    public StudentResponseDTO() { }
}
