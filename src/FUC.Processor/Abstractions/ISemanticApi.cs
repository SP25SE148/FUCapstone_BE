using System.Text.Json.Serialization;
using Refit;

namespace FUC.Processor.Abstractions;

public interface ISemanticApi
{
    [Get("/semantic/{semesterId}/{id}")]
    Task<HttpResponseMessage> GetSemanticStatisticWithCurrentSemester(string semesterId, string id);

    [Post("/semantic")]
    Task<HttpResponseMessage> GetSemanticStatisticWithPreviousSemesters([Body] SemanticPreviousSemesterRequest request);
}

public class SemanticPreviousSemesterRequest
{
    [JsonPropertyName("topic_id")]
    public string TopicId { get; set; }

    [JsonPropertyName("semester_ids")]
    public List<string> SemesterIds { get; set; }
}
