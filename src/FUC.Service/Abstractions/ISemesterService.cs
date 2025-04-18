using FUC.Common.Shared;
using FUC.Data.Entities;
using FUC.Service.DTOs.SemesterDTO;

namespace FUC.Service.Abstractions;

public interface ISemesterService
{
    Task<OperationResult<string>> CreateSemesterAsync(CreateSemesterRequest request);
    Task<OperationResult<SemesterResponse>> UpdateSemesterAsync(UpdateSemesterRequest request);
    Task<OperationResult<IEnumerable<SemesterResponse>>> GetSemestersAsync();
    Task<OperationResult<IEnumerable<SemesterResponse>>> GetAllActiveSemestersAsync();
    Task<OperationResult<SemesterResponse>> GetSemesterByIdAsync(string semesterId);
    Task<OperationResult> DeleteSemesterAsync(string semesterId);
    Task<OperationResult<Semester>> GetCurrentSemesterAsync();
    Task<List<string>> GetPreviouseSemesterIds(DateTime? startDayOfCurrentSemester = null, int top = 3);
}
