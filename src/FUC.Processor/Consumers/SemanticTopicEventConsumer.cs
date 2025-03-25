using System.Transactions;
using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.Options;
using FUC.Data.Entities;
using FUC.Processor.Abstractions;
using FUC.Processor.Data;
using FUC.Processor.Hubs;
using FUC.Processor.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace FUC.Processor.Consumers;

public class SemanticTopicEventConsumer : BaseEventConsumer<SemanticTopicEvent>
{
    private readonly ILogger<SemanticTopicEventConsumer> _logger;
    private readonly ISemanticApi _semanticApi;
    private readonly FucDbContext _dbContext;
    private readonly ICacheService _cacheService;
    private readonly UsersTracker _usersTracker;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;
    private readonly ProcessorDbContext _processorContext;


    public SemanticTopicEventConsumer(ILogger<SemanticTopicEventConsumer> logger,
        ISemanticApi semanticApi,
        FucDbContext fucDbContext,
        ICacheService cacheService,
        UsersTracker usersTracker,
        IHubContext<NotificationHub, INotificationClient> hub,
        ProcessorDbContext processorDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _semanticApi = semanticApi;
        _dbContext = fucDbContext;
        _cacheService = cacheService;
        _usersTracker = usersTracker;
        _hub = hub;
        _processorContext = processorDbContext;
    }

    protected override async Task ProcessMessage(SemanticTopicEvent message)
    {
        _logger.LogInformation("--> Consume semantic for Topic {TopicId} - Event {EventId}", message.TopicId,
            message.Id);

        var key = $"processing/{message.TopicId}";

        await _cacheService.SetAsync<object>(key, true, default);

        try
        {
            var response = message.IsCurrentSemester
                ? await _semanticApi.GetSemanticStatisticWithCurrentSemester(message.CampusId, message.CapstoneId, message.SemesterIds[0], message.TopicId)
                : await _semanticApi.GetSemanticStatisticWithPreviousSemesters(new SemanticPreviousSemesterRequest
                {
                    SemesterIds = message.SemesterIds,
                    TopicId = message.TopicId,
                    CapstoneId = message.CapstoneId,
                    CampusId = message.CampusId,
                });

            var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            if (!response.IsSuccessStatusCode)
            {
                _processorContext.Add(new Notification
                {
                    UserCode = message.ProcessedBy,
                    Content = $"Your semantic progress for {message.TopicEnglishName} was failed.",
                    IsRead = false,
                    ReferenceTarget = message.TopicId,
                    Type = nameof(SemanticTopicEvent)
                });
            }
            else
            {
                _processorContext.Add(new Notification
                {
                    UserCode = message.ProcessedBy,
                    Content = $"Your semantic progress for {message.TopicEnglishName} was successfully.",
                    IsRead = false,
                    ReferenceTarget = message.TopicId,
                    Type = nameof(SemanticTopicEvent)
                });
            }
            await _processorContext.SaveChangesAsync();

            var analysisResult = await response.Content.ReadAsStringAsync();

            _dbContext.TopicAnalysis.Add(new TopicAnalysis
            {
                AnalysisResult = analysisResult,
                TopicId = Guid.Parse(message.TopicId),
                ProcessedBy = message.ProcessedBy
            });
            await _dbContext.SaveChangesAsync();

            var connections = await _usersTracker.GetConnectionForUser(message.ProcessedBy);

            await _hub.Clients.Clients(connections).ReceiveNewNotification("New result of semantic for topic.");

            scope.Complete();
            scope.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError("Fail to process semantic for topicId: {Id} with error {Message}.", message.TopicId, ex.Message);
            throw;
        }
        finally
        {
            await _cacheService.RemoveAsync(key, default);
        }
    }
}
