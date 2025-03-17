using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.Options;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FUC.Service.Consumers;

public class TopicRequestExpirationEventConsumer : BaseEventConsumer<TopicRequestExpirationEvent>
{
    private readonly ILogger<TopicRequestExpirationEventConsumer> _logger;
    private readonly IRepository<TopicRequest> _topicRequestRepository;
    private readonly IUnitOfWork<FucDbContext> _unitOfWork;

    public TopicRequestExpirationEventConsumer(ILogger<TopicRequestExpirationEventConsumer> logger,
        IRepository<TopicRequest> topicRequestRepository,
        IUnitOfWork<FucDbContext> unitOfWork,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _topicRequestRepository = topicRequestRepository;
        _unitOfWork = unitOfWork;
    }

    protected override async Task ProcessMessage(TopicRequestExpirationEvent message)
    {
        try
        {
            _logger.LogInformation("Starting change status of TopicRequest {Id}",
            message.TopicRequestId);

            var topicRequest = await _topicRequestRepository.GetAsync(
                    x => x.Id == message.Id, default);

            ArgumentNullException.ThrowIfNull(topicRequest);

            topicRequest.Status = Data.Enums.TopicRequestStatus.Rejected;

            _topicRequestRepository.Update(topicRequest);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("End change status of TopicRequest {Id}",
            message.TopicRequestId);
        }
        catch (Exception ex) 
        {
            _logger.LogError("Expiration TopicRequest {Id} fail with error {Message}", message.TopicRequestId, ex.Message);

            throw;
        }
    }
}

public class JoinGroupRequestExpirationEventConsumer : BaseEventConsumer<JoinGroupRequestExpirationEvent>
{
    private readonly ILogger<JoinGroupRequestExpirationEventConsumer> _logger;
    private readonly IRepository<JoinGroupRequest> _joinGroupRequestRepository;
    private readonly IUnitOfWork<FucDbContext> _unitOfWork;

    public JoinGroupRequestExpirationEventConsumer(ILogger<JoinGroupRequestExpirationEventConsumer> logger,
        IRepository<JoinGroupRequest> joinGroupRequestRepository,
        IUnitOfWork<FucDbContext> unitOfWork,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _joinGroupRequestRepository = joinGroupRequestRepository;
        _unitOfWork = unitOfWork;
    }

    protected override async Task ProcessMessage(JoinGroupRequestExpirationEvent message)
    {
        try
        {
            _logger.LogInformation("Starting change status of JoinGroupRequest {Id}",
            message.JoinGroupRequestId);

            var request = await _joinGroupRequestRepository.GetAsync(
                    x => x.Id == message.Id, default);

            ArgumentNullException.ThrowIfNull(request);

            request.Status = Data.Enums.JoinGroupRequestStatus.Rejected;

            _joinGroupRequestRepository.Update(request);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("End change status of JoinGroupRequest {Id}",
            message.JoinGroupRequestId);
        }
        catch (Exception ex)
        {
            _logger.LogError("Expiration JoinGroupRequest {Id} fail with error {Message}", message.JoinGroupRequestId, ex.Message);

            throw;
        }
    }
}

public class GroupMemberExpirationEventConsumer : BaseEventConsumer<GroupMemberExpirationEvent>
{
    private readonly ILogger<GroupMemberExpirationEventConsumer> _logger;
    private readonly IRepository<GroupMember> _groupMemberRepository;
    private readonly IUnitOfWork<FucDbContext> _unitOfWork;

    public GroupMemberExpirationEventConsumer(ILogger<GroupMemberExpirationEventConsumer> logger,
        IRepository<GroupMember> groupMemberRepository,
        IUnitOfWork<FucDbContext> unitOfWork,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _groupMemberRepository = groupMemberRepository;
        _unitOfWork = unitOfWork;
    }

    protected override async Task ProcessMessage(GroupMemberExpirationEvent message)
    {
        try
        {
            _logger.LogInformation("Starting change status of GroupMember {Id}",
            message.GroupMemberId);

            var request = await _groupMemberRepository.GetAsync(
                    x => x.Id == message.Id, default);

            ArgumentNullException.ThrowIfNull(request);

            request.Status = Data.Enums.GroupMemberStatus.Rejected;

            _groupMemberRepository.Update(request);

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("End change status of GroupMember {Id}",
            message.GroupMemberId);
        }
        catch (Exception ex)
        {
            _logger.LogError("Expiration GroupMember {Id} fail with error {Message}", message.GroupMemberId, ex.Message);

            throw;
        }
    }
}

