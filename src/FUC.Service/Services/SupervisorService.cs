using AutoMapper;
using FUC.Common.Abstractions;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.SupervisorDTO;
using Microsoft.EntityFrameworkCore;

namespace FUC.Service.Services;

public sealed class SupervisorService(
    IMapper mapper,
    ICurrentUser currentUser,
    IRepository<Supervisor> supervisorRepository,
    IUnitOfWork<FucDbContext> uow) : ISupervisorService
{
    public async Task<OperationResult<IEnumerable<SupervisorResponseDTO>>> GetAllSupervisorAsync(
        CancellationToken cancellationToken)
    {
        var supervisors = await supervisorRepository.FindAsync(
            x => (currentUser.CampusId == "all" || x.CampusId == currentUser.CampusId) &&
                 (currentUser.MajorId == "all" || x.MajorId == currentUser.MajorId),
            s => s
                .Include(s => s.Major)
                .Include(s => s.Campus),
            null,
            cancellationToken);

        return supervisors.Count > 0
            ? OperationResult.Success(mapper.Map<IEnumerable<SupervisorResponseDTO>>(supervisors))
            : OperationResult.Failure<IEnumerable<SupervisorResponseDTO>>(Error.NullValue);
    }

    public async Task<OperationResult<SupervisorResponseDTO>> GetSupervisorByIdAsync(string id)
    {
        var supervisor = await supervisorRepository.GetAsync(
            s => s.Id == id,
            s => s.Include(s => s.Topics),
            default);
        return supervisor is not null
            ? OperationResult.Success(mapper.Map<SupervisorResponseDTO>(supervisor))
            : OperationResult.Failure<SupervisorResponseDTO>(Error.NullValue);
    }
}
