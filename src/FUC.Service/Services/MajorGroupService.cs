using AutoMapper;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.MajorGroupDTO;

namespace FUC.Service.Services;

public sealed class MajorGroupService(
    IUnitOfWork<FucDbContext> uow, 
    IRepository<MajorGroup> majorGroupRepository,
    IMapper mapper) : IMajorGroupService
{
    public async Task<OperationResult<string>> CreateMajorGroupAsync(CreateMajorGroupRequest request)
    {
        MajorGroup? existingMajorGroup = await majorGroupRepository.GetAsync(
            mg => mg.Id == request.Id,
            cancellationToken: default);
        if (existingMajorGroup is not null)
            return OperationResult.Failure<string>(new Error("Error.DuplicateValue",
                "The MajorGroup Id already exists."));

        var newMajorGroup = new MajorGroup
        {
            Id = request.Id,
            Name = request.Name,
            Description = request.Description
        };

        majorGroupRepository.Insert(newMajorGroup);
        await uow.SaveChangesAsync();

        return newMajorGroup.Id;
    }

    public async Task<OperationResult<MajorGroupResponse>> UpdateMajorGroupAsync(UpdateMajorGroupRequest request)
    {
        MajorGroup? majorGroup = await majorGroupRepository.GetAsync(
            mg => mg.Id == request.Id,
            cancellationToken: default);
        if (majorGroup is null) return OperationResult.Failure<MajorGroupResponse>(Error.NullValue);

        // Update major group fields
        majorGroup.Name = request.Name;
        majorGroup.Description = request.Description;

        majorGroupRepository.Update(majorGroup);
        await uow.SaveChangesAsync();

        return OperationResult.Success(mapper.Map<MajorGroupResponse>(majorGroup));
    }

    public async Task<OperationResult<IEnumerable<MajorGroupResponse>>> GetAllMajorGroupsAsync()
    {
        List<MajorGroup> majorGroups = await majorGroupRepository.GetAllAsync();
        return OperationResult.Success(mapper.Map<IEnumerable<MajorGroupResponse>>(majorGroups));
    }

    public async Task<OperationResult<IEnumerable<MajorGroupResponse>>> GetAllActiveMajorGroupsAsync()
    {
        IList<MajorGroup> majorGroups = await majorGroupRepository.FindAsync(
            m => !m.IsDeleted);

        return OperationResult.Success(mapper.Map<IEnumerable<MajorGroupResponse>>(majorGroups));
    }

    public async Task<OperationResult<MajorGroupResponse>> GetMajorGroupByIdAsync(string majorGroupId)
    {
        MajorGroup? majorGroup = await majorGroupRepository.GetAsync(
            mg => mg.Id == majorGroupId,
            cancellationToken: default);

        return majorGroup is not null
            ? OperationResult.Success(mapper.Map<MajorGroupResponse>(majorGroup))
            : OperationResult.Failure<MajorGroupResponse>(Error.NullValue);
    }

    public async Task<OperationResult> DeleteMajorGroupAsync(string majorGroupId)
    {
        MajorGroup? majorGroup = await majorGroupRepository.GetAsync(
            mg => mg.Id == majorGroupId,
            cancellationToken: default);

        if (majorGroup is null) return OperationResult.Failure(Error.NullValue);

        majorGroup.IsDeleted = true;
        majorGroup.DeletedAt = DateTime.Now;
        majorGroupRepository.Update(majorGroup);
        await uow.SaveChangesAsync();

        return OperationResult.Success();
    }
}
