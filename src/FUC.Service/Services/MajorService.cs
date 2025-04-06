using AutoMapper;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.MajorDTO;

namespace FUC.Service.Services;

public sealed class MajorService(
    IUnitOfWork<FucDbContext> uow, 
    IRepository<Major> majorRepository,
    IMapper mapper) : IMajorService
{
    public async Task<OperationResult<string>> CreateMajorAsync(CreateMajorRequest request)
    {
        Major? existingMajor = await majorRepository.GetAsync(
            m => m.Id == request.Id,
            cancellationToken: default);

        if (existingMajor is not null)
            return OperationResult.Failure<string>(new Error("Error.DuplicateValue", "The Major Id already exists."));

        var newMajor = new Major()
        {
            Id = request.Id,
            MajorGroupId = request.MajorGroupId,
            Name = request.Name,
            Description = request.Description
        };

        majorRepository.Insert(newMajor);
        await uow.SaveChangesAsync();

        return newMajor.Id;
    }

    public async Task<OperationResult<MajorResponse>> UpdateMajorAsync(UpdateMajorRequest request)
    {
        Major? major = await majorRepository.GetAsync(
            m => m.Id == request.Id,
            cancellationToken: default);

        if (major is null)
            return OperationResult.Failure<MajorResponse>(Error.NullValue);

        // Update major fields
        major.MajorGroupId = request.MajorGroupId;
        major.Name = request.Name;
        major.Description = request.Description;

        majorRepository.Update(major);
        await uow.SaveChangesAsync();

        return mapper.Map<MajorResponse>(major);
    }

    public async Task<OperationResult<IEnumerable<MajorResponse>>> GetAllMajorsAsync()
    {
        List<Major> majors = await majorRepository.GetAllAsync();
        return OperationResult.Success(mapper.Map<IEnumerable<MajorResponse>>(majors));
    }

    public async Task<OperationResult<IEnumerable<MajorResponse>>> GetAllActiveMajorsAsync()
    {
        IList<Major> majors = await majorRepository.FindAsync(m => !m.IsDeleted);
        return OperationResult.Success(mapper.Map<IEnumerable<MajorResponse>>(majors));
    }

    public async Task<OperationResult<IEnumerable<MajorResponse>>> GetMajorsByMajorGroupIdAsync(string majorGroupId)
    {
        IList<Major> majors = await majorRepository.FindAsync(
            m => m.MajorGroupId == majorGroupId,
            cancellationToken: default);

        return OperationResult.Success(mapper.Map<IEnumerable<MajorResponse>>(majors));
    }

    public async Task<OperationResult<MajorResponse>> GetMajorByIdAsync(string majorId)
    {
        Major? major = await majorRepository.GetAsync(
            m => m.Id == majorId,
            cancellationToken: default);

        return major is not null
            ? mapper.Map<MajorResponse>(major)
            : OperationResult.Failure<MajorResponse>(Error.NullValue);
    }

    public async Task<OperationResult> DeleteMajorAsync(string majorId)
    {
        Major? major = await majorRepository.GetAsync(
            m => m.Id == majorId,
            cancellationToken: default);

        if (major is null)
            return OperationResult.Failure(Error.NullValue);

        major.IsDeleted = true;
        major.DeletedAt = DateTime.Now;
        majorRepository.Update(major);

        await uow.SaveChangesAsync();

        return OperationResult.Success();
    }
}
