using FUC.Common.Shared;
using FUC.Service.DTOs.DefendCapstone;
using FUC.Service.DTOs.GroupDTO;
using Microsoft.AspNetCore.Http;

namespace FUC.Service.Abstractions;

public interface IDefendCapstoneService
{
    Task<OperationResult> UploadDefendCapstoneProjectCalendar(IFormFile file,
        CancellationToken cancellationToken);

    Task<OperationResult<List<(DateTime, List<DefendCapstoneCalendarResponse>)>>>
        GetDefendCalendersByCouncilMember(CancellationToken cancellationToken);

    Task<OperationResult> UploadThesisCouncilMeetingMinutesForDefendCapstone(
        UploadThesisCouncilMeetingMinutesRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<string>> PresentThesisForTopicResignedUrl(Guid calendarId, CancellationToken cancellationToken);

    Task<OperationResult> UpdateStatusOfGroupAfterDefend(UpdateGroupDecisionStatusByPresidentRequest request,
        CancellationToken cancellationToken);
}
