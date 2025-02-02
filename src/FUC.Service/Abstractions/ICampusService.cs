using FUC.Common.Shared;
using FUC.Data.Entities;
using FUC.Service.DTOs.CampusDTO;

namespace FUC.Service.Abstractions;

public interface ICampusService
{
    Task<OperationResult<string>> CreateCampusAsync(CreateCampusRequest request);
    Task<OperationResult<CampusResponse>> UpdateCampusAsync(UpdateCampusRequest request);
    Task<OperationResult<IEnumerable<CampusResponse>>> GetAllCampusAsync();
    
    Task<OperationResult<IEnumerable<CampusResponse>>> GetAllActiveCampusAsync();
    Task<OperationResult<CampusResponse>> GetCampusByIdAsync(string campusId);
    Task<OperationResult> DeleteCampusAsync(string campusId);
}
