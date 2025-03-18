namespace FUC.Service.DTOs.ReviewCalendarDTO;

public sealed class ReviewCalendarPreviewResponse
{
    public Guid Id { get; set; }
    public Guid TopicId { get; set; }
    public string TopicCode { get; set; }
    public Guid GroupId { get; set; }
    public string GroupCode { get; set; }
    public string TopicEnglishName { get; set; }
    public int Attempt { get; set; }
    public int Slot { get; set; }
    public string Room { get; set; }
    public DateTime Date { get; set; }
    public IReadOnlyCollection<ReviewersPreviewResponse> Reviewers { get; set; }
}

public sealed class ReviewersPreviewResponse
{
    public string SupervisorCode { get; set; }
}
