using FUC.Common.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FUC.Common.Abstractions;

public abstract class BaseEventConsumer<TIntegrationEvent> : IConsumer<TIntegrationEvent> 
    where TIntegrationEvent : IntegrationEvent
{
    private readonly ILogger<BaseEventConsumer<TIntegrationEvent>> _logger;
    
    protected abstract Task ProcessMessage(TIntegrationEvent message);

    protected string ConsumerName => GetType().Name;
    
    private const int MaxRetryCount = 3;

    public sealed class ProcessMessageException(string message) : Exception(message) { }

    protected BaseEventConsumer(ILogger<BaseEventConsumer<TIntegrationEvent>> logger)
    {
        _logger = logger;
    }


    public async Task Consume(ConsumeContext<TIntegrationEvent> context)
    {
        _logger.LogInformation("{Consumer} is starting.", ConsumerName);

        int retryCount = context.Message.RetryCount;
        
        try
        {
            if (retryCount > MaxRetryCount)
            {
                _logger.LogWarning("{Consumer}: Message failed after {RetryCount} retries. Unhandling message.", ConsumerName,

                    retryCount);

                return;
            }

            await ProcessMessage(context.Message);

            _logger.LogInformation("{Consumer}: Message processed successfully", ConsumerName);
        }
        catch (Exception ex) 
        {
            _logger.LogWarning("{Consumer}: Message failed. Retrying {RetryCount}/{MaxRetryCount}. {Message}", ConsumerName,

                    retryCount, MaxRetryCount, ex.Message);

            retryCount++;

            var endpoint = await context.GetSendEndpoint(new Uri(context.DestinationAddress!.AbsoluteUri));

            context.Message.RetryCount = retryCount;

            await endpoint.Send(context.Message);
        }
    }
}
