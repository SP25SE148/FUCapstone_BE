using System.Text.Json;
using Amazon.S3;
using AutoMapper;
using FUC.Common.Abstractions;
using FUC.Common.Constants;
using FUC.Common.Contracts;
using FUC.Common.IntegrationEventLog.Services;
using FUC.Common.Payloads;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Enums;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.BusinessAreaDTO;
using FUC.Service.DTOs.TopicAppraisalDTO;
using FUC.Service.DTOs.TopicDTO;
using FUC.Service.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FUC.Service.Services;

public class TopicService(
    ILogger<TopicService> logger,
    IMapper mapper,
    IS3Service s3Service,
    ICurrentUser currentUser,
    IUnitOfWork<FucDbContext> unitOfWork,
    IRepository<Topic> topicRepository,
    IRepository<TopicAppraisal> topicAppraisalRepository,
    IRepository<TopicAnalysis> topicAnalysisRepository,
    IRepository<Supervisor> supervisorRepository,
    IRepository<Capstone> capstoneRepository,
    IRepository<BusinessArea> businessRepository,
    IRepository<CoSupervisor> coSupervisorRepository,
    S3BucketConfiguration s3BucketConfiguration,
    ISemesterService semesterService,
    IRepository<GroupMember> groupMemberRepository,
    ICacheService cache,
    ITimeConfigurationService timeConfigurationService,
    ISystemConfigurationService systemConfigService,
    IIntegrationEventLogService integrationEventLogService) : ITopicService
{
    public async Task<OperationResult<Topic>> GetTopicEntityById(Guid topicId,
        bool isIncludeGroup = false,
        CancellationToken cancellationToken = default)
    {
        var topic = await topicRepository.GetAsync(
            t => t.Id == topicId,
            include: isIncludeGroup ? x => x.Include(x => x.Group) : null,
            orderBy: null,
            cancellationToken);

        return topic ?? OperationResult.Failure<Topic>(Error.NullValue);
    }

    public async Task<OperationResult<Topic>> GetTopicByCode(string topicCode, CancellationToken cancellationToken)
    {
        var topic = await topicRepository
            .GetAsync(t => t.Code == topicCode, cancellationToken);

        return topic ?? OperationResult.Failure<Topic>(Error.NullValue);
    }

    public async Task<OperationResult<TopicResponse>> GetTopicById(Guid topicId, CancellationToken cancellationToken)
    {
        var topic = await topicRepository.GetAsync(
            x => x.Id == topicId,
            isEnabledTracking: false,
            x => x.AsSplitQuery()
                .Include(x => x.MainSupervisor)
                .Include(x => x.BusinessArea)
                .Include(x => x.CoSupervisors)
                .ThenInclude(c => c.Supervisor),
            null,
            cancellationToken);

        return topic == null
            ? OperationResult.Failure<TopicResponse>(new Error("Topic.Error", "Topic does not exist."))
            : OperationResult.Success(new TopicResponse
            {
                Id = topic.Id.ToString(),
                Code = topic.Code ?? "undefined",
                MainSupervisorId = topic.MainSupervisorId,
                MainSupervisorEmail = topic.MainSupervisor.Email,
                MainSupervisorName = topic.MainSupervisor.FullName,
                EnglishName = topic.EnglishName,
                VietnameseName = topic.VietnameseName,
                Abbreviation = topic.Abbreviation,
                Description = topic.Description,
                FileName = topic.FileName,
                FileUrl = topic.FileUrl,
                Status = topic.Status.ToString(),
                DifficultyLevel = topic.DifficultyLevel.ToString(),
                BusinessAreaName = topic.BusinessArea.Name,
                CampusId = topic.CampusId,
                SemesterId = topic.SemesterId,
                CapstoneId = topic.CapstoneId,
                CoSupervisors = topic.CoSupervisors.Select(x => new CoSupervisorDto
                {
                    SupervisorCode = x.SupervisorId,
                    SupervisorEmail = x.Supervisor.Email,
                    SupervisorName = x.Supervisor.FullName,
                }).ToList(),
                CreatedDate = topic.CreatedDate,
            });
    }

    public async Task<OperationResult<PaginatedList<TopicForStudentResponse>>> GetAvailableTopicsForGroupAsync(
        TopicForGroupParams request)
    {
        var currentSemester = await semesterService.GetCurrentSemesterAsync();
        if (currentSemester.IsFailure)
            return OperationResult.Failure<PaginatedList<TopicForStudentResponse>>(new Error("Error.GetTopicsFailed",
                "The current semester is not existed!"));
        float? averageGpa = null;
        if (currentUser.Role == UserRoles.Student)
        {
            averageGpa = await GetAverageGPAOfGroupByStudent(currentUser.UserCode, default);
        }

#pragma warning disable CA1304 // Specify CultureInfo
#pragma warning disable CA1862 // Use the 'StringComparison' method overloads to perform case-insensitive string comparisons
#pragma warning disable CA1311 // Specify a culture or use an invariant version
        var searchTerm = request.SearchTerm?.ToLower().Trim();

        var topics = await topicRepository.FindPaginatedAsync(
            x =>
                (request.MainSupervisorEmail == "all" ||
                 x.MainSupervisor.Email == request.MainSupervisorEmail) &&
                (string.IsNullOrEmpty(searchTerm) ||
                x.Code != null && x.Code.ToLower().Contains(searchTerm) ||
                x.EnglishName.ToLower().Contains(searchTerm) ||
                x.VietnameseName.ToLower().Contains(searchTerm) ||
                x.Abbreviation.ToLower().Contains(searchTerm) ||
                x.Description.ToLower().Contains(searchTerm)) &&
                (request.BusinessAreaId == "all" || x.BusinessAreaId == Guid.Parse(request.BusinessAreaId)) &&
                (request.DifficultyLevel == "all" ||
                 x.DifficultyLevel == Enum.Parse<DifficultyLevel>(request.DifficultyLevel, true)) &&
                x.CapstoneId == currentUser.CapstoneId &&
                x.CampusId == currentUser.CampusId &&
                x.SemesterId == currentSemester.Value.Id &&
                !x.IsAssignedToGroup &&
                x.Status.Equals(TopicStatus.Approved),
            request.PageNumber,
            request.PageSize,
            x => x
                .OrderBy(x => currentUser.Role == UserRoles.Student && averageGpa != null
                ? Math.Abs((int)x.DifficultyLevel - (int)GetDifficultyByGPA(averageGpa))
                : 0)
                .ThenByDescending(x => x.CreatedDate)
                .ThenBy(x => x.Abbreviation),
            x => x.AsSplitQuery()
                .Include(x => x.MainSupervisor)
                .Include(x => x.BusinessArea)
                .Include(x => x.CoSupervisors)
                .ThenInclude(c => c.Supervisor),
            x => new TopicForStudentResponse
            {
                Id = x.Id.ToString(),
                Code = x.Code!,
                MainSupervisorId = x.MainSupervisorId,
                MainSupervisorEmail = x.MainSupervisor.Email,
                MainSupervisorName = x.MainSupervisor.FullName,
                EnglishName = x.EnglishName,
                VietnameseName = x.VietnameseName,
                Abbreviation = x.Abbreviation,
                Description = x.Description,
                FileName = x.FileName,
                FileUrl = x.FileUrl,
                Status = x.Status.ToString(),
                DifficultyLevel = x.DifficultyLevel.ToString(),
                BusinessAreaName = x.BusinessArea.Name,
                CampusId = x.CampusId,
                SemesterId = x.SemesterId,
                CapstoneId = x.CapstoneId,
                CoSupervisors = x.CoSupervisors.Select(x => new CoSupervisorDto
                {
                    SupervisorCode = x.SupervisorId,
                    SupervisorEmail = x.Supervisor.Email,
                    SupervisorName = x.Supervisor.FullName,
                }).ToList(),
                CreatedDate = x.CreatedDate,
                NumberOfTopicRequest = x.TopicRequests.Count(x => x.Status == TopicRequestStatus.UnderReview),
            });
#pragma warning restore CA1311 // Specify a culture or use an invariant version
#pragma warning restore CA1862 // Use the 'StringComparison' method overloads to perform case-insensitive string comparisons
#pragma warning restore CA1304 // Specify CultureInfo

        return OperationResult.Success(topics);
    }

    private static DifficultyLevel GetDifficultyByGPA(double? gpa)
    {
        if (gpa >= 8)
            return DifficultyLevel.Hard;
        if (gpa >= 6)
            return DifficultyLevel.Medium;
        return DifficultyLevel.Easy;
    }

    public async Task<OperationResult<IList<TopicResponse>>> GetTopicsByManagerLevel()
    {
        var topics = await topicRepository.FindAsync(
            x => (currentUser.CapstoneId == "all" || x.CapstoneId == currentUser.CapstoneId) &&
                 (currentUser.CampusId == "all" || x.CampusId == currentUser.CampusId),
            x => x.AsSplitQuery()
                .Include(x => x.MainSupervisor)
                .Include(x => x.BusinessArea)
                .Include(x => x.CoSupervisors)
                .ThenInclude(c => c.Supervisor)
                .ThenInclude(t => t.TopicAppraisals),
            x => x.OrderByDescending(x => x.CreatedDate),
            x => new TopicResponse
            {
                Id = x.Id.ToString(),
                Code = x.Code ?? "undefined",
                MainSupervisorId = x.MainSupervisorId,
                MainSupervisorEmail = x.MainSupervisor.Email,
                MainSupervisorName = x.MainSupervisor.FullName,
                EnglishName = x.EnglishName,
                VietnameseName = x.VietnameseName,
                Abbreviation = x.Abbreviation,
                Description = x.Description,
                FileName = x.FileName,
                FileUrl = x.FileUrl,
                Status = x.Status.ToString(),
                DifficultyLevel = x.DifficultyLevel.ToString(),
                BusinessAreaName = x.BusinessArea.Name,
                CampusId = x.CampusId,
                SemesterId = x.SemesterId,
                CapstoneId = x.CapstoneId,
                CoSupervisors = x.CoSupervisors.Select(x => new CoSupervisorDto
                {
                    SupervisorCode = x.SupervisorId,
                    SupervisorEmail = x.Supervisor.Email,
                    SupervisorName = x.Supervisor.FullName,
                }).ToList(),
                CreatedDate = x.CreatedDate,
                TopicAppraisals = x.TopicAppraisals.Select(x => new TopicAppraisalDto
                {
                    AppraisalComment = x.AppraisalComment,
                    AppraisalContent = x.AppraisalContent,
                    AppraisalDate = x.AppraisalDate,
                    SupervisorId = x.SupervisorId,
                    Status = x.Status,
                    TopicAppraisalId = x.Id,
                    TopicId = x.TopicId,
                    AttemptTime = x.AttemptTime,
                    CreatedDate = x.CreatedDate
                }).ToList()
            });

        return OperationResult.Success(topics);
    }

    public async Task<OperationResult<IList<TopicResponse>>> GetTopicsBySupervisor()
    {
        var topics = await topicRepository.FindAsync(
            x => x.MainSupervisorId == currentUser.UserCode,
            x => x.AsSplitQuery()
                .Include(x => x.MainSupervisor)
                .Include(x => x.BusinessArea)
                .Include(x => x.CoSupervisors)
                .ThenInclude(c => c.Supervisor),
            x => x.OrderByDescending(x => x.CreatedDate),
            x => new TopicResponse
            {
                Id = x.Id.ToString(),
                Code = x.Code ?? "undefined",
                MainSupervisorId = x.MainSupervisorId,
                MainSupervisorEmail = x.MainSupervisor.Email,
                MainSupervisorName = x.MainSupervisor.FullName,
                EnglishName = x.EnglishName,
                VietnameseName = x.VietnameseName,
                Abbreviation = x.Abbreviation,
                Description = x.Description,
                FileName = x.FileName,
                FileUrl = x.FileUrl,
                Status = x.Status.ToString(),
                DifficultyLevel = x.DifficultyLevel.ToString(),
                BusinessAreaName = x.BusinessArea.Name,
                CampusId = x.CampusId,
                SemesterId = x.SemesterId,
                CapstoneId = x.CapstoneId,
                CoSupervisors = x.CoSupervisors.Select(x => new CoSupervisorDto
                {
                    SupervisorCode = x.SupervisorId,
                    SupervisorEmail = x.Supervisor.Email,
                    SupervisorName = x.Supervisor.FullName,
                }).ToList(),
                CreatedDate = x.CreatedDate,
            });

        return OperationResult.Success(topics);
    }

    public async Task<OperationResult<IList<TopicResponse>>> GetTopicsByCoSupervisor()
    {
        var topics = await topicRepository.FindAsync(
            x => x.CoSupervisors.Any(x => x.SupervisorId == currentUser.UserCode),
            x => x.AsSplitQuery()
                .Include(x => x.MainSupervisor)
                .Include(x => x.BusinessArea)
                .Include(x => x.CoSupervisors)
                .ThenInclude(c => c.Supervisor),
            x => x.OrderByDescending(x => x.CreatedDate),
            x => new TopicResponse
            {
                Id = x.Id.ToString(),
                Code = x.Code ?? "undefined",
                MainSupervisorId = x.MainSupervisorId,
                MainSupervisorEmail = x.MainSupervisor.Email,
                MainSupervisorName = x.MainSupervisor.FullName,
                EnglishName = x.EnglishName,
                VietnameseName = x.VietnameseName,
                Abbreviation = x.Abbreviation,
                Description = x.Description,
                FileName = x.FileName,
                FileUrl = x.FileUrl,
                Status = x.Status.ToString(),
                DifficultyLevel = x.DifficultyLevel.ToString(),
                BusinessAreaName = x.BusinessArea.Name,
                CampusId = x.CampusId,
                SemesterId = x.SemesterId,
                CapstoneId = x.CapstoneId,
                CoSupervisors = x.CoSupervisors.Select(x => new CoSupervisorDto
                {
                    SupervisorCode = x.SupervisorId,
                    SupervisorEmail = x.Supervisor.Email,
                    SupervisorName = x.Supervisor.FullName,
                }).ToList(),
                CreatedDate = x.CreatedDate,
            });

        return OperationResult.Success(topics);
    }

#pragma warning disable CA1304 // Specify CultureInfo
#pragma warning disable CA1862 // Use the 'StringComparison' method overloads to perform case-insensitive string comparisons
#pragma warning disable CA1311 // Specify a culture or use an invariant version
    public async Task<OperationResult<PaginatedList<TopicResponse>>> GetTopics(TopicParams request)
    {
        var searchTerm = request.SearchTerm?.ToLower().Trim();

        var topics = await topicRepository.FindPaginatedAsync(
            x => (request.MainSupervisorEmail == "all" ||
                  x.MainSupervisor.Email == request.MainSupervisorEmail)
                 &&
                 (string.IsNullOrEmpty(searchTerm) ||
                  x.Code != null && x.Code.ToLower().Contains(searchTerm) ||
                  x.EnglishName.ToLower().Contains(searchTerm) ||
                  x.VietnameseName.ToLower().Contains(searchTerm) ||
                  x.Abbreviation.ToLower().Contains(searchTerm) ||
                  x.Description.ToLower().Contains(searchTerm)) &&
                 (request.BusinessAreaId == "all" ||
                  x.BusinessAreaId == Guid.Parse(request.BusinessAreaId)) &&
                 (request.DifficultyLevel == "all" ||
                  x.DifficultyLevel == Enum.Parse<DifficultyLevel>(request.DifficultyLevel, true)) &&
                 (request.Status == "all" || x.Status == Enum.Parse<TopicStatus>(request.Status, true)) &&
                 (request.CapstoneId == "all" || x.CapstoneId == request.CapstoneId) &&
                 (request.CampusId == "all" || x.CampusId == request.CampusId) &&
                 (request.SemesterId == "all" || x.SemesterId == request.SemesterId),
            request.PageNumber,
            request.PageSize,
            x => x.OrderByDescending(x => x.CreatedDate),
            x => x.AsSplitQuery()
                .Include(x => x.MainSupervisor)
                .Include(x => x.BusinessArea)
                .Include(x => x.CoSupervisors)
                .ThenInclude(c => c.Supervisor),
            x => new TopicResponse
            {
                Id = x.Id.ToString(),
                Code = x.Code ?? "undefined",
                MainSupervisorEmail = x.MainSupervisor.Email,
                MainSupervisorName = x.MainSupervisor.FullName,
                EnglishName = x.EnglishName,
                VietnameseName = x.VietnameseName,
                Abbreviation = x.Abbreviation,
                Description = x.Description,
                FileName = x.FileName,
                FileUrl = x.FileUrl,
                Status = x.Status.ToString(),
                DifficultyLevel = x.DifficultyLevel.ToString(),
                BusinessAreaName = x.BusinessArea.Name,
                CampusId = x.CampusId,
                SemesterId = x.SemesterId,
                CapstoneId = x.CapstoneId,
                CoSupervisors = x.CoSupervisors.Select(x => new CoSupervisorDto
                {
                    SupervisorCode = x.SupervisorId,
                    SupervisorEmail = x.Supervisor.Email,
                    SupervisorName = x.Supervisor.FullName,
                }).ToList(),
                CreatedDate = x.CreatedDate,
            });

        return OperationResult.Success(topics);
    }
#pragma warning restore CA1311 // Specify a culture or use an invariant version
#pragma warning restore CA1862 // Use the 'StringComparison' method overloads to perform case-insensitive string comparisons
#pragma warning restore CA1304 // Specify CultureInfo

    public async Task<TopicResponse?> GetTopicByTopicCode(string? topicCode)
    {
        var topic = await topicRepository.GetAsync(t => t.Code.Equals(topicCode),
            t => new TopicResponse
            {
                Id = t.Id.ToString(),
                Code = t.Code ?? "undefined",
                Abbreviation = t.Abbreviation,
                Description = t.Description,
                FileName = t.FileName,
                FileUrl = t.FileUrl,
                Status = t.Status.ToString(),
                CreatedDate = t.CreatedDate,
                CampusId = t.CampusId,
                CapstoneId = t.CapstoneId,
                SemesterId = t.SemesterId,
                DifficultyLevel = t.DifficultyLevel.ToString(),
                BusinessAreaName = t.BusinessArea.Name,
                EnglishName = t.EnglishName,
                VietnameseName = t.VietnameseName,
                MainSupervisorEmail = t.MainSupervisor.Email,
                MainSupervisorName = t.MainSupervisor.FullName,
                CoSupervisors = t.CoSupervisors.Select(c => new CoSupervisorDto()
                {
                    SupervisorCode = c.SupervisorId,
                    SupervisorEmail = c.Supervisor.Email,
                    SupervisorName = c.Supervisor.FullName
                }).ToList()
            },
            t => t.AsSplitQuery()
                .Include(t => t.BusinessArea)
                .Include(t => t.MainSupervisor)
                .Include(t => t.CoSupervisors)
                .ThenInclude(co => co.Supervisor));
        return topic;
    }

    public async Task<OperationResult> SemanticTopic(Guid topicId, bool withCurrentSemester,
        CancellationToken cancellationToken)
    {
        var topic = await topicRepository.GetAsync(x => x.Id == topicId, cancellationToken);

        return topic == null
            ? OperationResult.Failure(new Error("Topic.Error", "Topic does not exist."))
            : await SemanticTopic(topic, withCurrentSemester, cancellationToken);
    }

    private async Task<OperationResult> SemanticTopic(Topic topic, bool withCurrentSemester,
        CancellationToken cancellationToken)
    {
        try
        {
            var key = $"processing/{topic.Id.ToString()}";
            var isInprogress = await cache.GetAsync<object>(key, cancellationToken);

            if (isInprogress != null)
            {
                return OperationResult.Failure(new Error("Topic.Error",
                    "Topic is in semantic progressing. Try it later!"));
            }

            var currentSemester = await semesterService.GetCurrentSemesterAsync();

            if (currentSemester.IsFailure)
            {
                return OperationResult.Failure(currentSemester.Error);
            }

            var semesterIds = withCurrentSemester
                ? new List<string> { currentSemester.Value.Id }
                : await semesterService.GetPreviouseSemesterIds(currentSemester.Value.StartDate);

            integrationEventLogService.SendEvent(new SemanticTopicEvent
            {
                TopicId = topic.Id.ToString(),
                TopicEnglishName = topic.EnglishName,
                IsCurrentSemester = withCurrentSemester,
                SemesterIds = semesterIds,
                ProcessedBy = currentUser.UserCode,
                CampusId = topic.CampusId,
                CapstoneId = topic.CapstoneId,
            });

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Can not send the semantic event with error: {Error}", ex.Message);
            return OperationResult.Failure(new Error("Topic.Error", "Semantic topic was fail."));
        }
    }

    public async Task<OperationResult<Guid>> CreateTopic(CreateTopicRequest request,
        CancellationToken cancellationToken)
    {
        var mainSupervisor = await supervisorRepository.GetAsync(s => s.Email == currentUser.Email, cancellationToken);

        if (mainSupervisor == null)
        {
            logger.LogError("Create Topic fail with error: {SupervisorId} does not exist", currentUser.Id);
            return OperationResult.Failure<Guid>(new Error("Topic.Error", "Supervisor does not exist."));
        }

        if (!mainSupervisor.IsAvailable)
        {
            logger.LogError("Create Topic fail with error: {SupervisorId} is not availabke", currentUser.Id);
            return OperationResult.Failure<Guid>(new Error("Topic.Error", "Supervisor is not available."));
        }

        if (!await businessRepository.AnyAsync(b => b.Id == request.BusinessAreaId, cancellationToken))
        {
            logger.LogError("Create Topic fail with error: {BusinessArea} does not exist", request.BusinessAreaId);
            return OperationResult.Failure<Guid>(new Error("Topic.Error", "BusinessArea does not exist."));
        }

        if (!await capstoneRepository.AnyAsync(c => c.Id == request.CapstoneId, cancellationToken))
        {
            logger.LogError("Create Topic fail with error: {Capstone} does not exist", request.CapstoneId);
            return OperationResult.Failure<Guid>(new Error("Topic.Error", "Capstone does not exist."));
        }

        if (request.CoSupervisorEmails.Exists(x => x == currentUser.Email))
        {
            return OperationResult.Failure<Guid>(new Error("Topic.Error",
                "The supervisor can not copporate topic himself."));
        }

        var availableSupportSupervisors =
            await GetAvailableSupervisorsForSupportingTopic(request.CoSupervisorEmails, cancellationToken);

        if (availableSupportSupervisors.Count < request.CoSupervisorEmails.Count(x => !string.IsNullOrEmpty(x)))
        {
            return OperationResult.Failure<Guid>(new Error("Topic.Error", "Someone can not be assigned for topic"));
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
            var key =
                $"{currentUser.CampusId}/{getCurrentSemesterResult.Value.Id}/{request.CapstoneId}/{topicId}";

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
                Status = TopicStatus.Pending,
                CoSupervisors = availableSupportSupervisors.Select(s => new CoSupervisor
                {
                    TopicId = topicId,
                    SupervisorId = s.SupervisorId
                }).ToList()
            };

            topicRepository.Insert(topic);

            integrationEventLogService.SendEvent(new SemanticTopicEvent
            {
                TopicId = topic.Id.ToString(),
                TopicEnglishName = request.EnglishName,
                SemesterIds = await semesterService.GetPreviouseSemesterIds(getCurrentSemesterResult.Value.StartDate),
                ProcessedBy = currentUser.UserCode,
                IsCurrentSemester = false,
                CampusId = currentUser.CampusId,
                CapstoneId = request.CapstoneId
            });

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

            return OperationResult.Failure<Guid>(new Error("Topic.Error", "Fail to create topic."));
        }
    }

    public async Task<OperationResult> UpdateTopic(UpdateTopicRequest request,
        CancellationToken cancellationToken)
    {
        var mainSupervisor = await supervisorRepository.GetAsync(s => s.Email == currentUser.Email, cancellationToken);

        if (mainSupervisor == null)
        {
            logger.LogError("Update Topic fail with error: {SupervisorId} does not exist", currentUser.Id);
            return OperationResult.Failure<Guid>(new Error("Topic.Error", "Supervisor does not exist."));
        }

        if (!mainSupervisor.IsAvailable)
        {
            logger.LogError("Update Topic fail with error: {SupervisorId} is not availabke", currentUser.Id);
            return OperationResult.Failure<Guid>(new Error("Topic.Error", "Supervisor is not available."));
        }

        if (string.IsNullOrEmpty(request.Description) && request.File != null ||
            !string.IsNullOrEmpty(request.Description) && request.File == null)
        {
            logger.LogError("Update Topic fail with error");
            return OperationResult.Failure<Guid>(new Error("Topic.Error",
                "You need to update Description and File at the same time."));
        }

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var existedTopic = await topicRepository.GetAsync(
                x => x.Id == request.TopicId,
                isEnabledTracking: true,
                null,
                null,
                cancellationToken);

            if (existedTopic == null)
            {
                return OperationResult.Failure(new Error("Topic.Error", "Fail to update topic."));
            }

            if (existedTopic.MainSupervisorId != currentUser.UserCode)
            {
                return OperationResult.Failure(new Error("Topic.Error",
                    "The topic just only update by who had created it."));
            }

            if (existedTopic.Status == TopicStatus.Approved || existedTopic.Status == TopicStatus.Rejected)
            {
                return OperationResult.Failure(new Error("Topic.Error", "The topic can not update"));
            }

            mapper.Map(request, existedTopic);
            topicRepository.Update(existedTopic);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            if (request.File != null)
            {
                existedTopic.FileName = request.File.FileName;

                var semanticProgress = await SemanticTopic(existedTopic, withCurrentSemester: false, cancellationToken);

                if (semanticProgress.IsFailure)
                {
                    throw new InvalidOperationException(semanticProgress.Error.Message);
                }

                if (!await SaveToS3(request.File, existedTopic.FileUrl, cancellationToken))
                {
                    throw new AmazonS3Exception("Failed to save to S3, database changes rolled back.");
                }
            }

            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Fail to update topic with error {Message}", ex.Message);
            await unitOfWork.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("Topic.Error", "Fail to update topic."));
        }
    }

    public async Task<OperationResult<string>> PresentTopicPresignedUrl(Guid topicId,
        CancellationToken cancellationToken)
    {
        var topic = await topicRepository.GetAsync(t => t.Id == topicId, cancellationToken);

        if (topic == null)
        {
            return OperationResult.Failure<string>(new Error("Topic.Error", "Topic does not exist."));
        }

        try
        {
            var cacheKey = string.Join("/", s3BucketConfiguration.FUCTopicBucket, topic.FileUrl);

            var presignedUrl = await cache.GetAsync<string>(cacheKey, default);

            if (presignedUrl is null)
            {
                presignedUrl = await s3Service.GetPresignedUrl(s3BucketConfiguration.FUCTopicBucket,
                    topic.FileUrl, 1440,
                    isUpload: false);

                await cache.SetTimeoutAsync(cacheKey, presignedUrl, TimeSpan.FromDays(1), default);
            }

            return OperationResult.Success(presignedUrl);
        }
        catch (Exception ex)
        {
            logger.LogError("Get presignUrl for topic failed with error {Error}", ex.Message);
            return OperationResult.Failure<string>(new Error("Topic.Error", "Get presigned url of topic fail."));
        }
    }

    public async Task<OperationResult<List<TopicStatisticResponse>>> GetTopicAnalysises(Guid topicId,
        CancellationToken cancellationToken)
    {
        var topicAnalysises = await topicAnalysisRepository.FindAsync(x => x.TopicId == topicId,
            orderBy: x => x.OrderByDescending(x => x.CreatedDate),
            cancellationToken);

        var result = new List<TopicStatisticResponse>();

        foreach (var item in topicAnalysises)
        {
            int over80ratio = 0;
            int over90ratio = 0;

            var analysis = JsonSerializer.Deserialize<Dictionary<string, MatchingTopic>>(item.AnalysisResult);

            if (analysis is null || analysis.Count == 0)
            {
                result.Add(new TopicStatisticResponse
                {
                    Analysises = default,
                    Over80Ratio = over80ratio,
                    Over90Ratio = over90ratio,
                    CreatedDate = item.CreatedDate,
                    ProcessedBy = item.ProcessedBy,
                    StatusSemantic = "clean"
                });
                continue;
            }

            var analysisResponse = new List<TopicAnalysisResponse>();

            foreach (var a in analysis)
            {
                if (a.Value.Similarity > 80)
                {
                    over80ratio++;
                }

                if (a.Value.Similarity > 90)
                {
                    over90ratio++;
                }

                analysisResponse.Add(new TopicAnalysisResponse
                {
                    AnalysisTopicId = a.Key,
                    EnglishName = a.Value.EnglishName,
                    Similarity = a.Value.Similarity,
                });
            }

            result.Add(new TopicStatisticResponse
            {
                Analysises = analysisResponse,
                Over80Ratio = analysisResponse.Count != 0 ? (double)over80ratio / analysisResponse.Count : 0,
                Over90Ratio = analysisResponse.Count != 0 ? (double)over90ratio / analysisResponse.Count : 0,
                CreatedDate = item.CreatedDate,
                ProcessedBy = item.ProcessedBy,
                StatusSemantic = "un_clean"
            });
        }

        return OperationResult.Success(result);
    }

    public async Task<OperationResult> AssignTopicAppraisalForAvailableSupervisors(
        IReadOnlyList<string> supervisorEmail, CancellationToken cancellationToken)
    {
        try
        {
            // TODO: Check the valid date to assign supervisors to topics (TimeConfig table)

            var currentSemester = await semesterService.GetCurrentSemesterAsync();
            if (currentSemester.IsFailure)
            {
                logger.LogError("The current semester is inactive!");
                return OperationResult.Failure(
                    new Error("Error.CurrentSemesterIsNull", "Current semester is inactive!"));
            }

            if (supervisorEmail.Count < TopicAppraisalRequirement.SupervisorAppraisalMinimum)
            {
                logger.LogError("The Supervisor Appraisal size is invalid!");
                return OperationResult.Failure(new Error("Error.InvalidSupervisorAppraisalSize",
                    $"The Supervisor appraisal must be at least {TopicAppraisalRequirement.SupervisorAppraisalMinimum}"));
            }

            var supervisorIdList = await (from s in supervisorRepository.GetQueryable()
                                          where supervisorEmail.Contains(s.Email) &&
                                                s.IsAvailable &&
                                                s.MajorId == currentUser.MajorId &&
                                                s.CampusId == currentUser.CampusId
                                          select s.Id).ToListAsync(cancellationToken);

            if (supervisorIdList.Count == 0)
            {
                logger.LogError("Supervisors were not found!");
                return OperationResult.Failure(Error.NullValue);
            }

            IList<Topic> topicList = await topicRepository
                .FindAsync(t => t.Status == TopicStatus.Pending &&
                                t.CampusId == currentUser.CampusId &&
                                t.CapstoneId == currentUser.CapstoneId &&
                                t.SemesterId == currentSemester.Value.Id,
                    t => t.Include(t => t.TopicAppraisals)
                        .Include(t => t.CoSupervisors)
                        .Include(t => t.Capstone),
                    cancellationToken);

            topicList = topicList
                .Where(t => t.Capstone.MajorId == currentUser.MajorId &&
                            (t.TopicAppraisals == null || t.TopicAppraisals.Count == 0))
                .ToList();

            if (!topicList.Any())
            {
                logger.LogError("No pending topics found!");
                return OperationResult.Failure(Error.NullValue);
            }

            // Shuffle topics and supervisors to ensure fairness
            var rand = new Random();
#pragma warning disable CA5394 // Do not use insecure randomness
            topicList = topicList.OrderBy(_ => rand.Next()).ToList();
            supervisorIdList = supervisorIdList.OrderBy(_ => rand.Next()).ToList();
#pragma warning restore CA5394 // Do not use insecure randomness

            // Track supervisor assignments count
            var supervisorAssignments = supervisorIdList.ToDictionary(s => s, _ => 0);
            var topicAppraisalList = new List<TopicAppraisal>();

            foreach (var topic in topicList)
            {
                var assignedSupervisors = new HashSet<string>();

                while (assignedSupervisors.Count <
                       systemConfigService.GetSystemConfiguration().MaxTopicAppraisalsForTopic)
                {
                    var availableSupervisors = supervisorAssignments
                        .Where(s => s.Key != topic.MainSupervisorId &&
                                    !topic.CoSupervisors.Any(x => x.SupervisorId == s.Key) &&
                                    !assignedSupervisors.Contains(s.Key))
                        .OrderBy(s => s.Value) // Pick the least assigned
                        .ToList();

                    if (availableSupervisors.Count == 0)
                        break; // No more available supervisors

                    var supervisorId = availableSupervisors[0].Key;

                    assignedSupervisors.Add(supervisorId);
                    supervisorAssignments[supervisorId]++;
                }

                topicAppraisalList.AddRange(assignedSupervisors.Select(s => new TopicAppraisal
                {
                    SupervisorId = s,
                    TopicId = topic.Id,
                    AttemptTime = 1
                }));
            }

            await unitOfWork.BeginTransactionAsync(cancellationToken);

            topicAppraisalRepository.InsertRange(topicAppraisalList);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            integrationEventLogService.SendEvent(new AssignedAvailableSupervisorForAppraisalEvent
            {
                SupervisorIds = supervisorIdList
            });

            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Fail to assign all the available supervisors with error: {Message}", ex.Message);

            await unitOfWork.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("TopicAppraisal.Error",
                "Fail to assign supervisors for appraisal."));
        }
    }

    public async Task<OperationResult> AssignSupervisorForAppraisalTopic(AssignSupervisorAppraisalTopicRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var currentSemester = await semesterService.GetCurrentSemesterAsync();

            if (currentSemester.IsFailure)
                return OperationResult.Failure(currentSemester.Error);

            var topic = await topicRepository.GetAsync(x => x.Id == request.TopicId,
                isEnabledTracking: true,
                include: x => x.AsSplitQuery()
                    .Include(x => x.TopicAppraisals)
                    .Include(x => x.Capstone)
                    .Include(x => x.CoSupervisors),
                orderBy: null,
                cancellationToken);

            if (topic == null || topic.Status != TopicStatus.Pending)
                return OperationResult.Failure(Error.NullValue);

            if (topic.MainSupervisorId == request.SupervisorId ||
                topic.CoSupervisors.Any(x => x.SupervisorId == request.SupervisorId))
                return OperationResult.Failure(new Error("Topic.Error",
                    "Can not assign this supervisor for his/her topic."));

            if (topic.SemesterId != currentSemester.Value.Id ||
                topic.CampusId != currentUser.CampusId ||
                topic.CapstoneId != currentUser.CapstoneId)
            {
                return OperationResult.Failure(new Error("Topic.Error", "You can not assign this topic."));
            }

            var assignedSupvisor = await supervisorRepository.GetAsync(
                x => x.Id == request.SupervisorId &&
                     x.IsAvailable,
                cancellationToken);

            if (assignedSupvisor is null)
                return OperationResult.Failure(Error.NullValue);

            if (topic.CampusId != assignedSupvisor.CampusId || topic.Capstone.MajorId != currentUser.MajorId)
            {
                return OperationResult.Failure(
                    new Error("Topic.Error", "You can not assign this superviosr for topic."));
            }

            var newAttempTime = topic.TopicAppraisals.Max(x => x.AttemptTime);

            if (topic.TopicAppraisals.Count(x => x.AttemptTime == newAttempTime)
                == systemConfigService.GetSystemConfiguration().MaxTopicAppraisalsForTopic)
                return OperationResult.Failure(new Error("Topic.Error", "This topic has enough appraisal."));

            if (!topic.TopicAppraisals.Exists(x => x.AttemptTime == newAttempTime &&
                                                   x.Status == TopicAppraisalStatus.Pending))
            {
                newAttempTime++;
            }

            await unitOfWork.BeginTransactionAsync(cancellationToken);

            topic.TopicAppraisals.Add(new TopicAppraisal
            {
                SupervisorId = request.SupervisorId,
                AttemptTime = newAttempTime
            });

            topicRepository.Update(topic);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            integrationEventLogService.SendEvent(new AssignedSupervisorForAppraisalEvent
            {
                TopicId = topic.Id,
                SupervisorId = request.SupervisorId,
                TopicEnglishName = topic.EnglishName
            });

            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Fail to assign appraisal for topic {Id} with error {Message}", request.TopicId,
                ex.Message);

            await unitOfWork.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("Topic.Error", "Fail to assign topic appraisal."));
        }
    }

    public async Task<OperationResult> RemoveAssignSupervisorForAppraisalTopic(
        RemoveAssignSupervisorAppraisalTopicRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var topic = await topicRepository.GetAsync(x => x.Id == request.TopicId,
                isEnabledTracking: true,
                include: x => x.Include(x => x.TopicAppraisals),
                orderBy: null,
                cancellationToken);

            if (topic == null || topic.Status != TopicStatus.Pending)
                return OperationResult.Failure(Error.NullValue);

            var topicAppraisal = topic.TopicAppraisals.Find(x => x.Id == request.TopicAppraisalId);

            if (topicAppraisal == null)
                return OperationResult.Success();

            if (topicAppraisal.Status != TopicAppraisalStatus.Pending)
                return OperationResult.Failure(new Error("Topic.Error", "This topic was appraised."));

            await unitOfWork.BeginTransactionAsync(cancellationToken);

            topic.TopicAppraisals.Remove(topicAppraisal);

            topicRepository.Update(topic);

            integrationEventLogService.SendEvent(new SupervisorAppraisalRemovedEvent
            {
                SupervisorId = topicAppraisal.SupervisorId,
                TopicId = topic.Id,
                TopicEnglishName = topic.EnglishName
            });

            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Fail to remove appraisal for topic {Id} with error {Message}", request.TopicId,
                ex.Message);

            return OperationResult.Failure(new Error("Topic.Error", "Fail to remove topic appraisal."));
        }
    }

    public async Task<OperationResult<List<TopicAppraisalResponse>>> GetTopicAppraisalByUserId(
        TopicAppraisalBaseRequest request)
    {
        var query = topicAppraisalRepository.GetQueryable();

        query = query.Where(ta => ta.SupervisorId == currentUser.UserCode)
            .Include(ta => ta.Topic);

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(ta => ta.Topic.EnglishName.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrEmpty(request.Status) &&
            Enum.TryParse<TopicAppraisalStatus>(
                request.Status,
                true,
                out TopicAppraisalStatus enumStatus))
        {
            query = enumStatus switch
            {
                TopicAppraisalStatus.Pending => query.Where(ta => ta.Status.Equals(TopicAppraisalStatus.Pending)),
                TopicAppraisalStatus.Accepted => query.Where(ta => ta.Status.Equals(TopicAppraisalStatus.Accepted)),
                TopicAppraisalStatus.Considered => query.Where(ta => ta.Status.Equals(TopicAppraisalStatus.Considered)),
                TopicAppraisalStatus.Rejected => query.Where(ta => ta.Status.Equals(TopicAppraisalStatus.Rejected)),
                _ => query
            };
        }

        if (!string.IsNullOrEmpty(request.OrderByAppraisalDate))
        {
            query = request.OrderByAppraisalDate switch
            {
                "_asc" => query.OrderByDescending(x => x.AttemptTime).ThenBy(ta => ta.CreatedDate),
                _ => query.OrderByDescending(x => x.AttemptTime).ThenByDescending(ta => ta.CreatedDate)
            };
        }

        var response = await query.Select(ta => new TopicAppraisalResponse
        {
            TopicAppraisalId = ta.Id,
            SupervisorId = ta.SupervisorId,
            Status = ta.Status.ToString(),
            AppraisalComment = ta.AppraisalComment,
            AppraisalContent = ta.AppraisalContent,
            AppraisalDate = ta.AppraisalDate,
            TopicId = ta.TopicId,
            TopicEnglishName = ta.Topic.EnglishName,
            AttemptTime = ta.AttemptTime,
            CreatedDate = ta.CreatedDate
        }).ToListAsync();

        return OperationResult.Success(response);
    }

    public async Task<OperationResult> AppraisalTopic(AppraisalTopicRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request.Status == TopicAppraisalStatus.Pending)
            {
                return OperationResult.Failure(new Error("TopicAppraisal.Error", "This appraisal is already Pending."));
            }

            var topic = await topicRepository.GetAsync(
                t => t.Id == request.TopicId,
                include: t => t.AsSplitQuery()
                    .Include(x => x.TopicAppraisals)
                    .Include(x => x.Capstone),
                orderBy: null,
                cancellationToken);

            if (topic is null || topic.TopicAppraisals.Single(x => x.Id == request.TopicAppraisalId) is null)
            {
                return OperationResult.Failure(new Error("Topic.Error",
                    $"The appraisal topic of supervisor: {currentUser.Name} does not exist."));
            }

            var topicAppraisal = topic.TopicAppraisals.Single(x => x.Id == request.TopicAppraisalId);

            if (topicAppraisal.SupervisorId != currentUser.UserCode)
                return OperationResult.Failure(new Error("TopicAppraisal.Error",
                    "You can appraisal your assigned Topic."));

            if (topicAppraisal.Status != TopicAppraisalStatus.Pending)
            {
                return OperationResult.Failure(new Error("TopicAppraisal.Error", "This appraisal was reviewed."));
            }

            await unitOfWork.BeginTransactionAsync(cancellationToken);

            SubmitAppraisalTopic(topicAppraisal,
                request.AppraisalContent,
                request.AppraisalComment,
                request.Status);

            var otherTopicAppraisals = topic.TopicAppraisals.Where(
                                           x => x.AttemptTime == topicAppraisal.AttemptTime &&
                                                x.Id != topicAppraisal.Id).ToList()
                                       ?? new List<TopicAppraisal>();

            if (systemConfigService.GetSystemConfiguration().MaxTopicAppraisalsForTopic > 0
                && otherTopicAppraisals.Count == 0)
            {
                return OperationResult.Failure(new Error("Topic.Error",
                    $"The other appraisal of this topic does not exist."));
            }

            if ((topicAppraisal.Status == TopicAppraisalStatus.Accepted ||
                 topicAppraisal.Status == TopicAppraisalStatus.Rejected) &&
                otherTopicAppraisals.TrueForAll(x => x.Status == topicAppraisal.Status))
            {
                await (topicAppraisal.Status switch
                {
                    TopicAppraisalStatus.Accepted => UpdateStatusTopicAfterAppraisal(topic,
                        TopicStatus.Approved, cancellationToken),
                    TopicAppraisalStatus.Rejected => UpdateStatusTopicAfterAppraisal(topic,
                        TopicStatus.Rejected, cancellationToken),
                    _ => throw new InvalidOperationException()
                });
            }
            else if (otherTopicAppraisals.TrueForAll(x =>
                         x.AppraisalDate is not null && x.Status != TopicAppraisalStatus.Pending))
            {
                await UpdateStatusTopicAfterAppraisal(topic, TopicStatus.Considered,
                    cancellationToken);
            }

            topicRepository.Update(topic);

            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Submit appraisal topic fail with {Error}", ex.Message);
            await unitOfWork.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("Topic.Error", "Appraisal Topic create fail."));
        }
    }

    public async Task<OperationResult> ReAppraisalTopicForMainSupervisorOfTopic(Guid topicId,
        CancellationToken cancellationToken)
    {
        try
        {
            var topic = await topicRepository.GetAsync(
                x => x.Id == topicId,
                include: x => x.Include(x => x.TopicAppraisals),
                orderBy: null,
                cancellationToken);

            ArgumentNullException.ThrowIfNull(topic);

            if (topic.MainSupervisorId != currentUser.UserCode)
                return OperationResult.Failure(new Error("Topic.Error",
                    "Only main supervisor of topic can do this action."));

            if (topic.Status == TopicStatus.Approved || topic.Status == TopicStatus.Rejected)
                return OperationResult.Failure(new Error("Topic.Error",
                    $"The status of topic was {topic.Status}, then you can not do this action."));

            if (topic.TopicAppraisals.Count == 0)
                return OperationResult.Failure(new Error("Topic.Error", $"You can not do this action rightnow!."));

            var currentAttemptTime = topic.TopicAppraisals.Max(x => x.AttemptTime);

            var currentTopicAppraisals = topic.TopicAppraisals.Where(x => x.AttemptTime == currentAttemptTime).ToList();

            if (currentTopicAppraisals.Exists(x => x.Status == TopicAppraisalStatus.Pending))
                return OperationResult.Failure(new Error("Topic.Error",
                    $"You can not do this action while supervisor appraisaling."));

            if (!(currentTopicAppraisals[0].AppraisalDate < topic.UpdatedDate &&
                  topic.UpdatedBy != null &&
                  topic.UpdatedBy == currentUser.Email))
                return OperationResult.Failure(new Error("Topic.Error",
                    $"You need to update before appraisal."));

            if (currentTopicAppraisals.TrueForAll(x => x.Status == TopicAppraisalStatus.Accepted) ||
                currentTopicAppraisals.TrueForAll(x => x.Status == TopicAppraisalStatus.Rejected))
                return OperationResult.Failure(new Error("Topic.Error",
                    $"The status of topic was {topic.Status}, then you can not do this action."));

            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var appraisalSupervisors = new List<string>();

            appraisalSupervisors.AddRange(currentTopicAppraisals.Select(x => x.SupervisorId).Distinct());

            var newAppraisals = appraisalSupervisors.Select(s => new TopicAppraisal
            {
                SupervisorId = s,
                TopicId = topic.Id,
                AttemptTime = currentAttemptTime + 1,
            }).ToList();

            if (newAppraisals != null && newAppraisals.Count > 0)
            {
                topic.TopicAppraisals.AddRange(newAppraisals);
            }

            topicRepository.Update(topic);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            integrationEventLogService.SendEvent(new ReAssignAppraisalTopicEvent
            {
                SupervisorIds = appraisalSupervisors,
                TopicId = topic.Id,
                TopicEnglishName = topic.EnglishName
            });

            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Re-appraisal topic fail with {Error}", ex.Message);
            await unitOfWork.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("Topic.Error", "Re-Appraisal Topic fail."));
        }
    }

    private async Task UpdateStatusTopicAfterAppraisal(Topic topic, TopicStatus topicStatus,
        CancellationToken cancellationToken)
    {
        if (topic.Status == TopicStatus.Approved || topic.Status == TopicStatus.Rejected)
        {
            throw new InvalidOperationException("The topic was already appraised");
        }

        topic.Status = topicStatus;

        if (topic.Status == TopicStatus.Approved)
        {
            topic.Code = await GenerationTopicCode(topic.CampusId,
                topic.Capstone.MajorId,
                topic.CapstoneId,
                cancellationToken);
        }

        integrationEventLogService.SendEvent(new TopicStatusUpdatedEvent
        {
            SupervisorId = topic.MainSupervisorId,
            TopicId = topic.Id,
            TopicEnglishName = topic.EnglishName,
            TopicStatus = topicStatus.ToString(),
            TopicCode = topic.Code,
        });
    }

    private async Task<string> GenerationTopicCode(string campusId, string majorId, string capstoneId,
        CancellationToken cancellationToken)
    {
        try
        {
            var currentSemesterCode = await semesterService.GetCurrentSemesterAsync();

            if (currentSemesterCode.IsFailure)
                throw new InvalidOperationException(currentSemesterCode.Error.ToString());

            var nextTopicNumer = await topicRepository.CountAsync(
                t => t.Status == TopicStatus.Approved &&
                     t.SemesterId == currentSemesterCode.Value.Id &&
                     t.CampusId == campusId &&
                     t.CapstoneId == capstoneId,
                cancellationToken) + 1;

#pragma warning disable CA1305 // Specify IFormatProvider
            var topicNumberCode = nextTopicNumer.ToString($"D{Math.Max(3, nextTopicNumer.ToString().Length)}");
#pragma warning restore CA1305 // Specify IFormatProvider

            return $"{currentSemesterCode.Value.Id}{majorId}{topicNumberCode}";
        }
        catch (Exception ex)
        {
            logger.LogError("Generate the topic code fail with error {Message}", ex.Message);
            throw;
        }
    }

    private void SubmitAppraisalTopic(TopicAppraisal topicAppraisal,
        string appraisalContent,
        string appraisalComment,
        TopicAppraisalStatus status)
    {
        topicAppraisal.Status = status;
        topicAppraisal.AppraisalContent = appraisalContent;
        topicAppraisal.AppraisalComment = appraisalComment;
        topicAppraisal.AppraisalDate = DateTime.Now;

        topicAppraisalRepository.Update(topicAppraisal);
    }

    private async Task<List<(string SupervisorId, string SupervisorEmail)>>
        GetAvailableSupervisorsForSupportingTopic(List<string> coSupervisorEmails,
            CancellationToken cancellationToken)
    {
        var result = new List<(string, string)>();

        var query = from s in supervisorRepository.GetQueryable()
                    where coSupervisorEmails.Contains(s.Email) &&
                          s.CoSupervisors.Count < systemConfigService.GetSystemConfiguration().MaxTopicsForCoSupervisors
                    select new { s.Id, s.Email };

        (await query.ToListAsync(cancellationToken))
            .ForEach(s => result.Add((s.Id.ToString(), s.Email)));

        return result;
    }

    private async Task<bool> SaveToS3(IFormFile file, string key, CancellationToken cancellationToken)
    {
        using var stream = file.OpenReadStream();

        var s3Response = await s3Service.SaveToS3(s3BucketConfiguration.FUCTopicBucket, key, file.ContentType, stream,
            cancellationToken);

        return s3Response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    public async Task<OperationResult<List<BusinessAreaResponse>>> GetAllBusinessAreas()
    {
        var queryable = from ba in businessRepository.GetQueryable()
                        select new BusinessAreaResponse
                        {
                            Id = ba.Id,
                            Description = ba.Description,
                            Name = ba.Name
                        };
        var businessAreas = await queryable.ToListAsync();

        return OperationResult.Success(businessAreas);
    }

    public async Task<OperationResult> AssignNewSupervisorForTopic(AssignNewSupervisorForTopicRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var topic = await topicRepository.GetAsync(x => x.Id == request.TopicId,
                isEnabledTracking: true,
                include: x => x.AsSplitQuery()
                    .Include(x => x.Group)
                    .Include(x => x.CoSupervisors),
                orderBy: null,
                cancellationToken);

            ArgumentNullException.ThrowIfNull(topic);

            var oldSupervisorId = topic.MainSupervisorId!;
            var newSupervior =
                await supervisorRepository.GetAsync(x => x.Id == request.SupervisorId, cancellationToken);

            ArgumentNullException.ThrowIfNull(newSupervior);

            await unitOfWork.BeginTransactionAsync(cancellationToken);

            if (topic.IsAssignedToGroup && topic.Group != null)
            {
                topic.Group.SupervisorId = newSupervior.Id;
            }

            var coSupervisor = topic.CoSupervisors.FirstOrDefault(x => x.SupervisorId == request.SupervisorId);

            if (coSupervisor != null)
            {
                topic.CoSupervisors.Remove(coSupervisor);
                coSupervisorRepository.Delete(coSupervisor);
            }

            topic.MainSupervisorId = newSupervior.Id;

            topicRepository.Update(topic);

            integrationEventLogService.SendEvent(new NewSupervisorAssignedForTopicEvent
            {
                NewSupervisorId = newSupervior.Id,
                OldSupervisorId = oldSupervisorId,
                TopicId = topic.Id,
                TopicShortName = topic.Abbreviation,
            });

            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Fail to assign supervisor {SupCode} with topic {TopicId} with error {Message}",
                request.SupervisorId, request.TopicId, ex.Message);

            await unitOfWork.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("Topic.Error", "Fail to assign new supervisor for this topic"));
        }
    }

    public async Task<OperationResult> AddCoSupervisorForTopic(AssignNewSupervisorForTopicRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var topic = await topicRepository.GetAsync(x => x.Id == request.TopicId,
                include: x => x.Include(x => x.CoSupervisors),
                orderBy: null,
                cancellationToken);

            ArgumentNullException.ThrowIfNull(topic);

            var supervior = await supervisorRepository.GetAsync(x => x.Id == request.SupervisorId, cancellationToken);

            ArgumentNullException.ThrowIfNull(supervior);

            if (topic.MainSupervisorId == request.SupervisorId)
                return OperationResult.Failure(new Error("Topic.Error",
                    $"Supervisor {request.SupervisorId} is the main supervisor of Topic."));

            if (topic.CoSupervisors.Any(x => x.SupervisorId == request.SupervisorId))
                return OperationResult.Failure(new Error("Topic.Error",
                    $"Supervisor {request.SupervisorId} was in Topic."));
            if (topic.CoSupervisors.Count == systemConfigService.GetSystemConfiguration().MaxTopicsForCoSupervisors)
                return OperationResult.Failure(new Error("Topic.Error",
                    $"The number of co-supervisors of Topic {topic.Abbreviation} has been reached."));
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            topic.CoSupervisors.Add(new CoSupervisor
            {
                SupervisorId = supervior.Id,
            });

            topicRepository.Update(topic);

            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Fail to assign CoSupervisor {SupCode} with topic {TopicId} with error {Message}",
                request.SupervisorId, request.TopicId, ex.Message);

            await unitOfWork.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("Topic.Error", "Fail to add new coSupervisor for this topic"));
        }
    }

    public async Task<OperationResult> RemoveCoSupervisorForTopic(RemoveCoSupervisorForTopicRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var topic = await topicRepository.GetAsync(x => x.Id == request.TopicId,
                isEnabledTracking: true,
                include: x => x.Include(x => x.CoSupervisors),
                orderBy: null,
                cancellationToken);

            ArgumentNullException.ThrowIfNull(topic);

            var coSupervisor = topic.CoSupervisors.FirstOrDefault(x => x.SupervisorId == request.SupervisorId);

            if (coSupervisor == null)
                return OperationResult.Success();

            await unitOfWork.BeginTransactionAsync(cancellationToken);

            topic.CoSupervisors.Remove(coSupervisor);

            topicRepository.Update(topic);

            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Fail to remove CoSupervisor {SupCode} with topic {TopicId} with error {Message}",
                request.SupervisorId, request.TopicId, ex.Message);

            await unitOfWork.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("Topic.Error", "Fail to remove coSupervisor for this topic"));
        }
    }

    public async Task<OperationResult<Topic>> GetTopicByGroupIdAsync(Guid groupId)
    {
        var topic = await topicRepository.GetAsync(t => t.Group.Id == groupId, include: t =>
            t.Include(t => t.Group)
                .ThenInclude(g => g.GroupMembers.Where(gm => gm.Status == GroupMemberStatus.Accepted))
                .Include(t => t.CoSupervisors));

        return topic ?? OperationResult.Failure<Topic>(Error.NullValue);
    }

    private async Task<float?> GetAverageGPAOfGroupByStudent(string studentId, CancellationToken cancellationToken)
    {
        var groupMember = await groupMemberRepository.GetAsync(
            x => x.StudentId == studentId && x.Status == GroupMemberStatus.Accepted,
            include: x => x.Include(x => x.Group),
            orderBy: null,
            cancellationToken);

        ArgumentNullException.ThrowIfNull(groupMember);

        return groupMember.Group.GPA;
    }
}
