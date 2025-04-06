using AutoMapper;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.CampusDTO;

namespace FUC.Service.Services;

public sealed class CampusService(IUnitOfWork<FucDbContext> uow, 
    IRepository<Campus> campusRepository,
    IMapper mapper) : ICampusService
{
    public async Task<OperationResult<string>> CreateCampusAsync(CreateCampusRequest request)
    {
        // check if campus code was exist
        Campus? result = await campusRepository.GetAsync(c =>
                c.Id == request.Id,
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

        campusRepository.Insert(campus);
        await uow.SaveChangesAsync();
        return campus.Id;
    }

    public async Task<OperationResult<CampusResponse>> UpdateCampusAsync(UpdateCampusRequest request)
    {
        // check if campus was not exist
        Campus? campus = await campusRepository.GetAsync(
            predicate: c => c.Id == request.Id,
            cancellationToken: default);
        if (campus is null) return OperationResult.Failure<CampusResponse>(Error.NullValue);

        // update campus field
        campus.Address = request.Address;
        campus.Email = request.Email;
        campus.Phone = request.Phone;
        campus.Name = request.Name;

        campusRepository.Update(campus);
        await uow.SaveChangesAsync();
        return mapper.Map<CampusResponse>(campus);
    }

    public async Task<OperationResult<IEnumerable<CampusResponse>>> GetAllCampusAsync()
    {
        List<Campus> campusList = await campusRepository.GetAllAsync();

        return OperationResult.Success(mapper.Map<IEnumerable<CampusResponse>>(campusList));
    }

    public async Task<OperationResult<IEnumerable<CampusResponse>>> GetAllActiveCampusAsync()
    {
        IList<Campus> campusList = await campusRepository.FindAsync(c => !c.IsDeleted);

        return OperationResult.Success(mapper.Map<IEnumerable<CampusResponse>>(campusList.ToList()));
    }

    public async Task<OperationResult<CampusResponse>> GetCampusByIdAsync(string campusId)
    {
        Campus? campus = await campusRepository.GetAsync(
            predicate: c => c.Id == campusId,
            cancellationToken: default);

        return mapper.Map<CampusResponse>(campus) ?? OperationResult.Failure<CampusResponse>(Error.NullValue);
    }

    public async Task<OperationResult> DeleteCampusAsync(string campusId)
    {
        Campus? campus = await campusRepository.GetAsync(
            predicate: c => c.Id == campusId,
            cancellationToken: default);

        if (campus is null) return OperationResult.Failure<Campus>(Error.NullValue);

        campus.IsDeleted = true;
        campus.DeletedAt = DateTime.Now;
        
        campusRepository.Update(campus);

        await uow.SaveChangesAsync();
        return OperationResult.Success();
    }
}
