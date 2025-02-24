﻿using FUC.Common.Shared;
using FUC.Service.DTOs.BusinessAreaDTO;
using FUC.Service.DTOs.TopicDTO;

namespace FUC.Service.Abstractions;

public interface ITopicService
{
    Task<OperationResult<PaginatedList<TopicResponse>>> GetTopics(TopicRequest request);
    Task<OperationResult<Guid>> CreateTopic(CreateTopicRequest request, CancellationToken cancellationToken);
    Task<OperationResult<List<BusinessAreaResponse>>> GetAllBusinessAreas();
    Task<OperationResult<List<TopicStatisticResponse>>> GetTopicAnalysises(Guid topicId, CancellationToken cancellationToken);
}
