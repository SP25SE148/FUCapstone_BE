using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.Options;
using FUC.Data.Entities;
using FUC.Processor.Abstractions;
using FUC.Processor.Data;
using Microsoft.Extensions.Options;

namespace FUC.Processor.Consumers;

public class SemanticProcessingForCreatedTopicEventConsumer : BaseEventConsumer<SemanticProcessingForCreatedTopicEvent>
{
    private readonly ILogger<SemanticProcessingForCreatedTopicEventConsumer> _logger;   
    private readonly ISemanticApi _semanticApi;
    private readonly FucDbContext _dbContext;

    public SemanticProcessingForCreatedTopicEventConsumer(ILogger<SemanticProcessingForCreatedTopicEventConsumer> logger,
        ISemanticApi semanticApi,
        FucDbContext fucDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _semanticApi = semanticApi;
        _dbContext = fucDbContext;  
    }

    protected override async Task ProcessMessage(SemanticProcessingForCreatedTopicEvent message)
    {
        _logger.LogInformation("--> Consume semantic for Topic {TopicId} - Event {EventId}", message.TopicId ,message.Id);

        var response = await _semanticApi.GetSemanticStatistic(message.TopicId);

        if (!response.IsSuccessStatusCode)
        {
            throw new ProcessMessageException("The semantic processing fail.");
        }

        var analysisResult = await response.Content.ReadAsStringAsync();

        _dbContext.TopicAnalyses.Add(new TopicAnalysis
        {
            AnalysisResult = analysisResult,
            TopicId = message.Id,
        });
        
        await _dbContext.SaveChangesAsync();
    }
}
