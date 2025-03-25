﻿namespace FUC.Service.DTOs.ReviewCalendarDTO;

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
    public int Attempt { get; set; }
    public int Slot { get; set; }
    public string Room { get; set; }
    public DateTime Date { get; set; }
    public IReadOnlyCollection<string> ReviewersCode { get; set; }
    public IReadOnlyCollection<string?> Suggestion { get; set; }
    public IReadOnlyCollection<string?> Comment { get; set; }
}
