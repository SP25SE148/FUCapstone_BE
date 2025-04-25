using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.Options;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Enums;
using FUC.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FUC.Service.Consumers;

public class CalendarOnTimeEventConsumer
    : BaseEventConsumer<CalendarOnTimeEvent>
{
    private readonly ILogger<CalendarOnTimeEventConsumer> _logger;
    private readonly IOptions<EventConsumerConfiguration> _options;
    private readonly FucDbContext _dbContext;

    public CalendarOnTimeEventConsumer(ILogger<CalendarOnTimeEventConsumer> logger,
        FucDbContext fucDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _dbContext = fucDbContext;
        _dbContext.DisableInterceptors = true;
    }

    protected override async Task ProcessMessage(CalendarOnTimeEvent message)
    {
        _logger.LogInformation("\"Starting consume CalendarOnTimeEventConsumer - Event {EventId}\"", message.Id);
        if (message.Type == nameof(ReviewCalendar))
        {
            var reviewCalendar = await _dbContext.ReviewCalendars.AsTracking()
                .FirstOrDefaultAsync(t => t.Id == message.CalendarId);
            if (reviewCalendar != null)
            {
                switch (reviewCalendar!.Status)
                {
                    case ReviewCalendarStatus.InProgress:
                        reviewCalendar.Status = ReviewCalendarStatus.Done;
                        break;
                    case ReviewCalendarStatus.Pending:
                        reviewCalendar.Status = ReviewCalendarStatus.InProgress;
                        break;
                }

                await _dbContext.SaveChangesAsync();
            }
        }
        else if (message.Type == nameof(DefendCapstoneProjectInformationCalendar))
        {
            var defendCalendar = await _dbContext.DefendCapstoneProjectInformationCalendars.AsTracking()
                .FirstOrDefaultAsync(t => t.Id == message.CalendarId);
            if (defendCalendar != null)
            {
                switch (defendCalendar!.Status)
                {
                    case DefendCapstoneProjectCalendarStatus.InProgress:
                        defendCalendar.Status = DefendCapstoneProjectCalendarStatus.Done;
                        break;
                    case DefendCapstoneProjectCalendarStatus.NotStarted:
                        defendCalendar.Status = DefendCapstoneProjectCalendarStatus.InProgress;
                        break;
                }

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
