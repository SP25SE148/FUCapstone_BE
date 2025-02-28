using FUC.Data.Entities;
using FUC.Service.DTOs.TopicAppraisalDTO;

namespace FUC.Service.Filters;

public interface ITopicAppraisalFilterStrategy
{
    IQueryable<TopicAppraisal> ApplyFilter(IQueryable<TopicAppraisal> query, string userCode);
}
