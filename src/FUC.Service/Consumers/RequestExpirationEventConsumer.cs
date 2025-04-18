﻿using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.Options;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FUC.Service.Consumers;

public class TopicRequestExpirationEventConsumer : BaseEventConsumer<TopicRequestExpirationEvent>
{
    private readonly ILogger<TopicRequestExpirationEventConsumer> _logger;
    private readonly FucDbContext _fucDbContext;

    public TopicRequestExpirationEventConsumer(ILogger<TopicRequestExpirationEventConsumer> logger,
        FucDbContext fucDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _fucDbContext = fucDbContext;
        _fucDbContext.DisableInterceptors = true;
    }

    protected override async Task ProcessMessage(TopicRequestExpirationEvent message)
    {
        try
        {
            _logger.LogInformation("Starting change status of TopicRequest {Id}",
                message.TopicRequestId);

            var topicRequest = await _fucDbContext.Set<TopicRequest>().FirstOrDefaultAsync(
                x => x.Id == message.TopicRequestId, default);

            ArgumentNullException.ThrowIfNull(topicRequest);

            topicRequest.Status = Data.Enums.TopicRequestStatus.Rejected;
            topicRequest.UpdatedDate = DateTime.Now;
            topicRequest.UpdatedBy = "System";

            _fucDbContext.Update(topicRequest);

            await _fucDbContext.SaveChangesAsync();

            _logger.LogInformation("End change status of TopicRequest {Id}",
                message.TopicRequestId);
        }
        catch (Exception ex)
        {
            _logger.LogError("Expiration TopicRequest {Id} fail with error {Message}", message.TopicRequestId,
                ex.Message);

            throw;
        }
    }
}

public class JoinGroupRequestExpirationEventConsumer : BaseEventConsumer<JoinGroupRequestExpirationEvent>
{
    private readonly ILogger<JoinGroupRequestExpirationEventConsumer> _logger;
    private readonly FucDbContext _fucDbContext;

    public JoinGroupRequestExpirationEventConsumer(ILogger<JoinGroupRequestExpirationEventConsumer> logger,
        FucDbContext fucDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _fucDbContext = fucDbContext;
        _fucDbContext.DisableInterceptors = true;
    }

    protected override async Task ProcessMessage(JoinGroupRequestExpirationEvent message)
    {
        try
        {
            _logger.LogInformation("Starting change status of JoinGroupRequest {Id}",
                message.JoinGroupRequestId);

            var request = await _fucDbContext.Set<JoinGroupRequest>().FirstOrDefaultAsync(
                x => x.Id == message.JoinGroupRequestId, default);

            ArgumentNullException.ThrowIfNull(request);

            request.Status = Data.Enums.JoinGroupRequestStatus.Rejected;
            request.UpdatedDate = DateTime.Now;
            request.UpdatedBy = "System";

            _fucDbContext.Update(request);

            await _fucDbContext.SaveChangesAsync();

            _logger.LogInformation("End change status of JoinGroupRequest {Id}",
                message.JoinGroupRequestId);
        }
        catch (Exception ex)
        {
            _logger.LogError("Expiration JoinGroupRequest {Id} fail with error {Message}", message.JoinGroupRequestId,
                ex.Message);

            throw;
        }
    }
}

public class GroupMemberExpirationEventConsumer : BaseEventConsumer<GroupMemberExpirationEvent>
{
    private readonly ILogger<GroupMemberExpirationEventConsumer> _logger;
    private readonly FucDbContext _fucDbContext;

    public GroupMemberExpirationEventConsumer(ILogger<GroupMemberExpirationEventConsumer> logger,
        FucDbContext fucDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _fucDbContext = fucDbContext;
        _fucDbContext.DisableInterceptors = true;
    }

    protected override async Task ProcessMessage(GroupMemberExpirationEvent message)
    {
        try
        {
            _logger.LogInformation("Starting change status of GroupMember {Id}",
                message.GroupMemberId);

            var request = await _fucDbContext.Set<GroupMember>().FirstOrDefaultAsync(
                x => x.Id == message.GroupMemberId);

            ArgumentNullException.ThrowIfNull(request);

            if (request.Status == Data.Enums.GroupMemberStatus.UnderReview)
            {
                request.Status = Data.Enums.GroupMemberStatus.Rejected;
                request.UpdatedDate = DateTime.Now;
                request.UpdatedBy = "System";

                _fucDbContext.Update(request);

                await _fucDbContext.SaveChangesAsync();

                _logger.LogInformation("End change status of GroupMember {Id}",
                    message.GroupMemberId);
            }
            else
            {
                await Task.CompletedTask;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Expiration GroupMember {Id} fail with error {Message}", message.GroupMemberId,
                ex.Message);

            throw;
        }
    }
}

public class ReviewCalendarExpirationEventConsumer : BaseEventConsumer<ReviewCalendarExpirationEvent>
{
    private readonly ILogger<ReviewCalendarExpirationEventConsumer> _logger;
    private readonly FucDbContext _fucDbContext;

    public ReviewCalendarExpirationEventConsumer(
        ILogger<ReviewCalendarExpirationEventConsumer> logger,
        FucDbContext fucDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _fucDbContext = fucDbContext;
        _fucDbContext.DisableInterceptors = true;
    }

    protected override async Task ProcessMessage(ReviewCalendarExpirationEvent message)
    {
        try
        {
            var reviewCalendar = _fucDbContext.Set<ReviewCalendar>().FirstOrDefault(
                x => x.Id == message.ReviewCalendarId);

            ArgumentNullException.ThrowIfNull(reviewCalendar);
            if (reviewCalendar.Status == ReviewCalendarStatus.InProgress)
            {
                reviewCalendar.Status = ReviewCalendarStatus.Done;
                _fucDbContext.Update(reviewCalendar);
                await _fucDbContext.SaveChangesAsync();
                _logger.LogInformation("End change status of GroupMember {Id}",
                    message.ReviewCalendarId);
            }
            else
            {
                await Task.CompletedTask;
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Expiration GroupMember {Id} fail with error {Message}", message.ReviewCalendarId,
                e.Message);

            throw;
        }
    }
}
