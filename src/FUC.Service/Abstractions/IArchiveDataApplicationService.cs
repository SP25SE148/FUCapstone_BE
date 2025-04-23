using FUC.Common.Shared;
using FUC.Service.DTOs.ConfigDTO;
using FUC.Service.DTOs.DocumentDTO;

namespace FUC.Service.Abstractions;

public interface IArchiveDataApplicationService
{
    Task<OperationResult<ExportCompletedStudents>> ArchiveDataCompletedStudents(CancellationToken cancellationToken);

    Task<OperationResult<SuperAdminDashBoardDto>> PresentSuperAdminDashBoard(string semesterId,
        CancellationToken cancellationToken);

    Task<OperationResult<AdminDashBoardDto>> PresentAdminDashBoard(string semesterId,
        CancellationToken cancellationToken);

    Task<OperationResult<ManagerDashBoardDto>> PresentManagerDashBoard(string semesterId,
        CancellationToken cancellationToken);
}
