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
}
