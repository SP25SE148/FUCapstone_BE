using FUC.Common.Shared;
using FUC.Service.DTOs.ConfigDTO;
using FUC.Service.DTOs.DocumentDTO;

namespace FUC.Service.Abstractions;

public interface IArchiveDataApplicationService
{
    Task<OperationResult<ExportCompletedStudents>> ArchiveDataCompletedStudents(CancellationToken cancellationToken);
    Task<OperationResult<SuperAdminDashBoardDto>> PresentSuperAdminDashBoard(CancellationToken cancellationToken);
    Task<OperationResult<AdminDashBoardDto>> PresentAdminDashBoard(CancellationToken cancellationToken);
    Task<OperationResult<ManagerDashBoardDto>> PresentManagerDashBoard(CancellationToken cancellationToken);
}
