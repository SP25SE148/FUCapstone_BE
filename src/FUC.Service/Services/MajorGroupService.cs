using AutoMapper;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.MajorGroupDTO;

namespace FUC.Service.Services;
public sealed class MajorGroupService(IUnitOfWork<FucDbContext> uow, IMapper mapper) : IMajorGroupService
{
    private readonly IRepository<MajorGroup> _majorGroupRepository = uow.GetRepository<MajorGroup>() ?? throw new ArgumentNullException(nameof(uow));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<OperationResult<Guid>> CreateMajorGroupAsync(CreateMajorGroupRequest request)
    {
        MajorGroup? existingMajorGroup = await _majorGroupRepository.GetAsync(
            mg => mg.Code.ToLower() == request.Code.ToLower(),
            cancellationToken: default);
        if (existingMajorGroup is not null)
            return OperationResult.Failure<Guid>(new Error("Error.DuplicateValue", "The MajorGroup code already exists."));
        
        var newMajorGroup = new MajorGroup
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Code = request.Code,
            Description = request.Description
        };

        _majorGroupRepository.Insert(newMajorGroup);
        await uow.SaveChangesAsync();

        return OperationResult.Success(newMajorGroup.Id);
    }

    public async Task<OperationResult<MajorGroupResponse>> UpdateMajorGroupAsync(UpdateMajorGroupRequest request)
    {
        MajorGroup? majorGroup = await _majorGroupRepository.GetAsync(
            mg => mg.Id == request.Id,
            cancellationToken: default);
        if (majorGroup is null) return OperationResult.Failure<MajorGroupResponse>(Error.NullValue);

        // Update major group fields
        majorGroup.Name = request.Name;
        majorGroup.Code = request.Code;
        majorGroup.Description = request.Description;

        _majorGroupRepository.Update(majorGroup);
        await uow.SaveChangesAsync();

        return OperationResult.Success(_mapper.Map<MajorGroupResponse>(majorGroup));
    }

    public async Task<OperationResult<IEnumerable<MajorGroupResponse>>> GetAllMajorGroupsAsync()
    {
        List<MajorGroup> majorGroups = await _majorGroupRepository.GetAllAsync();
        return majorGroups.Count != 0
            ? OperationResult.Success(_mapper.Map<IEnumerable<MajorGroupResponse>>(majorGroups))
            : OperationResult.Failure<IEnumerable<MajorGroupResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<MajorGroupResponse>>> GetAllActiveMajorGroupsAsync()
    {
        IList<MajorGroup> majorGroups = await _majorGroupRepository.FindAsync(
            m => m.IsDeleted == false);
        return majorGroups.Count != 0
            ? OperationResult.Success(_mapper.Map<IEnumerable<MajorGroupResponse>>(majorGroups))
            : OperationResult.Failure<IEnumerable<MajorGroupResponse>>(Error.NullValue);

    }

    public async Task<OperationResult<MajorGroupResponse>> GetMajorGroupByIdAsync(Guid majorGroupId)
    {
        MajorGroup? majorGroup = await _majorGroupRepository.GetAsync(
            mg => mg.Id == majorGroupId,
            cancellationToken: default);
        return majorGroup is not null
            ? OperationResult.Success(_mapper.Map<MajorGroupResponse>(majorGroup))
            : OperationResult.Failure<MajorGroupResponse>(Error.NullValue);
    }

    public async Task<OperationResult> DeleteMajorGroupAsync(Guid majorGroupId)
    {
        MajorGroup? majorGroup = await _majorGroupRepository.GetAsync(
            mg => mg.Id == majorGroupId,
            cancellationToken: default);
        if (majorGroup is null) return OperationResult.Failure(Error.NullValue);

        majorGroup.IsDeleted = true;
        majorGroup.DeletedAt = DateTime.UtcNow;
        _majorGroupRepository.Update(majorGroup);
        await uow.SaveChangesAsync();

        return OperationResult.Success();
    }
}
