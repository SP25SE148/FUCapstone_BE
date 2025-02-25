namespace FUC.Common.Payloads;

public class SemanticResponse
{
    public Dictionary<string, MatchingTopic> MatchingTopics { get; set; }
}

public class MatchingTopic
{
    public double Similarity { get; set; }
    public string EnglishName { get; set; }
}
