namespace FUC.Service.DTOs.ReviewCalendarDTO;

public record UpdateReviewerSuggestionAndCommentRequest(Guid ReviewCalenderId, string? Suggestion, string? Comment);
