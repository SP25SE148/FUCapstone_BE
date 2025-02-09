namespace FUC.Service.DTOs.GroupDTO;

public sealed record GroupResponse(
    Guid Id,
    string SemesterName,
    string MajorName,
    string CampusName,
    string TopicName,
    string TopicCode,
    string GroupCode,
    string Status,
    bool IsDeleted,
    DateTime CreatedDate, 
    DateTime? UpdatedDate, 
    string CreatedBy, 
    string? UpdatedBy, 
    DateTime? DeletedAt
);
