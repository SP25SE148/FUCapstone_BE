using FUC.Data.Entities;

namespace FUC.Service.Filters;

public sealed class ManagerTopicAppraisalFilter : ITopicAppraisalFilterStrategy
{
    public IQueryable<TopicAppraisal> ApplyFilter(IQueryable<TopicAppraisal> query, string userCode)
    {
        return query.Where(ta =>
            ta.SupervisorId == null &&
            ta.ManagerId!.Equals(userCode));
    }
}
