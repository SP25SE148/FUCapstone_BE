using AutoMapper;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.CampusDTO;

namespace FUC.Service.Services;

public sealed class CampusService(IUnitOfWork<FucDbContext> uow, IMapper mapper) : ICampusService
{
    private readonly IUnitOfWork<FucDbContext> _uow = uow ?? throw new ArgumentNullException(nameof(uow));

    private readonly IRepository<Campus> _campusRepository =
        uow.GetRepository<Campus>() ?? throw new ArgumentNullException(nameof(uow));

    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

    public async Task<OperationResult<string>> CreateCampusAsync(CreateCampusRequest request)
    {
        // check if campus code was exist
        Campus? result = await _campusRepository.GetAsync(c =>
                c.Id.Equals(request.Id),
            cancellationToken: default);
        if (result is not null)
            return OperationResult.Failure<string>(new Error("Error.DuplicateValue", "The campus id is duplicate!!"));
        // create new campus
        var campus = new Campus()
        {
            Id = request.Id,
            Name = request.Name,
            Address = request.Address,
            Email = request.Email,
            Phone = request.Phone
        };

        _campusRepository.Insert(campus);
        await _uow.SaveChangesAsync();
        return campus.Id;
    }

    public async Task<OperationResult<CampusResponse>> UpdateCampusAsync(UpdateCampusRequest request)
    {
        // check if campus was not exist
        Campus? campus = await _campusRepository.GetAsync(
            predicate: c => c.Id.Equals(request.Id),
            cancellationToken: default);
        if (campus is null) return OperationResult.Failure<CampusResponse>(Error.NullValue);

        // update campus field
        campus.Address = request.Address;
        campus.Email = request.Email;
        campus.Phone = request.Phone;
        campus.Name = request.Name;

        _campusRepository.Update(campus);
        await _uow.SaveChangesAsync();
        return _mapper.Map<CampusResponse>(campus);
    }

    public async Task<OperationResult<IEnumerable<CampusResponse>>> GetAllCampusAsync()
    {
        List<Campus> campusList = await _campusRepository.GetAllAsync();
        return OperationResult.Success(_mapper.Map<IEnumerable<CampusResponse>>(campusList));
    }

    public async Task<OperationResult<IEnumerable<CampusResponse>>> GetAllActiveCampusAsync()
    {
        IList<Campus> campusList = await _campusRepository.FindAsync(c => c.IsDeleted == false);
        return OperationResult.Success(_mapper.Map<IEnumerable<CampusResponse>>(campusList.ToList()));
    }

    public async Task<OperationResult<CampusResponse>> GetCampusByIdAsync(string campusId)
    {
        Campus? campus = await _campusRepository.GetAsync(
            predicate: c => c.Id.Equals(campusId),
            cancellationToken: default);
        return _mapper.Map<CampusResponse>(campus) ?? OperationResult.Failure<CampusResponse>(Error.NullValue);
    }

    public async Task<OperationResult> DeleteCampusAsync(string campusId)
    {
        Campus? campus = await _campusRepository.GetAsync(
            predicate: c => c.Id.Equals(campusId),
            cancellationToken: default);
        if (campus is null) return OperationResult.Failure<Campus>(Error.NullValue);
        campus.IsDeleted = true;
        campus.DeletedAt = DateTime.Now;
        _campusRepository.Update(campus);
        await _uow.SaveChangesAsync();
        return OperationResult.Success();
    }
}
