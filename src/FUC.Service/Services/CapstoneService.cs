using AutoMapper;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.CapstoneDTO;

namespace FUC.Service.Services;

public sealed class CapstoneService(IUnitOfWork<FucDbContext> uow, IMapper mapper) : ICapstoneService
{
    private readonly IUnitOfWork<FucDbContext> _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    private readonly IRepository<Capstone> _capstoneRepository = uow.GetRepository<Capstone>() ?? throw new ArgumentNullException(nameof(uow));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<OperationResult<Guid>> CreateCapstoneAsync(CreateCapstoneRequest request)
    {
        // Check if Capstone code exists
        Capstone? existingCapstone = await _capstoneRepository.GetAsync(c =>
            c.Code.ToLower().Trim() == request.Code.ToLower().Trim(), cancellationToken: default);
        if (existingCapstone is not null)
            return OperationResult.Failure<Guid>(new Error("Error.DuplicateValue", "The capstone code is duplicate!"));

        // Create new Capstone
        var capstone = new Capstone
        {
            Id = Guid.NewGuid(),
            MajorId = request.MajorId,
            Code = request.Code,
            Name = request.Name,
            MinMember = request.MinMember,
            MaxMember = request.MaxMember
        };

        _capstoneRepository.Insert(capstone);
        await _uow.SaveChangesAsync();
        return capstone.Id;
    }

    public async Task<OperationResult<CapstoneResponse>> UpdateCapstoneAsync(UpdateCapstoneRequest request)
    {
        // Check if Capstone exists
        Capstone? capstone = await _capstoneRepository.GetAsync(
            predicate: c => c.Id == request.Id, cancellationToken: default);
        if (capstone is null) return OperationResult.Failure<CapstoneResponse>(Error.NullValue);

        // Update Capstone fields
        capstone.MajorId = request.MajorId;
        capstone.Code = request.Code;
        capstone.Name = request.Name;
        capstone.MinMember = request.MinMember;
        capstone.MaxMember = request.MaxMember;
        capstone.ReviewCount = request.ReviewCount;

        _capstoneRepository.Update(capstone);
        await _uow.SaveChangesAsync();
        return _mapper.Map<CapstoneResponse>(capstone);
    }

    public async Task<OperationResult<IEnumerable<CapstoneResponse>>> GetAllCapstonesAsync()
    {
        List<Capstone> capstones = await _capstoneRepository.GetAllAsync();
        return capstones.Count > 0
            ? OperationResult.Success(_mapper.Map<IEnumerable<CapstoneResponse>>(capstones))
            : OperationResult.Failure<IEnumerable<CapstoneResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<CapstoneResponse>>> GetAllActiveCapstonesAsync()
    {
        IList<Capstone> capstones = await _capstoneRepository.FindAsync(c => c.IsDeleted == false);
        return capstones.Count > 0
            ? OperationResult.Success(_mapper.Map<IEnumerable<CapstoneResponse>>(capstones))
            : OperationResult.Failure<IEnumerable<CapstoneResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<CapstoneResponse>> GetCapstoneByIdAsync(Guid capstoneId)
    {
        Capstone? capstone = await _capstoneRepository.GetAsync(
            predicate: c => c.Id.Equals(capstoneId), cancellationToken: default);
        return capstone is not null
            ? OperationResult.Success(_mapper.Map<CapstoneResponse>(capstone))
            : OperationResult.Failure<CapstoneResponse>(Error.NullValue);
    }

    public async Task<OperationResult> DeleteCapstoneAsync(Guid capstoneId)
    {
        Capstone? capstone = await _capstoneRepository.GetAsync(
            predicate: c => c.Id.Equals(capstoneId), cancellationToken: default);
        if (capstone is null) return OperationResult.Failure<Capstone>(Error.NullValue);

        capstone.IsDeleted = true;
        capstone.DeletedAt = DateTime.UtcNow;
        _capstoneRepository.Update(capstone);
        await _uow.SaveChangesAsync();
        return OperationResult.Success();
    }
}
