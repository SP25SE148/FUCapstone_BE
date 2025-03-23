using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.Options;
using Microsoft.Extensions.Options;

namespace FUC.Processor.Consumers;

public class ProjectProgressCreatedEventConsumer : BaseEventConsumer<ProjectProgressCreatedEvent>
{
    private readonly ILogger<ProjectProgressCreatedEventConsumer> _logger;

    public ProjectProgressCreatedEventConsumer(ILogger<ProjectProgressCreatedEventConsumer> logger, 
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
    }

    protected override Task ProcessMessage(ProjectProgressCreatedEvent message)
    {
        _logger.LogInformation(message.Type);

        return Task.CompletedTask;  
    }
}
