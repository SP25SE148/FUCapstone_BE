namespace FUC.Service.DTOs.GroupDTO;

public sealed record GroupResponse
{
    public Guid Id { get; init; }
    public string SemesterName { get; init; } = string.Empty;
    public string MajorName { get; init; } = string.Empty;
    public string CapstoneName { get; init; } = string.Empty;
    public string CampusName { get; init; } = string.Empty;
    public string TopicCode { get; init; } = string.Empty;
    public string GroupCode { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public IReadOnlyList<string> MemberEmailList { get; init; } = new List<string>();
    public bool IsDeleted { get; init; }
    public DateTime CreatedDate { get; init; }
    public DateTime? UpdatedDate { get; init; }
    public string CreatedBy { get; init; } = string.Empty;
    public string? UpdatedBy { get; init; }
    public DateTime? DeletedAt { get; init; }

    // Constructor mặc định
    public GroupResponse() { }
}

