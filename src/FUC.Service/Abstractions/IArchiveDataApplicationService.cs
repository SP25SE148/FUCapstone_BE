using FUC.Common.Shared;
using FUC.Service.DTOs.DocumentDTO;

namespace FUC.Service.Abstractions;

public interface IArchiveDataApplicationService
{
    Task<OperationResult<ExportCompletedStudents>> ArchiveDataCompletedStudents(CancellationToken cancellationToken);
}
