﻿using FUC.Common.Events;
namespace FUC.Common.IntegrationEventLog;

public class IntegrationEventLog
{
    public Guid Id { get; init; }
    public required string Type { get; init; }
    public required string Content { get; init; }
    public DateTime OccurredOnUtc { get; init; }
    public DateTime? ProcessedOnUtc { get; init; }
    public string? Error { get; init; }
}
