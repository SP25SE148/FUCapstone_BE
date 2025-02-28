using FUC.Data.Entities;
using FUC.Service.DTOs.TopicAppraisalDTO;

namespace FUC.Service.Filters;

public sealed class SupervisorTopicAppraisalFilter(string? supervisorId) : ITopicAppraisalFilterStrategy
{
    public IQueryable<TopicAppraisal> ApplyFilter(IQueryable<TopicAppraisal> query, string userCode)
    {
        return query.Where(ta =>
            ta.SupervisorId!.Equals(supervisorId ?? userCode) &&
            ta.ManagerId == null);
    }
}
