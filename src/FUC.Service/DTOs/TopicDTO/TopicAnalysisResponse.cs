namespace FUC.Service.DTOs.TopicDTO;

public class TopicStatisticResponse
{
    public IReadOnlyCollection<TopicAnalysisResponse>? Analysises { get; set; }
    public double Over60Ratio { get; set; }
    public double Over80Ratio { get; set; }
    public DateTime CreatedDate { get; set; }
    public required string ProcessedBy { get; set; }
    public string StatusSemantic { get; set; }
}

public class TopicAnalysisResponse
{
    public string AnalysisTopicId { get; set; } // the other topics
    public string EnglishName { get; set; }
    public double Similarity { get; set; }
}
