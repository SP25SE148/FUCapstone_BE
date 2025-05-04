using System.Linq.Expressions;
using ClosedXML.Excel;
using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.IntegrationEventLog.Services;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Enums;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.GroupDTO;
using FUC.Service.DTOs.ReviewCalendarDTO;
using FUC.Service.DTOs.TopicDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

namespace FUC.Service.Services;

public sealed class ReviewCalendarService(
    IUnitOfWork<FucDbContext> uow,
    ILogger<ReviewCalendarService> logger,
    ISemesterService semesterService,
    IGroupService groupService,
    ITopicService topicService,
    IRepository<Reviewer> reviewerRepository,
    ISupervisorService supervisorService,
    ICurrentUser currentUser,
    ISystemConfigurationService systemConfigurationService,
    IRepository<ReviewCriteria> reviewCriteriaRepository,
    IRepository<ReviewCalendar> reviewCalendarRepository,
    ISystemConfigurationService systemConfigService,
    IIntegrationEventLogService integrationEventLogService) : IReviewCalendarService
{
    public async Task<OperationResult> ImportReviewCalendar(IFormFile file)
    {
        // check if file is valid
        if (!IsValidFile(file))
            return OperationResult.Failure(new Error("Error.ImportFailed", "Invalid file"));
        // get current semester 
        var currentSemester = await semesterService.GetCurrentSemesterAsync();
        if (currentSemester.IsFailure)
            return OperationResult.Failure(new Error("Error.SemesterIsNotGoingOn",
                "The current semester is not going on"));
        var timeConfiguration = currentSemester.Value.TimeConfigurations?
            .FirstOrDefault(
                s => s.SemesterId == currentSemester.Value.Id &&
                     s.CampusId == currentUser.CampusId);

        if (timeConfiguration != null &&
            timeConfiguration.IsActived &&
            (timeConfiguration.ReviewAttemptDate > DateTime.Now
             || timeConfiguration.ReviewAttemptExpiredDate < DateTime.Now))
            return OperationResult.Failure<Guid>(new Error("CreateFailed",
                "Must import the review for group on available time. The time that you can import the review calendar file is from " +
                timeConfiguration.ReviewAttemptDate + " to " +
                timeConfiguration.ReviewAttemptExpiredDate));

        try
        {
            var reviewCalendars =
                await ParseReviewCalendarsFromFile(file, currentSemester.Value.Id);

            reviewCalendarRepository.InsertRange(reviewCalendars);

            // send review calendar created event
            var calendarCreatedDetails = new List<CalendarCreatedDetail>();

            foreach (var reviewCalendar in reviewCalendars)
            {
                calendarCreatedDetails.Add(new CalendarCreatedDetail()
                {
                    CalendarId = reviewCalendar.Id,
                    StartDate = reviewCalendar.Date,
                    Users = reviewCalendar.Reviewers.Select(x => x.SupervisorId).ToList(),
                    Type = nameof(ReviewCalendar)
                });
            }

            integrationEventLogService.SendEvent(new CalendarCreatedEvent
            {
                Details = calendarCreatedDetails
            });

            await uow.SaveChangesAsync();
            return OperationResult.Success();
        }
        catch (Exception e)
        {
            logger.LogError("import review failed with message: {Message}", e.Message);
            return OperationResult.Failure(new Error("Error.ImportFailed", "import review failed"));
        }
    }

    public async Task<OperationResult<IEnumerable<ReviewCalendarResponse>>> GetReviewCalendars()
    {
        var reviewCalendars = await reviewCalendarRepository.GetAllAsync(
            CreateIncludeForReviewCalendarResponse(),
            rc => rc.OrderBy(rc => rc.CreatedDate),
            CreateReviewCalendarSelector());
        return reviewCalendars;
    }

    public async Task<OperationResult<IEnumerable<ReviewCalendarResponse>>> GetReviewCalendarBySupervisorId()
    {
        var reviewCalendars = await reviewerRepository.FindAsync(
            r => r.SupervisorId == currentUser.UserCode,
            r => r.AsSplitQuery()
                .Include(r => r.ReviewCalender)
                .ThenInclude(rc => rc.Topic)
                .ThenInclude(t => t.CoSupervisors)
                .Include(r => r.ReviewCalender.Reviewers),
            r => r.OrderBy(r => r.ReviewCalender.CreatedDate),
            selector: r => new ReviewCalendarResponse()
            {
                Id = r.ReviewCalender.Id,
                Attempt = r.ReviewCalender.Attempt,
                Date = r.ReviewCalender.Date,
                Room = r.ReviewCalender.Room,
                Time = r.ReviewCalender.Time,
                GroupId = r.ReviewCalender.GroupId,
                GroupCode = r.ReviewCalender.Group.GroupCode,
                TopicId = r.ReviewCalender.TopicId,
                TopicCode = r.ReviewCalender.Topic.Code!,
                TopicEnglishName = r.ReviewCalender.Topic.EnglishName,
                MainSupervisorCode = r.ReviewCalender.Topic.MainSupervisorId,
                CoSupervisorsCode = r.ReviewCalender.Topic.CoSupervisors.Select(c => c.SupervisorId).ToList(),
                Status = r.ReviewCalender.Status.ToString(),
                Reviewers = r.ReviewCalender.Reviewers.Select(r => r.Supervisor.FullName).ToList()
            });
        return reviewCalendars.ToList();
    }

    public async Task<OperationResult<IEnumerable<ReviewCalendarResponse>>> GetReviewCalendarByStudentId()
    {
        var groupOfCurrentUser = await groupService.GetGroupInformationByGroupSelfId();
        if (groupOfCurrentUser.IsFailure)
            return OperationResult.Failure<IEnumerable<ReviewCalendarResponse>>(Error.NullValue);

        var reviewCalendars = await reviewCalendarRepository.FindAsync(
            rc => rc.GroupId.Equals(groupOfCurrentUser.Value.Id),
            CreateIncludeForReviewCalendarResponse(),
            rc => rc.OrderByDescending(rc => rc.Attempt),
            CreateReviewCalendarSelector());
        return reviewCalendars.ToList();
    }

    public async Task<OperationResult<IEnumerable<ReviewCalendarResponse>>> GetReviewCalendarByManagerId()
    {
        var reviewCalendars = await reviewCalendarRepository.FindAsync(rc => rc.CreatedBy.Equals(currentUser.Email),
            CreateIncludeForReviewCalendarResponse(),
            rc => rc.OrderBy(rc => rc.CreatedDate),
            CreateReviewCalendarSelector());
        return reviewCalendars.ToList();
    }

    public async Task<OperationResult> UpdateReviewCalendar(UpdateReviewerSuggestionAndCommentRequest request)
    {
        try
        {
            var reviewer = await reviewerRepository.GetAsync(
                r => r.SupervisorId == currentUser.UserCode && r.ReviewCalenderId == request.ReviewCalenderId,
                true,
                include: r => r.AsSplitQuery()
                    .Include(rc => rc.ReviewCalender).ThenInclude(rc => rc.Reviewers));
            if (!IsReviewerValidToEnterSuggestionAndComment(reviewer))
                return OperationResult.Failure(new Error("UpdateReviewFailed",
                    "You can not update this review suggestion and comment"));
            reviewer!.Suggestion = request.Suggestion;
            reviewer.Comment = request.Comment;
            reviewer.IsReview = true;
            if (reviewer.ReviewCalender.Reviewers.All(r => r.IsReview))
                reviewer.ReviewCalender.Status = ReviewCalendarStatus.Done;
            await uow.SaveChangesAsync();
            return OperationResult.Success();
        }
        catch (Exception e)
        {
            logger.LogError("update reviewer suggestion and comment failed with message: {Message}", e.Message);
            return OperationResult.Failure(new Error("Error.UpdateFailed",
                "update reviewer suggestion and comment failed"));
        }
    }

    public async Task<OperationResult<IEnumerable<ReviewCalendarResultResponse>>> GetReviewCalendarResultByStudentId()
    {
        var group = await groupService.GetGroupInformationByGroupSelfId();
        if (group.IsFailure)
            return OperationResult.Failure<IEnumerable<ReviewCalendarResultResponse>>(Error.NullValue);
        var reviewCalendar = await reviewCalendarRepository.FindAsync(rc => rc.GroupId == group.Value.Id,
            include: rc => rc.Include(rc => rc.Reviewers),
            orderBy: rc => rc.OrderBy(rc => rc.Attempt));
        var reviewCalendarResultResponse = MapReviewCalendarsToResponses(reviewCalendar);
        return reviewCalendarResultResponse.ToList();
    }

    public async Task<OperationResult<IEnumerable<ReviewCalendarResultResponse>>>
        GetReviewCalendarResultByGroupId(Guid groupId)
    {
        var group = await groupService.GetGroupByIdAsync(groupId);
        var reviewCalendar = await reviewCalendarRepository.FindAsync(rc => rc.GroupId == group.Value.Id,
            include: rc => rc.Include(rc => rc.Reviewers),
            orderBy: rc => rc.OrderBy(rc => rc.Attempt));

        var reviewCalendarResultResponse = MapReviewCalendarsToResponses(reviewCalendar);
        return reviewCalendarResultResponse.ToList();
    }

    public async Task<OperationResult<IEnumerable<ReviewCalendarResultResponse>>> GetReviewCalendarResultByManagerId()
    {
        var reviewCalendar = await reviewCalendarRepository.FindAsync(rc => rc.CreatedBy == currentUser.Email,
            include: rc => rc.Include(rc => rc.Reviewers),
            orderBy: rc => rc.OrderBy(rc => rc.Attempt));
        var reviewCalendarResultResponse = MapReviewCalendarsToResponses(reviewCalendar);
        return reviewCalendarResultResponse.ToList();
    }

    public async Task<OperationResult> UpdateReviewCalendarStatus(UpdateReviewCalendarStatusRequest request)
    {
        var reviewCalendar = await reviewCalendarRepository.GetAsync(
            x => x.Id == request.Id,
            true);
        if (reviewCalendar is null)
            return OperationResult.Failure(Error.NullValue);

        reviewCalendar.Status = request.Status;
        await uow.SaveChangesAsync();
        return OperationResult.Success();
    }

    public async Task<OperationResult<IEnumerable<ReviewCriteriaResponse>>> GetReviewCriteriaByAttemptAsync(int attempt)
    {
        var reviewCriteria = await reviewCriteriaRepository.FindAsync(
            rc => rc.Attempt == attempt && rc.IsActive,
            null,
            null,
            selector: rc => new ReviewCriteriaResponse
            {
                Id = rc.Id,
                Attempt = rc.Attempt,
                Description = rc.Description,
                Name = rc.Name,
                Requirement = rc.Requirement
            });
        return reviewCriteria.ToList();
    }


    #region private method

    private IEnumerable<ReviewCalendarResultResponse> MapReviewCalendarsToResponses(
        IEnumerable<ReviewCalendar> reviewCalendars)
    {
        return reviewCalendars.Select(rc => new ReviewCalendarResultResponse
        {
            Attempt = rc.Attempt,
            ReviewCalendarResultDetailList = rc.Reviewers.Select(r => new ReviewCalendarResultDetailResponse
            {
                Suggestion = r.Suggestion ?? "undefined",
                Comment = r.Comment ?? "undefined",
                IsReview = r.IsReview,
                Author = r.SupervisorId
            }).ToList()
        });
    }

    private static bool IsReviewerValidToEnterSuggestionAndComment(Reviewer? reviewer)
    {
        return reviewer != null &&
               reviewer.ReviewCalender.Status == ReviewCalendarStatus.InProgress &&
               reviewer.IsReview == false &&
               reviewer.ReviewCalender.Date.Date == DateTime.Now.Date;
    }

    private async Task<List<ReviewCalendar>> ParseReviewCalendarsFromFile(IFormFile file, string currentSemester)
    {
        var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        using var wb = new XLWorkbook(stream);
        IXLWorksheet workSheet = wb.Worksheet(1);

        var reviewCalendars = new List<ReviewCalendar>();

        // Get the next attempt number
        var existingCalendars = await reviewCalendarRepository.GetAllAsync();
        var attempt = existingCalendars.Any() ? existingCalendars.Max(rc => rc.Attempt) + 1 : 1;
        if (attempt > systemConfigurationService.GetSystemConfiguration().MaxAttemptTimesToReviewTopic)
            throw new Exception("Max attempt times to review topic");

        foreach (var row in workSheet.Rows().Skip(5))
        {
            var groupCode = row.Cell(2).GetValue<string>();
            var topicCode = row.Cell(3).GetValue<string>();
            // Validate group code and topic code
            if (string.IsNullOrEmpty(groupCode) || string.IsNullOrEmpty(topicCode))
            {
                break;
            }

            var group = await groupService.GetGroupByGroupCodeAsync(groupCode);
            var topic = await topicService.GetTopicByTopicCode(topicCode);

            if (group.IsFailure ||
                group.Value.SemesterName != currentSemester ||
                group.Value.CampusName != currentUser.CampusId ||
                group.Value.MajorName != currentUser.MajorId ||
                topic == null ||
                topic.Code != group.Value.TopicCode)
            {
                throw new Exception("Invalid group or topic");
            }

            await IsGroupIsExistInReviewCalendarInCurrentAttempt(reviewCalendars, group.Value, attempt);

            var reviewersCalendarDetail = await GetReviewCalendarDetailsFromRow(row, workSheet);
            var reviewCalendar =
                CreateReviewCalendar(group.Value, topic, attempt, currentSemester, reviewersCalendarDetail);
            reviewCalendars.Add(reviewCalendar);
        }

        return reviewCalendars;
    }

    private async Task IsGroupIsExistInReviewCalendarInCurrentAttempt(List<ReviewCalendar> reviewCalendars,
        GroupResponse group, int attempt)
    {
        if (await reviewCalendarRepository.GetAsync(x => x.GroupId == group.Id &&
                                                         x.Attempt == attempt, default) != null ||
            reviewCalendars.Exists(rc => rc.GroupId == group.Id))
        {
            throw new Exception("Group is already exist in review calendar in current attempt");
        }
    }


    private async Task<ReviewCalendarDetail> GetReviewCalendarDetailsFromRow(IXLRow row, IXLWorksheet workSheet)
    {
        var reviewers = new List<string>();
        int reviewerCol = 9;
        for (; reviewerCol < row.Cells().Count(); reviewerCol++)
        {
            var isReviewerColumn = workSheet.Cell(5, reviewerCol).GetValue<string>().Contains("Reviewer");
            if (!isReviewerColumn)
                break;

            var reviewerCode = row.Cell(reviewerCol).GetValue<string>();
            var reviewer = await supervisorService.GetSupervisorByIdAsync(reviewerCode);
            if (reviewer.IsFailure)
            {
                logger.LogError("import review failed with message: reviewer with code: {ReviewerCode} is not found!",
                    reviewerCode);
                return new ReviewCalendarDetail();
            }

            reviewers.Add(reviewerCode);
        }

        var reviewDate = row.Cell(6).GetValue<string>();
        var time = row.Cell(7).GetValue<string>();
        var room = row.Cell(8).GetValue<string>();

        if (string.IsNullOrEmpty(reviewDate) || string.IsNullOrEmpty(time) || string.IsNullOrEmpty(room))
        {
            logger.LogError("import review failed with message: review date, slot or room is empty!");
            throw new Exception("Invalid review details");
        }

        // Validate review date
        if (DateTime.TryParse(reviewDate, out var date) == false)
            // && date.Date < DateTime.Now.Date)
        {
            throw new Exception("Review date must be in the future and in correct format");
        }

        return new ReviewCalendarDetail()
        {
            ReviewersId = reviewers,
            Date = date,
            Time = time,
            Room = room
        };
    }

    private ReviewCalendar CreateReviewCalendar(GroupResponse group, TopicResponse topic, int defendAttempt,
        string currentSemester, ReviewCalendarDetail reviewDetail)
    {
        var reviewCalendar = new ReviewCalendar
        {
            Id = Guid.NewGuid(),
            TopicId = Guid.Parse(topic.Id),
            GroupId = group.Id,
            MajorId = currentUser.MajorId,
            CampusId = currentUser.CampusId,
            SemesterId = currentSemester,
            Attempt = defendAttempt,
            Time = reviewDetail.Time,
            Room = reviewDetail.Room,
            Date = reviewDetail.Date
        };

        reviewCalendar.Reviewers = reviewDetail.ReviewersId.Select(r => new Reviewer
        {
            Id = Guid.NewGuid(),
            ReviewCalenderId = reviewCalendar.Id,
            SupervisorId = r // r is supervisor id
        }).ToList();

        return reviewCalendar;
    }

    private static bool IsValidFile(IFormFile file)
    {
        return file != null && file.Length > 0 &&
               file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase);
    }

    private Expression<Func<ReviewCalendar, ReviewCalendarResponse>> CreateReviewCalendarSelector()
        => calendar => new ReviewCalendarResponse()
        {
            Id = calendar.Id,
            Attempt = calendar.Attempt,
            Date = calendar.Date,
            Room = calendar.Room,
            Time = calendar.Time,
            GroupId = calendar.GroupId,
            GroupCode = calendar.Group.GroupCode,
            TopicId = calendar.TopicId,
            TopicCode = calendar.Topic.Code!,
            TopicEnglishName = calendar.Topic.EnglishName,
            MainSupervisorCode = calendar.Topic.MainSupervisorId,
            CoSupervisorsCode = calendar.Topic.CoSupervisors.Select(c => c.SupervisorId).ToList(),
            Status = calendar.Status.ToString(),
            Reviewers = calendar.Reviewers.Select(r => r.Supervisor.FullName).ToList()
        };

    private Func<IQueryable<ReviewCalendar>, IIncludableQueryable<ReviewCalendar, object>>
        CreateIncludeForReviewCalendarResponse()
        => rc =>
            rc.Include(rc => rc.Group)
                .Include(rc => rc.Topic).ThenInclude(t => t.CoSupervisors)
                .Include(rc => rc.Reviewers).ThenInclude(r => r.Supervisor);

    #endregion
}
