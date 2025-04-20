using AutoMapper;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.CapstoneDTO;

namespace FUC.Service.Services;

public sealed class CapstoneService(
    IUnitOfWork<FucDbContext> uow,
    IRepository<Capstone> capstoneRepository,
    IMapper mapper) : ICapstoneService
{
    public async Task<OperationResult<string>> CreateCapstoneAsync(CreateCapstoneRequest request)
    {
        // Check if Capstone code exists
        Capstone? existingCapstone = await capstoneRepository.GetAsync(c =>
                c.Id == request.Id,
            cancellationToken: default);

        if (existingCapstone is not null)
            return OperationResult.Failure<string>(new Error("Error.DuplicateValue", "The capstone id is duplicate!"));

        // Create new Capstone
        var capstone = new Capstone
        {
            Id = request.Id,
            MajorId = request.MajorId,
            Name = request.Name,
            MinMember = request.MinMember,
            MaxMember = request.MaxMember,
            ReviewCount = request.ReviewCount,
            DurationWeeks = request.DurationWeeks
        };

        capstoneRepository.Insert(capstone);
        await uow.SaveChangesAsync();
        return capstone.Id;
    }

    public async Task<OperationResult<CapstoneResponse>> UpdateCapstoneAsync(UpdateCapstoneRequest request)
    {
        // Check if Capstone exists
        Capstone? capstone = await capstoneRepository.GetAsync(
            predicate: c => c.Id == request.Id,
            cancellationToken: default);

        if (capstone is null) return OperationResult.Failure<CapstoneResponse>(Error.NullValue);

        // Update Capstone fields
        capstone.MajorId = request.MajorId;
        capstone.Name = request.Name;
        capstone.MinMember = request.MinMember;
        capstone.MaxMember = request.MaxMember;
        capstone.ReviewCount = request.ReviewCount;

        capstoneRepository.Update(capstone);

        await uow.SaveChangesAsync();

        return mapper.Map<CapstoneResponse>(capstone);
    }

    public async Task<OperationResult<IEnumerable<CapstoneResponse>>> GetAllCapstonesAsync()
    {
        List<Capstone> capstones = await capstoneRepository.GetAllAsync();

        return OperationResult.Success(mapper.Map<IEnumerable<CapstoneResponse>>(capstones));
    }

    public async Task<OperationResult<IEnumerable<CapstoneResponse>>> GetCapstonesByMajorIdAsync(string majorId)
    {
        var capstones = await capstoneRepository.FindAsync(
            c => c.MajorId == majorId);

        return capstones.Count > 0
            ? OperationResult.Success(mapper.Map<IEnumerable<CapstoneResponse>>(capstones.ToList()))
            : OperationResult.Failure<IEnumerable<CapstoneResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<CapstoneResponse>>> GetAllActiveCapstonesAsync()
    {
        IList<Capstone> capstones = await capstoneRepository.FindAsync(c => !c.IsDeleted);

        return OperationResult.Success(mapper.Map<IEnumerable<CapstoneResponse>>(capstones));
    }

    public async Task<OperationResult<CapstoneResponse>> GetCapstoneByIdAsync(string capstoneId)
    {
        Capstone? capstone = await capstoneRepository.GetAsync(
            predicate: c => c.Id == capstoneId,
            cancellationToken: default);

        return capstone is not null
            ? OperationResult.Success(mapper.Map<CapstoneResponse>(capstone))
            : OperationResult.Failure<CapstoneResponse>(Error.NullValue);
    }

    public async Task<OperationResult> DeleteCapstoneAsync(string capstoneId)
    {
        Capstone? capstone = await capstoneRepository.GetAsync(
            predicate: c => c.Id == capstoneId,
            cancellationToken: default);

        if (capstone is null) return OperationResult.Failure<Capstone>(Error.NullValue);

        capstone.IsDeleted = true;
        capstone.DeletedAt = DateTime.Now;

        capstoneRepository.Update(capstone);

        await uow.SaveChangesAsync();

        return OperationResult.Success();
    }

    public async Task<OperationResult<int>> GetMaxMemberByCapstoneId(string capstoneId)
    {
        var capstone = await capstoneRepository
            .GetAsync(c => c.Id == capstoneId,
                default);

        return capstone?.MaxMember ?? OperationResult.Failure<int>(Error.NullValue);
    }
}
