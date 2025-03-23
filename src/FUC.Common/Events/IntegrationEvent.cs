using System.Text.Json.Serialization;
using MassTransit;

namespace FUC.Common.Events;

[ExcludeFromTopology]
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
