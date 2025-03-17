using FUC.Data.Entities;
using FUC.Service.DTOs.ProjectProgressDTO;

namespace FUC.Service.Helpers;

public static class TrackingTaskHistory
{
    public static Dictionary<string, (object? OldValue, object? NewValue)> GetChangedProperties(UpdateTaskRequest request, FucTask existingTask)
    {
        var changes = new Dictionary<string, (object?, object?)>();

        if (request.KeyTask != null && request.KeyTask != existingTask.KeyTask)
            changes[nameof(request.KeyTask)] = (existingTask.KeyTask, request.KeyTask);

        if (request.Description != null && request.Description != existingTask.Description)
            changes[nameof(request.Description)] = (existingTask.Description, request.Description);

        if (request.Summary != null && request.Summary != existingTask.Summary)
            changes[nameof(request.Summary)] = (existingTask.Summary, request.Summary);

        if (request.AssigneeId != null && request.AssigneeId != existingTask.AssigneeId?.ToString())
            changes[nameof(request.AssigneeId)] = (existingTask.AssigneeId, request.AssigneeId);

        if (request.Status.HasValue && request.Status != existingTask.Status)
            changes[nameof(request.Status)] = (existingTask.Status, request.Status);

        if (request.Priority.HasValue && request.Priority != existingTask.Priority)
            changes[nameof(request.Priority)] = (existingTask.Priority, request.Priority);

        if (!string.IsNullOrEmpty(request.Comment))
            changes[nameof(request.Comment)] = ("", request.Comment);

        if (request.DueDate.HasValue && request.DueDate != existingTask.DueDate)
            changes[nameof(request.DueDate)] = (existingTask.DueDate, request.DueDate);

        return changes;
    }
}
