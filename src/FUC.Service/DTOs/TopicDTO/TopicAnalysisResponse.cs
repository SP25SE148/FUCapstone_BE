namespace FUC.Service.DTOs.TopicDTO;

public class TopicStatisticResponse
{
    public List<TopicAnalysisResponse> Analysises { get; set; }
    public double Over80Ratio { get; set; }
    public double Over90Ratio { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class TopicAnalysisResponse
{
    public string AnalysisTopicId { get; set; } // the other topics
    public string EnglishName { get; set; }
    public double Similarity { get; set; }
    public string ProcessedBy { get; set; }
}
