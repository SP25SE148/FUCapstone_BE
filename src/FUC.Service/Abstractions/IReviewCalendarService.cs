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
    Task<OperationResult> GetReviewCriteria();
    Task<OperationResult> UpdateReviewCalendar(UpdateReviewerSuggestionAndCommentRequest request);

    Task<OperationResult<IEnumerable<ReviewCalendarResultResponse>>> GetReviewCalendarResultByStudentId();
    Task<OperationResult<IEnumerable<ReviewCalendarResultResponse>>> GetReviewCalendarResultByReviewerId();

    Task<OperationResult<IEnumerable<ReviewCalendarResultResponse>>>
        GetReviewCalendarResultByGroupId(Guid groupId); // user for supervisor 

    Task<OperationResult<IEnumerable<ReviewCalendarResultResponse>>> GetReviewCalendarResultByManagerId();
}
