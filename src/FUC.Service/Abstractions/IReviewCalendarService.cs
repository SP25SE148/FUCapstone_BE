using FUC.Common.Shared;
using FUC.Service.DTOs.ReviewCalendarDTO;
using Microsoft.AspNetCore.Http;

namespace FUC.Service.Abstractions;

public interface IReviewCalendarService
{
    Task<OperationResult> ImportReviewCalendar(IFormFile file);
    Task<OperationResult<IEnumerable<ReviewCalendarResponse>>> GetReviewCalendars();
    Task<OperationResult<IEnumerable<ReviewCalendarResponse>>> GetReviewCalendarBySupervisorId();
    Task<OperationResult<IEnumerable<ReviewCalendarResponse>>> GetReviewCalendarByStudentId();
    Task<OperationResult<IEnumerable<ReviewCalendarResponse>>> GetReviewCalendarByManagerId();
    Task<OperationResult> UpdateReviewCalendar(UpdateReviewerSuggestionAndCommentRequest request);

    Task<OperationResult<IEnumerable<ReviewCalendarResultResponse>>>
        GetReviewCalendarResultByStudentId(); // use for student

    Task<OperationResult<IEnumerable<ReviewCalendarResultResponse>>>
        GetReviewCalendarResultByReviewerId(); // use for reviewer

    Task<OperationResult<IEnumerable<ReviewCalendarResultResponse>>>
        GetReviewCalendarResultByGroupId(Guid groupId); // use for supervisor 

    Task<OperationResult<IEnumerable<ReviewCalendarResultResponse>>>
        GetReviewCalendarResultByManagerId(); // use for manager

    Task<OperationResult<IEnumerable<ReviewCriteriaResponse>>> GetReviewCriteriaByAttemptAsync(int attempt);
}
