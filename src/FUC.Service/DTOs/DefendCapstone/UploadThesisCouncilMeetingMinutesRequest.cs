using Microsoft.AspNetCore.Http;

namespace FUC.Service.DTOs.DefendCapstone;

public class UploadThesisCouncilMeetingMinutesRequest
{
    public required IFormFile File { get; set; }
    public Guid DefendCapstoneCalendarId { get; set; }
}
