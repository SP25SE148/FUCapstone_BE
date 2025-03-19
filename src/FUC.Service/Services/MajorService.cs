using AutoMapper;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.MajorDTO;

namespace FUC.Service.Services;

public sealed class MajorService(IUnitOfWork<FucDbContext> uow, IMapper mapper) : IMajorService
{
    private readonly IRepository<Major> _majorRepository = uow.GetRepository<Major>() ?? throw new ArgumentNullException(nameof(uow));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<OperationResult<string>> CreateMajorAsync(CreateMajorRequest request)
    {
        Major? existingMajor = await _majorRepository.GetAsync(
            m => m.Id.Equals(request.Id),
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

        _majorRepository.Insert(newMajor);
        await uow.SaveChangesAsync();

        return newMajor.Id;
    }

    public async Task<OperationResult<MajorResponse>> UpdateMajorAsync(UpdateMajorRequest request)
    {
        Major? major = await _majorRepository.GetAsync(
            m => m.Id.Equals(request.Id),
            cancellationToken: default);
        if (major is null)
            return OperationResult.Failure<MajorResponse>(Error.NullValue);

        // Update major fields
        major.MajorGroupId = request.MajorGroupId;
        major.Name = request.Name;
        major.Description = request.Description;

        _majorRepository.Update(major);
        await uow.SaveChangesAsync();

        return _mapper.Map<MajorResponse>(major);
    }

    public async Task<OperationResult<IEnumerable<MajorResponse>>> GetAllMajorsAsync()
    {
        List<Major> majors = await _majorRepository.GetAllAsync();
        return majors.Count != 0
            ? OperationResult.Success(_mapper.Map<IEnumerable<MajorResponse>>(majors))
            : OperationResult.Failure<IEnumerable<MajorResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<MajorResponse>>> GetAllActiveMajorsAsync()
    {
        IList<Major> majors = await _majorRepository.FindAsync(m => m.IsDeleted == false);
        return majors.Count != 0
            ? OperationResult.Success(_mapper.Map<IEnumerable<MajorResponse>>(majors))
            : OperationResult.Failure<IEnumerable<MajorResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<MajorResponse>>> GetMajorsByMajorGroupIdAsync(string majorGroupId)
    {
        IList<Major> majors = await _majorRepository.FindAsync(
            m => m.MajorGroupId.Equals(majorGroupId),
            cancellationToken: default);
        return majors.Count != 0
            ? OperationResult.Success(_mapper.Map<IEnumerable<MajorResponse>>(majors))
            : OperationResult.Failure<IEnumerable<MajorResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<MajorResponse>> GetMajorByIdAsync(string majorId)
    {
        Major? major = await _majorRepository.GetAsync(
            m => m.Id.Equals(majorId),
            cancellationToken: default);
        return major is not null
            ? _mapper.Map<MajorResponse>(major)
            : OperationResult.Failure<MajorResponse>(Error.NullValue);
    }

    public async Task<OperationResult> DeleteMajorAsync(string majorId)
    {
        Major? major = await _majorRepository.GetAsync(
            m => m.Id.Equals(majorId),
            cancellationToken: default);
        if (major is null)
            return OperationResult.Failure(Error.NullValue);

        major.IsDeleted = true;
        major.DeletedAt = DateTime.Now;
        _majorRepository.Update(major);
        await uow.SaveChangesAsync();

        return OperationResult.Success();
    }
}
