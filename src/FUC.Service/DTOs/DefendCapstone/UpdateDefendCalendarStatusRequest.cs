using FUC.Data.Enums;

namespace FUC.Service.DTOs.DefendCapstone;

public record UpdateDefendCalendarStatusRequest(Guid Id, DefendCapstoneProjectCalendarStatus Status);
