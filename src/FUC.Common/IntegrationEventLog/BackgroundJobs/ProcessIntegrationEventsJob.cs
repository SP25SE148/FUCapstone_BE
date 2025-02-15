using System.Collections.Concurrent;
using System.Text.Json;
using FUC.Common.Options;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;
using Quartz;

namespace FUC.Common.IntegrationEventLog.BackgroundJobs;

[DisallowConcurrentExecution]
public class ProcessIntegrationEventsJob<TDbContext> : IJob where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;
    private const int BatchSize = 1000;
    private static readonly ConcurrentDictionary<string, Type> TypeCache = new();
    private readonly ILogger<ProcessIntegrationEventsJob<TDbContext>> _logger;  

    public ProcessIntegrationEventsJob(TDbContext dbContext, 
        IPublishEndpoint publishEndpoint, 
        ILogger<ProcessIntegrationEventsJob<TDbContext>> logger)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("--> Starting execute the Integration Event for {Database}.", typeof(TDbContext).Name);

        await _dbContext.Database.BeginTransactionAsync(context.CancellationToken);

        var events = await _dbContext.Database.SqlQueryRaw<IntegrationEventLog>(
            $"""
            SELECT *
            FROM "IntegrationEventLogs"
            WHERE "ProcessedOnUtc" IS NULL
            ORDER BY "OccurredOnUtc" LIMIT @BatchSize
            FOR UPDATE SKIP LOCKED
            """
            , new NpgsqlParameter("@BatchSize", NpgsqlDbType.Integer) { Value = BatchSize })
            .ToListAsync(context.CancellationToken);

        var updateQueue = new ConcurrentQueue<EventUpdate>();

        var publishTasks = events
            .Select(e => PublishEvent(e, updateQueue, _publishEndpoint, context.CancellationToken))
            .ToList();

        await Task.WhenAll(publishTasks);

        if (!updateQueue.IsEmpty)
        {
            var updateSql = """
                UPDATE "IntegrationEventLogs" AS t
                SET "ProcessedOnUtc" = v.processed_on_utc,
                    "Error" = v.error
                FROM (VALUES {0}) AS v(id, processed_on_utc, error)
                WHERE t."Id" = v.id::uuid
                """;

            var updates = updateQueue.ToList();
            var valuesList = string.Join(",",
                updateQueue.Select((_, i) => $"(@Id{i}::uuid, @ProcessedOn{i}, @Error{i})"));

            var parameters = new List<NpgsqlParameter>();

            for (int i = 0; i < updateQueue.Count; i++)
            {
                parameters.Add(new NpgsqlParameter($"@Id{i}", NpgsqlDbType.Uuid) { Value = updates[i].Id });
                parameters.Add(new NpgsqlParameter($"@ProcessedOn{i}", NpgsqlDbType.TimestampTz) { Value = updates[i].ProcessedOnUtc });

                // Fix: Specify `NpgsqlDbType.Text` for `Error` and handle NULL values properly.
                parameters.Add(new NpgsqlParameter($"@Error{i}", NpgsqlDbType.Text) { Value = updates[i].Error ?? (object)DBNull.Value });
            }
            var formattedSql = string.Format(updateSql, valuesList);

            await _dbContext.Database.ExecuteSqlRawAsync(formattedSql, parameters);

            _logger.LogInformation("Updated status of {Count} events", updateQueue.Count);
        }

        await _dbContext.Database.CommitTransactionAsync(context.CancellationToken);

        _logger.LogInformation("--> Ending execute the Integration Event for {Database}.", typeof(TDbContext).Name);
    }

    private async Task PublishEvent(
        IntegrationEventLog @event,
        ConcurrentQueue<EventUpdate> updateQueue,
        IPublishEndpoint publishEndpoint,
        CancellationToken cancellationToken)
    {
        try
        {
            var messageType = GetOrAddMessageType(@event.Type);
            var deserializedMessage = JsonSerializer.Deserialize(@event.Content, messageType)!;

            await publishEndpoint.Publish(deserializedMessage, cancellationToken);

            updateQueue.Enqueue(new EventUpdate { Id = @event.Id, ProcessedOnUtc = DateTime.UtcNow });

            _logger.LogInformation("Event {EventId} send successfully.", @event.Id);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Event {EventId} was processed fail. {Error}", @event.Id, ex.Message);

            updateQueue.Enqueue(
                new EventUpdate { Id = @event.Id, ProcessedOnUtc = DateTime.UtcNow, Error = ex.ToString() });
        }
    }

    private static Type GetOrAddMessageType(string typename)
    {
        return TypeCache.GetOrAdd(typename, name => FUC.Common.AssemblyReference.Assembly.GetType(name)!);
    }

    private struct EventUpdate
    {
        public Guid Id { get; init; }
        public DateTime ProcessedOnUtc { get; init; }
        public string? Error { get; init; }
    }
}
