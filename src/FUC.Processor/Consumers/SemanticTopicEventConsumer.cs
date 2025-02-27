using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.Options;
using FUC.Data.Entities;
using FUC.Processor.Abstractions;
using FUC.Processor.Data;
using Microsoft.Extensions.Options;

namespace FUC.Processor.Consumers;

public class SemanticTopicEventConsumer : BaseEventConsumer<SemanticTopicEvent>
{
    private readonly ILogger<SemanticTopicEventConsumer> _logger;
    private readonly ISemanticApi _semanticApi;
    private readonly FucDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public SemanticTopicEventConsumer(ILogger<SemanticTopicEventConsumer> logger,
        ISemanticApi semanticApi,
        FucDbContext fucDbContext,
        ICacheService cacheService,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _semanticApi = semanticApi;
        _dbContext = fucDbContext;
        _cacheService = cacheService;
    }

    protected override async Task ProcessMessage(SemanticTopicEvent message)
    {
        _logger.LogInformation("--> Consume semantic for Topic {TopicId} - Event {EventId}", message.TopicId, message.Id);

        var key = $"processing/{message.TopicId}";

        await _cacheService.SetAsync<object>(key, true, default);

        try
        {
            var response = message.IsCurrentSemester ?
                await _semanticApi.GetSemanticStatisticWithCurrentSemester(message.SemesterIds[0], message.TopicId)
            :   await _semanticApi.GetSemanticStatisticWithPreviousSemesters(new SemanticPreviousSemesterRequest
            {
                SemesterIds = message.SemesterIds,
                TopicId = message.TopicId
            });

            if (!response.IsSuccessStatusCode)
            {
                throw new ProcessMessageException("The semantic processing fail.");
            }

            var analysisResult = await response.Content.ReadAsStringAsync();

            _dbContext.TopicAnalyses.Add(new TopicAnalysis
            {
                AnalysisResult = analysisResult,
                TopicId = message.Id,
                ProcessedBy = message.ProcessedBy
            });

            await _dbContext.SaveChangesAsync();
        }
        finally
        {
            await _cacheService.RemoveAsync(key, default);
        }
    }
}
