namespace FUC.Service.DTOs.ReviewCalendarDTO;

public sealed class ReviewCalendarResponse
{
    public Guid Id { get; set; }
    public Guid TopicId { get; set; }
    public string TopicCode { get; set; }
    public Guid GroupId { get; set; }
    public string GroupCode { get; set; }
    public string TopicEnglishName { get; set; }
    public string MainSupervisorCode { get; set; }
    public IReadOnlyCollection<string> CoSupervisorsCode { get; set; }
    public IReadOnlyCollection<string> Reviewers { get; set; }
    public int Attempt { get; set; }
    public int Slot { get; set; }
    public string Room { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; }
}

public sealed class ReviewCalendarResultResponse
{
    public int Attempt { get; set; }
    public IReadOnlyCollection<ReviewCalendarResultDetailResponse> ReviewCalendarResultDetailList { get; set; }
}

public sealed class ReviewCalendarResultDetailResponse
{
    public string? Suggestion { get; set; }
    public string? Comment { get; set; }
    public string? Author { get; set; }
}

public sealed class ReviewCriteriaResponse
{
    public Guid Id { get; set; }
    public int Attempt { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Requirement { get; set; }
}
