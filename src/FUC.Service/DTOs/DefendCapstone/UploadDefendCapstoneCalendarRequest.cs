using Microsoft.AspNetCore.Http;

namespace FUC.Service.DTOs.DefendCapstone;

public sealed record UploadDefendCapstoneCalendarRequest(IFormFile File, string semesterId);
