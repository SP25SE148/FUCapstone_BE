using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public class TopicAnalysis : Entity
{
    public Guid Id { get; set; }
    public string AnalysisResult { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid TopicId { get; set; }

    public Topic Topic { get; set; } = null!;
}
