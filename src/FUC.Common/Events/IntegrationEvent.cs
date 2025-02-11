using System.Text.Json.Serialization;

namespace FUC.Common.Events;

public record IntegrationEvent
{
    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        RetryCount = 0;
        CreationDate = DateTime.UtcNow;
    }

    [JsonInclude]
    public Guid Id { get; set; }

    [JsonInclude]
    public int RetryCount { get; set; }

    [JsonInclude]
    public DateTime CreationDate { get; set; }
}
