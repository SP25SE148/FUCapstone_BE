using FUC.Common.Shared;
using FUC.Data.Entities;
using FUC.Service.DTOs.BusinessAreaDTO;
using FUC.Service.DTOs.TopicAppraisalDTO;
using FUC.Service.DTOs.TopicDTO;

namespace FUC.Service.Abstractions;

public interface ITopicService
{
    Task<OperationResult<TopicResponse>> GetTopicById(Guid topicId, CancellationToken cancellationToken);
    Task<OperationResult<IList<TopicResponse>>> GetTopicsBySupervisor();
    Task<OperationResult<PaginatedList<TopicResponse>>> GetTopics(TopicParams request);
    Task<OperationResult<PaginatedList<TopicResponse>>> GetAvailableTopicsForGroupAsync(TopicForGroupParams request);
    Task<OperationResult<IList<TopicResponse>>> GetTopicsByManagerLevel();
    Task<OperationResult<Guid>> CreateTopic(CreateTopicRequest request, CancellationToken cancellationToken);
    Task<OperationResult> UpdateTopic(UpdateTopicRequest request, CancellationToken cancellationToken);
    Task<OperationResult<List<BusinessAreaResponse>>> GetAllBusinessAreas();

    Task<OperationResult<List<TopicStatisticResponse>>> GetTopicAnalysises(Guid topicId,
        CancellationToken cancellationToken);

    Task<TopicResponse?> GetTopicByTopicCode(string? topicCode);
    Task<OperationResult> SemanticTopic(Guid topicId, bool withCurrentSemester, CancellationToken cancellationToken);
    Task<OperationResult<string>> PresentTopicPresignedUrl(Guid topicId, CancellationToken cancellationToken);
    Task<OperationResult> AssignTopicAppraisalForAvailableSupervisors(IReadOnlyList<string> supervisorEmail);
    Task<OperationResult> AssignSupervisorForAppraisalTopic(AssignSupervisorAppraisalTopicRequest request, CancellationToken cancellationToken);
    Task<OperationResult<List<TopicAppraisalResponse>>> GetTopicAppraisalByUserId(TopicAppraisalBaseRequest request);
    Task<OperationResult> AppraisalTopic(AppraisalTopicRequest request, CancellationToken cancellationToken);
    Task<OperationResult<Topic>> GetTopicEntityById(Guid topicId, CancellationToken cancellationToken);
    Task<OperationResult<Topic>> GetTopicByCode(string topicCode, CancellationToken cancellationToken);
}
