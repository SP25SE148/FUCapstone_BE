using AutoMapper;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.SupervisorDTO;
using Microsoft.EntityFrameworkCore;

namespace FUC.Service.Services;

public sealed class SupervisorService(IMapper mapper, IUnitOfWork<FucDbContext> uow) : ISupervisorService
{
    private readonly IUnitOfWork<FucDbContext> _uow = uow ?? throw new ArgumentNullException(nameof(uow));

    private readonly IRepository<Supervisor> _SupervisorRepository = uow.GetRepository<Supervisor>() ?? throw new ArgumentNullException(nameof(uow));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<OperationResult<IEnumerable<SupervisorResponseDTO>>> GetAllSupervisorAsync()
    {
        List<Supervisor> supervisors = await _SupervisorRepository.GetAllAsync(
            s => s
                .Include(s => s.Major)
                .Include(s => s.Campus));
        return supervisors.Count > 0
            ? OperationResult.Success(_mapper.Map<IEnumerable<SupervisorResponseDTO>>(supervisors))
            : OperationResult.Failure<IEnumerable<SupervisorResponseDTO>>(Error.NullValue); 
    }
}
