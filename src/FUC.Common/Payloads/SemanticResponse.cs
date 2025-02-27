using System.Text.Json.Serialization;

namespace FUC.Common.Payloads;

public class SemanticResponse
{
    public Dictionary<string, MatchingTopic> MatchingTopics { get; set; }
}

public class MatchingTopic
{
    [JsonPropertyName("similarity")]
    public double Similarity { get; set; }

    [JsonPropertyName("english_name")]
    public string EnglishName { get; set; }

    [JsonPropertyName("processed_by")]
    public string ProcessedBy { get; set; }
}
