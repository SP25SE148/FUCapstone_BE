using System.Runtime.InteropServices.JavaScript;
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

        public async Task<OperationResult<Guid>> CreateMajorAsync(CreateMajorRequest request)
        {
            Major? existingMajor = await _majorRepository.GetAsync(
                m => m.Code.ToLower() == request.Code.ToLower(),
                cancellationToken: default);
            if (existingMajor is not null)
                return OperationResult.Failure<Guid>(new Error("Error.DuplicateValue", "The Major code already exists."));
            var newMajor = new Major()
            {
                Id = Guid.NewGuid(),
                MajorGroupId = request.MajorGroupId,
                Name = request.Name,
                Code = request.Code,
                Description = request.Description
            };

            _majorRepository.Insert(newMajor);
            await uow.SaveChangesAsync();

            return OperationResult.Success(newMajor.Id);
        }

        public async Task<OperationResult<MajorResponse>> UpdateMajorAsync(UpdateMajorRequest request)
        {
            Major? major = await _majorRepository.GetAsync(
                m => m.Id == request.Id,
                cancellationToken: default);
            if (major is null) return OperationResult.Failure<MajorResponse>(Error.NullValue);

            // Update major fields
            major.MajorGroupId = request.MajorGroupId;
            major.Name = request.Name;
            major.Code = request.Code;
            major.Description = request.Description;
            
            _majorRepository.Update(major);
            await uow.SaveChangesAsync();

            return OperationResult.Success(_mapper.Map<MajorResponse>(major));
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

        public async Task<OperationResult<IEnumerable<MajorResponse>>> GetMajorsByGroupIdAsync(Guid majorGroupId)
        {
            IList<Major> majors = await _majorRepository.FindAsync(
                m => m.MajorGroupId == majorGroupId,
                cancellationToken: default);
            return majors.Count != 0
                ? OperationResult.Success(_mapper.Map<IEnumerable<MajorResponse>>(majors))
                : OperationResult.Failure<IEnumerable<MajorResponse>>(Error.NullValue);
        }

        public async Task<OperationResult<MajorResponse>> GetMajorByIdAsync(Guid majorId)
        {
            Major? major = await _majorRepository.GetAsync(
                m => m.Id == majorId,
                cancellationToken: default);
            return major is not null
                ? OperationResult.Success(_mapper.Map<MajorResponse>(major))
                : OperationResult.Failure<MajorResponse>(Error.NullValue);
        }

        public async Task<OperationResult> DeleteMajorAsync(Guid majorId)
        {
            Major? major = await _majorRepository.GetAsync(
                m => m.Id == majorId,
                cancellationToken: default);
            if (major is null) return OperationResult.Failure(Error.NullValue);

            major.IsDeleted = true;
            major.DeletedAt = DateTime.UtcNow;
            _majorRepository.Update(major);
            await uow.SaveChangesAsync();

            return OperationResult.Success();
        }
    }
