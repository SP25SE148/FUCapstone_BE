using FUC.Data.Enums;

namespace FUC.Service.DTOs.ReviewCalendarDTO;

public record UpdateReviewCalendarStatusRequest(Guid Id, ReviewCalendarStatus Status);
