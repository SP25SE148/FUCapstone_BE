using Refit;

namespace FUC.Processor.Abstractions;

public interface ISemanticApi
{
    [Get("/semantic/{id}")]
    Task<HttpResponseMessage> GetSemanticStatistic(string id);
}
