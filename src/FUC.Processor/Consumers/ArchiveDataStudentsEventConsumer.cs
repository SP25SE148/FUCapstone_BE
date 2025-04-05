using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.Options;
using FUC.Processor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FUC.Processor.Consumers;

public class ArchiveDataStudentsEventConsumer : BaseEventConsumer<ArchiveDataStudentsEvent>
{
    private readonly ProcessorDbContext _processorDbContext;
    private readonly ILogger<ArchiveDataStudentsEventConsumer> _logger;

    public ArchiveDataStudentsEventConsumer(ILogger<ArchiveDataStudentsEventConsumer> logger, 
        ProcessorDbContext processorDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _processorDbContext = processorDbContext;
        _logger = logger;
    }

    protected override async Task ProcessMessage(ArchiveDataStudentsEvent message)
    {
        _logger.LogInformation("Starting to consume the event {EventType}.", nameof(ArchiveDataStudentsEvent));

        await _processorDbContext.Users
            .Where(x => message.StudentsCode.Contains(x.UserCode))
            .ExecuteDeleteAsync();

        _logger.LogInformation("Done to delete students data.");
    }
}
