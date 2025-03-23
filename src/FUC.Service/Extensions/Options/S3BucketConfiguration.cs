namespace FUC.Service.Extensions.Options;

public class S3BucketConfiguration
{
    public string FUCTopicBucket { get; set; } = string.Empty;
    public string FUCTemplateBucket { get; set; } = string.Empty;
    public string FUCGroupDocumentBucket { get; set; } = string.Empty;
    public string FUCThesisBucket { get; set; } = string.Empty;
    public string EvaluationProjectProgressKey { get; set; } = string.Empty;
    public string ReviewsCalendarsKey { get; set; } = string.Empty;
    public string DefenseCalendarKey { get; set; } = string.Empty;
    public string EvaluationWeeklyKey { get; set; } = string.Empty;
    public string StudentsTemplateKey { get; set; } = string.Empty;
    public string SupervisorsTemplateKey { get; set; } = string.Empty;
    public string ThesisCouncilMeetingMinutesTemplateKey { get; set; } = string.Empty;
}
