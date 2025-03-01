using System.Text.Json;
using Amazon.S3;
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
using FUC.Service.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FUC.Service.Services;

public class TopicService(
    ILogger<TopicService> logger,
    IS3Service s3Service,
    ICurrentUser currentUser,
    IUnitOfWork<FucDbContext> unitOfWork,
    IRepository<Topic> topicRepository,
    IRepository<TopicAppraisal> topicAppraisalRepository,
    IRepository<TopicAnalysis> topicAnalysisRepository,
    IRepository<Supervisor> supervisorRepository,
    IRepository<Capstone> capstoneRepository,
    IRepository<BusinessArea> businessRepository,
    S3BucketConfiguration s3BucketConfiguration,
    ISemesterService semesterService,
    ICacheService cache,
    IIntegrationEventLogService integrationEventLogService,
    TopicAppraisalFilterFactory topicAppraisalFilterFactory) : ITopicService
{
    private const int MaxTopicsForCoSupervisors = 3;

    public async Task<OperationResult<IList<TopicResponse>>> GetTopicsBySupervisor()
    {
        var topics = await topicRepository.FindAsync(
            x => x.MainSupervisorId == currentUser.Id,
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
                MainSupervisorEmail = x.MainSupervisor.Email,
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
                CapstoneId = x.CapstoneId,
                CoSupervisors = x.CoSupervisors.Select(x => new CoSupervisorDto
                {
                    SupervisorEmail = x.Supervisor.Email,
                    SupervisorName = x.Supervisor.FullName,
                }).ToList(),
                CreatedDate = x.CreatedDate,
            });

        return OperationResult.Success(topics);
    }

    public async Task<OperationResult<PaginatedList<TopicResponse>>> GetTopics(TopicRequest request)
    {
        var topics = await topicRepository.FindPaginatedAsync(
            x => (request.MainSupervisorEmail == "all" || 
                    x.MainSupervisor.Email == request.MainSupervisorEmail) &&
                 (string.IsNullOrEmpty(request.SearchTerm) ||
                  x.Code != null && x.Code.Contains(request.SearchTerm.Trim()) ||
                  x.EnglishName.Contains(request.SearchTerm.Trim()) ||
                  x.VietnameseName.Contains(request.SearchTerm.Trim()) ||
                  x.Abbreviation.Contains(request.SearchTerm.Trim()) ||
                  x.Description.Contains(request.SearchTerm.Trim())) &&
                 (request.BusinessAreaId == "all" || 
                    x.BusinessAreaId == Guid.Parse(request.BusinessAreaId)) &&
                 (request.DifficultyLevel == "all" || 
                    x.DifficultyLevel == Enum.Parse<DifficultyLevel>(request.DifficultyLevel, true))  &&
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
                Status = x.Status,
                DifficultyLevel = x.DifficultyLevel,
                BusinessAreaName = x.BusinessArea.Name,
                CampusId = x.CampusId,
                SemesterId = x.SemesterId,
                CapstoneId = x.CapstoneId,
                CoSupervisors = x.CoSupervisors.Select(x => new CoSupervisorDto
                {
                    SupervisorEmail = x.Supervisor.Email,
                    SupervisorName = x.Supervisor.FullName,
                }).ToList(),
                CreatedDate = x.CreatedDate,
            });

        return OperationResult.Success(topics);
    }

    public async Task<OperationResult> SemanticTopic(Guid topicId, bool withCurrentSemester,
        CancellationToken cancellationToken)
    {
        try
        {
            var key = $"processing/{topicId.ToString()}";
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
                TopicId = topicId.ToString(),
                IsCurrentSemester = withCurrentSemester,
                SemesterIds = semesterIds,
                ProcessedBy = currentUser.Email,
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

        if (request.CoSupervisorEmails.Any(x => x == currentUser.Email))
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
                $"{currentUser.CampusId}/{getCurrentSemesterResult.Value.Id}/{request.CapstoneId}/Pending/{topicId}";

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
                SemesterIds = await semesterService.GetPreviouseSemesterIds(getCurrentSemesterResult.Value.StartDate),
                ProcessedBy = currentUser.Email,
                IsCurrentSemester = false
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

            var analysis = JsonSerializer.Deserialize<SemanticResponse>(item.AnalysisResult);

            var analysisResponse = new List<TopicAnalysisResponse>();

            foreach (var a in analysis!.MatchingTopics)
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
            });
        }

        return OperationResult.Success(result);
    }

    public async Task<OperationResult> CreateTopicAppraisal(IReadOnlyList<string> supervisorEmail)
    {
        //TODO: Check the valid date to assign supervisors to topics (TimeCongig table)

        var currentSemester = await semesterService.GetCurrentSemesterAsync();
        if (currentSemester.IsFailure)
        {
            logger.LogError("The current semester is inactive !");
            return OperationResult.Failure(new Error("Error.CurrentSemesterIsNull", " Current semester is inactive !"));
        }

        if (supervisorEmail.Count < TopicAppraisalRequirement.SupervisorAppraisalMinimum)
        {
            logger.LogError("The Supervisor Appraisal size is invalid !");
            return OperationResult.Failure(new Error("Error.InvalidSupervisorAppraisalSize",
                $"The Supervisor appraisal must be greater than {TopicAppraisalRequirement.SupervisorAppraisalMinimum}"));
        }

        var supervisorIdList = await (from s in supervisorRepository.GetQueryable()
            where supervisorEmail.Contains(s.Email) &&
                  s.IsAvailable &&
                  s.MajorId.Equals(currentUser.MajorId) &&
                  s.CampusId.Equals(currentUser.CampusId)
            select s.Id).ToListAsync();
        //check if supervisor list is null or empty!
        if (supervisorIdList.Count < 1)
        {
            logger.LogError("Supervisors was not found !");
            return OperationResult.Failure(Error.NullValue);
        }

        IList<Topic> topicList = await topicRepository
            .FindAsync(t => t.Status.Equals(TopicStatus.Pending) &&
                            t.CampusId.Equals(currentUser.CampusId) &&
                            t.SemesterId.Equals(currentSemester.Value.Id),
                t => t
                    .Include(t => t.MainSupervisor)
                    .Include(t => t.CoSupervisors)
                    .Include(t => t.Capstone)
                    .Include(t => t.TopicAppraisals));

        // get topic list based on current manager major
        topicList = topicList.Where(t => t.Capstone.MajorId.Equals(currentUser.MajorId) &&
                                         (t.TopicAppraisals.Count == 0 || t.TopicAppraisals == null)).ToList();

        // check if topics list is null or empty!
        if (topicList.Count < 1)
        {
            logger.LogError($"Topics with status Pending was not found !");
            return OperationResult.Failure(Error.NullValue);
        }

        // Shuffle the supervisor id list 
        var rand = new Random();
        var shuffledSupervisorIdList = supervisorIdList.OrderBy(_ => rand.Next()).ToList();
        int index = 0;
        int supervisorCount = shuffledSupervisorIdList.Count;

        foreach (Topic topic in topicList)
        {
            var selectedSupervisorIdList = new List<string>();
            index = 0;
            while (selectedSupervisorIdList.Count < 2)
            {
                if (index >= supervisorCount)
                {
                    break;
                }

                string selectedSupervisorId = shuffledSupervisorIdList[index++]!;

                if (!string.IsNullOrEmpty(selectedSupervisorId) &&
                    !selectedSupervisorId.Equals(topic.MainSupervisorId) &&
                    !selectedSupervisorIdList.Contains(selectedSupervisorId))
                {
                    selectedSupervisorIdList.Add(selectedSupervisorId);
                }
            }

            var topicAppraisalList = selectedSupervisorIdList.Select(s => new TopicAppraisal
            {
                SupervisorId = s.ToString(),
                TopicId = topic.Id
            }).ToList();

            topicAppraisalRepository.InsertRange(topicAppraisalList);
        }

        await unitOfWork.SaveChangesAsync();

        return OperationResult.Success();
    }

    public async Task<OperationResult<List<TopicAppraisalResponse>>> GetTopicAppraisalByUserId(
        TopicAppraisalBaseRequest request)
    {
        var query = topicAppraisalRepository.GetQueryable();

        ITopicAppraisalFilterStrategy filterStrategy =
            topicAppraisalFilterFactory.GetStrategy(request, currentUser.Role);

        query = filterStrategy.ApplyFilter(query, currentUser.UserCode).Include(ta => ta.Topic);

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
                "_asc" => query.OrderBy(ta => ta.CreatedDate),
                _ => query.OrderByDescending(ta => ta.CreatedDate)
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
            ManagerId = ta.ManagerId,
            TopicId = ta.TopicId,
            TopicEnglishName = ta.Topic.EnglishName
        }).ToListAsync();

        return response.Count < 1
            ? OperationResult.Failure<List<TopicAppraisalResponse>>(Error.NullValue)
            : OperationResult.Success(response);
    }

    private async Task<List<(string SupervisorId, string SupervisorEmail)>>
        GetAvailableSupervisorsForSupportingTopic(List<string> coSupervisorEmails,
            CancellationToken cancellationToken)
    {
        var result = new List<(string, string)>();

        var query = from s in supervisorRepository.GetQueryable()
            where coSupervisorEmails.Contains(s.Email) &&
                  s.CoSupervisors.Count < MaxTopicsForCoSupervisors
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

        if (businessAreas.Count != 0)
        {
            return OperationResult.Success(businessAreas);
        }

        return OperationResult.Failure<List<BusinessAreaResponse>>(Error.NullValue);
    }
}
