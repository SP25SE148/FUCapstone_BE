using FUC.Common.Constants;
using FUC.Service.DTOs.TopicAppraisalDTO;

namespace FUC.Service.Filters;

public class TopicAppraisalFilterFactory
{
    public ITopicAppraisalFilterStrategy GetStrategy(TopicAppraisalBaseRequest request, string role)
    {
        return role switch
        {
            UserRoles.Manager when request is ManagerTopicAppraisalRequest managerTopicAppraisalRequest =>
                new SupervisorTopicAppraisalFilter(managerTopicAppraisalRequest.SupervisorId),
            UserRoles.Manager => new ManagerTopicAppraisalFilter(),
            UserRoles.Supervisor => new SupervisorTopicAppraisalFilter(null),
            _ => throw new NotSupportedException($"Role {role} is not supported!")
        };
    }
}
