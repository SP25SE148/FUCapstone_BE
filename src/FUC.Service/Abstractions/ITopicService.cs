using FUC.Common.Shared;
using FUC.Service.DTOs.BusinessAreaDTO;
using FUC.Service.DTOs.TopicAppraisalDTO;
using FUC.Service.DTOs.TopicDTO;

namespace FUC.Service.Abstractions;

public interface ITopicService
{
    Task<OperationResult<TopicResponse>> GetTopicById(Guid topicId, CancellationToken cancellationToken);
    Task<OperationResult<IList<TopicResponse>>> GetTopicsBySupervisor();
    Task<OperationResult<PaginatedList<TopicResponse>>> GetTopics(TopicRequest request);
    Task<OperationResult<Guid>> CreateTopic(CreateTopicRequest request, CancellationToken cancellationToken);
    Task<OperationResult<List<BusinessAreaResponse>>> GetAllBusinessAreas();

    Task<OperationResult<List<TopicStatisticResponse>>> GetTopicAnalysises(Guid topicId,
        CancellationToken cancellationToken);

    Task<OperationResult> SemanticTopic(Guid topicId, bool withCurrentSemester, CancellationToken cancellationToken);
    Task<OperationResult<string>> PresentTopicPresignedUrl(Guid topicId, CancellationToken cancellationToken);
    Task<OperationResult> CreateTopicAppraisal(IReadOnlyList<string> supervisorEmail);
    Task<OperationResult<List<TopicAppraisalResponse>>> GetTopicAppraisalByUserId(TopicAppraisalBaseRequest request);
}
