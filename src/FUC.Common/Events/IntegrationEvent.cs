using System.Text.Json.Serialization;

namespace FUC.Common.Events;

public abstract class IntegrationEvent
{
    protected IntegrationEvent()
    {
        Id = Guid.NewGuid();
        RetryCount = 0;
        CreationDate = DateTime.Now;
    }

    [JsonInclude]
    public Guid Id { get; set; }

    [JsonInclude]
    public int RetryCount { get; set; }

    [JsonInclude]
    public DateTime CreationDate { get; set; }
}
