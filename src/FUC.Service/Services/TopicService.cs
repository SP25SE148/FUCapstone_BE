using Amazon.S3;
using FUC.Common.Abstractions;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Enums;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.BusinessAreaDTO;
using FUC.Service.DTOs.TopicDTO;
using FUC.Service.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FUC.Service.Services;

public class TopicService(ILogger<TopicService> logger,
    IS3Service s3Service,
    ICurrentUser currentUser,
    IUnitOfWork<FucDbContext> unitOfWork,
    IRepository<Topic> topicRepository,
    IRepository<Supervisor> supervisorRepository,
    IRepository<Capstone> capstoneRepository,
    IRepository<BusinessArea> bussinessRepository,
    S3BucketConfiguration s3BucketConfiguration,
    ISemesterService semesterService) : ITopicService
{
    public async Task<OperationResult<PaginatedList<TopicResponse>>> GetTopics(TopicRequest request)
    {
        var topics = await topicRepository.FindPaginatedAsync(
            x => x.Code == request.Code &&
            x.MainSupervisor.Email == request.MainSupervisorEmail &&
            (string.IsNullOrEmpty(request.SearchTerm) ||
            x.EnglishName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
            x.VietnameseName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
            x.Abbreviation.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
            x.Description.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)) &&
            (request.BusinessAreaName == "all" || x.BusinessArea.Name.Equals(request.BusinessAreaName,
                StringComparison.OrdinalIgnoreCase)) &&
            (request.DifficultyLevel == "all" || x.DifficultyLevel.ToString().Equals(request.DifficultyLevel,
                StringComparison.OrdinalIgnoreCase)) &&
            (request.Status == "all" || x.Status.ToString().Equals(request.Status,
                StringComparison.OrdinalIgnoreCase)) &&
            x.CapstoneId == request.CapstoneId &&
            x.CampusId == request.CampusId,
            request.PageNumber,
            request.PageSize,
            x => x.OrderBy(x => x.CreatedDate),
            x => x.Include(x => x.MainSupervisor)
                .Include(x => x.BusinessArea),
            x => new TopicResponse
            {
                Code = request.Code,
                MainSupervisorEmail = request.MainSupervisorEmail,
                MainSupervisorName = x.MainSupervisor.FullName,
                EnglishName = x.EnglishName,
                VietnameseName = x.VietnameseName,
                Abbreviation = x.Abbreviation,
                Description = x.Description,
                FileName = x.FileName,
                FileUrl = x.FileUrl,
                Status = x.Status,
                DifficultyLevel = x.DifficultyLevel,
                BusinessAreaName = x.BusinessArea.Name,
                CampusId = x.CampusId,
                SemesterId = x.SemesterId,
                CapstoneId = x.CapstoneId
            });

        return OperationResult.Success(topics);
    }

    public async Task<OperationResult<Guid>> CreateTopic(CreateTopicRequest request, CancellationToken cancellationToken)
    {
        var mainSupervisor = await supervisorRepository.GetAsync(s => s.Email == currentUser.Email, cancellationToken);

        if (mainSupervisor == null)
        {
            logger.LogError("Create Topic fail with error: {SupervisorId} does not exist", currentUser.Id);
            return OperationResult.Failure<Guid>(new Error("Topic.Error", "Supervisor does not exist."));
        }

        if (!await bussinessRepository.AnyAsync(b => b.Id == request.BusinessAreaId, cancellationToken))
        {
            logger.LogError("Create Topic fail with error: {BusinessArea} does not exist", request.BusinessAreaId);
            return OperationResult.Failure<Guid>(new Error("Topic.Error", "Supervisor does not exist."));
        }

        if (!await capstoneRepository.AnyAsync(c => c.Id == request.CapstoneId, cancellationToken))
        {
            logger.LogError("Create Topic fail with error: {Capstone} does not exist", request.CapstoneId);
            return OperationResult.Failure<Guid>(new Error("Topic.Error", "Supervisor does not exist."));
        }

        var getCurrentSemesterResult = await semesterService.GetCurrentSemesterAsync();

        if (getCurrentSemesterResult.IsFailure)
        {
            return OperationResult.Failure<Guid>(getCurrentSemesterResult.Error);
        }

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var topicId = Guid.NewGuid();
            var key = $"{currentUser.CampusId}/{getCurrentSemesterResult.Value.Id}/{request.CapstoneId}/Pending/{topicId}";

            var topic = new Topic
            {
                Id = topicId,
                MainSupervisorId = mainSupervisor.Id,
                EnglishName = request.EnglishName,
                VietnameseName = request.VietnameseName,
                Abbreviation = request.Abbreviation,
                BusinessAreaId = request.BusinessAreaId,
                CampusId = currentUser.CampusId,
                CapstoneId = request.CapstoneId,
                SemesterId = getCurrentSemesterResult.Value.Id,
                Description = request.Description,
                FileName = request.File.FileName,
                FileUrl = key,
                DifficultyLevel = request.DifficultyLevel,
                Status = TopicStatus.Pending
            };

            topicRepository.Insert(topic);

            await unitOfWork.SaveChangesAsync(cancellationToken);


            if (!await SaveToS3(request.File, key, cancellationToken))
            {
                throw new AmazonS3Exception("Failed to save to S3, database changes rolled back.");
            }

            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success(topic.Id);
        }
        catch (Exception)
        {
            logger.LogError("Fail to create topic");
            await unitOfWork.RollbackAsync(cancellationToken);

            throw;
        }
    }

    private async Task<bool> SaveToS3(IFormFile file, string key, CancellationToken cancellationToken)
    {
        using var stream = file.OpenReadStream();

        var s3Response = await s3Service.SaveToS3(s3BucketConfiguration.FUCTopicBucket, key, file.ContentType, stream, cancellationToken);

        return s3Response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    public async Task<OperationResult<List<BusinessAreaResponse>>> GetAllBusinessAreas()
    {
        var queryable = from ba in bussinessRepository.GetQueryable()
                        select new BusinessAreaResponse
                        {
                            Id = ba.Id,
                            Description = ba.Description,
                            Name = ba.Name
                        };
        var businessAreas = await queryable.ToListAsync();

        if (businessAreas.Count != 0)
        {
            return OperationResult.Success(businessAreas);
        }

        return OperationResult.Failure<List<BusinessAreaResponse>>(Error.NullValue);
    }
}
