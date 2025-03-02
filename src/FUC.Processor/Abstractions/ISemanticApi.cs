using System.Text.Json.Serialization;
using Refit;

namespace FUC.Processor.Abstractions;

public interface ISemanticApi
{
    [Get("/semantic/{campusId}/{capstoneId}/{semesterId}/{id}")]
    Task<HttpResponseMessage> GetSemanticStatisticWithCurrentSemester(string campusId, string capstoneId, string semesterId, string id);

    [Post("/previous/semantic")]
    [Headers("Content-Type: application/json")]
    Task<HttpResponseMessage> GetSemanticStatisticWithPreviousSemesters([Body] SemanticPreviousSemesterRequest request);
}

public class SemanticPreviousSemesterRequest
{
    [JsonPropertyName("topic_id")]
    public string TopicId { get; set; }

    [JsonPropertyName("semester_ids")]
    public List<string> SemesterIds { get; set; }

    [JsonPropertyName("campus_id")]
    public string CampusId { get; set; }

    [JsonPropertyName("capstone_id")]
    public string CapstoneId { get; set; }
}
