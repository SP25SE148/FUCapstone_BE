using FUC.Common.Events;
using FUC.Common.Options;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FUC.Common.Abstractions;

public abstract class BaseEventConsumer<TIntegrationEvent> : IConsumer<TIntegrationEvent> 
    where TIntegrationEvent : IntegrationEvent
{
    private readonly ILogger<BaseEventConsumer<TIntegrationEvent>> _logger;
    
    protected abstract Task ProcessMessage(TIntegrationEvent message);

    protected string ConsumerName => GetType().Name;
    
    private readonly EventConsumerConfiguration _eventConsumerConfiguration;

    public sealed class ProcessMessageException(string message) : Exception(message) { }

    protected BaseEventConsumer(ILogger<BaseEventConsumer<TIntegrationEvent>> logger,
        IOptions<EventConsumerConfiguration> options)
    {
        _logger = logger;
        _eventConsumerConfiguration = options.Value;
    }


    public async Task Consume(ConsumeContext<TIntegrationEvent> context)
    {
        _logger.LogInformation("{Consumer} is starting with messageId: {Id}.", ConsumerName, context.Message.Id);

        int retryCount = context.Message.RetryCount;
        
        try
        {
            if (retryCount > _eventConsumerConfiguration.MaxRetryCount)
            {
                _logger.LogWarning("{Consumer}: Message failed after {RetryCount} retries. Unhandling message.", ConsumerName,

                    retryCount);

                return;
            }

            await ProcessMessage(context.Message);

            _logger.LogInformation("{Consumer}: MessageId: {Id} processed successfully", ConsumerName, context.Message.Id);
        }
        catch (Exception ex) 
        {
            _logger.LogWarning("{Consumer}: MessageId: {Id} failed. Retrying {RetryCount}/{MaxRetryCount}. {Message}", 
                ConsumerName, 
                context.Message.Id,
                retryCount, 
                _eventConsumerConfiguration.MaxRetryCount, 
                ex.Message);

            retryCount++;

            var endpoint = await context.GetSendEndpoint(new Uri(context.DestinationAddress!.AbsoluteUri));

            context.Message.RetryCount = retryCount;

            await Task.Delay(TimeSpan.FromSeconds(_eventConsumerConfiguration.DelayTime));

            await endpoint.Send(context.Message);
        }
    }
}
