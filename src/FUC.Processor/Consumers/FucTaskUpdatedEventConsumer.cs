using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.Options;
using FUC.Processor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FUC.Processor.Consumers;

public class FucTaskAssigneeUpdatedEventConsumer : BaseEventConsumer<FucTaskAssigneeUpdatedEvent>
{
    private readonly ILogger<FucTaskAssigneeUpdatedEventConsumer> _logger;
    private readonly ProcessorDbContext _processorDbContext;

    public FucTaskAssigneeUpdatedEventConsumer(ILogger<FucTaskAssigneeUpdatedEventConsumer> logger,
        ProcessorDbContext processorDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;   
        _processorDbContext = processorDbContext;
    }

    protected override async Task ProcessMessage(FucTaskAssigneeUpdatedEvent message)
    {
        _logger.LogInformation("Start processing {Type} event", message.ReminderType);

        var reminders = await _processorDbContext.Reminders
                .Where(x => x.ReminderType == nameof(FucTaskCreatedEvent) &&
                        x.Content!.StartsWith($"{message.ProjectProgressId}/{message.FucTaskId}"))
                .ToListAsync();

        if (reminders.Count == 0)
            await Task.CompletedTask;

        reminders.ForEach(x =>
        {
            x.RemindFor = message.NotificationFor;
        });

        _processorDbContext.UpdateRange(reminders);

        await _processorDbContext.SaveChangesAsync();
    }
}

public class FucTaskDueDateUpdatedEventConsumer : BaseEventConsumer<FucTaskDueDateUpdatedEvent>
{
    private readonly ILogger<FucTaskDueDateUpdatedEventConsumer> _logger;
    private readonly ProcessorDbContext _processorDbContext;

    public FucTaskDueDateUpdatedEventConsumer(ILogger<FucTaskDueDateUpdatedEventConsumer> logger,
        ProcessorDbContext processorDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _processorDbContext = processorDbContext;
    }

    protected override async Task ProcessMessage(FucTaskDueDateUpdatedEvent message)
    {
        _logger.LogInformation("Start processing {Type} event", message.ReminderType);

        var reminders = await _processorDbContext.Reminders
                .Where(x => x.ReminderType == nameof(FucTaskCreatedEvent) &&
                        x.Content!.StartsWith($"{message.ProjectProgressId}/{message.FucTaskId}"))
                .ToListAsync();

        if (reminders.Count == 0)
            await Task.CompletedTask;

        reminders.ForEach(x =>
        {
            x.RemindDate = x.RemindDate.Add(message.DueDateChangedTime);
        });

        _processorDbContext.UpdateRange(reminders);

        await _processorDbContext.SaveChangesAsync();
    }
}

public class FucTaskStatusDoneUpdatedEventConsumer : BaseEventConsumer<FucTaskStatusDoneUpdatedEvent>
{
    private readonly ILogger<FucTaskStatusDoneUpdatedEventConsumer> _logger;

    private readonly ProcessorDbContext _processorDbContext;

    public FucTaskStatusDoneUpdatedEventConsumer(ILogger<FucTaskStatusDoneUpdatedEventConsumer> logger,
        ProcessorDbContext processorDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _processorDbContext = processorDbContext;
    }

    protected override async Task ProcessMessage(FucTaskStatusDoneUpdatedEvent message)
    {
        _logger.LogInformation("Start processing {Type} event", message.ReminderType);

        var reminders = await _processorDbContext.Reminders
                .Where(x => x.ReminderType == nameof(FucTaskCreatedEvent) &&
                        x.Content!.StartsWith($"{message.ProjectProgressId}/{message.FucTaskId}"))
                .ToListAsync();

        if (reminders.Count == 0)
            await Task.CompletedTask;

        _processorDbContext.RemoveRange(reminders);

        await _processorDbContext.SaveChangesAsync();
    }
}
