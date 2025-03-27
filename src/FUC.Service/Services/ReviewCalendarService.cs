using System.Linq.Expressions;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using FUC.Common.Abstractions;
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
using MassTransit.Internals;
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
    IRepository<ReviewCalendar> reviewCalendarRepository) : IReviewCalendarService
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

        try
        {
            var reviewCalendars = await ParseReviewCalendarsFromFile(file, currentSemester.Value.Id);
            reviewCalendarRepository.InsertRange(reviewCalendars);
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
        return reviewCalendars.Count > 0
            ? reviewCalendars
            : OperationResult.Failure<IEnumerable<ReviewCalendarResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<ReviewCalendarResponse>>> GetReviewCalendarBySupervisorId()
    {
        var reviewCalendars = await reviewerRepository.FindAsync(
            r => r.SupervisorId.Equals(currentUser.UserCode),
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
                Slot = r.ReviewCalender.Slot,
                GroupId = r.ReviewCalender.GroupId,
                GroupCode = r.ReviewCalender.Group.GroupCode,
                TopicId = r.ReviewCalender.TopicId,
                TopicCode = r.ReviewCalender.Topic.Code!,
                TopicEnglishName = r.ReviewCalender.Topic.EnglishName,
                MainSupervisorCode = r.ReviewCalender.Topic.MainSupervisorId,
                ReviewersCode = r.ReviewCalender.Reviewers.Select(r => r.SupervisorId).ToList(),
                CoSupervisorsCode = r.ReviewCalender.Topic.CoSupervisors.Select(c => c.SupervisorId).ToList(),
                Comment = r.ReviewCalender.Reviewers.Select(r => r.Comment).ToList(),
                Suggestion = r.ReviewCalender.Reviewers.Select(r => r.Suggestion).ToList()
            });
        return reviewCalendars.Count > 0
            ? reviewCalendars.ToList()
            : OperationResult.Failure<IEnumerable<ReviewCalendarResponse>>(Error.NullValue);
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
        return reviewCalendars.Count > 0
            ? reviewCalendars.ToList()
            : OperationResult.Failure<IEnumerable<ReviewCalendarResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<ReviewCalendarResponse>>> GetReviewCalendarByManagerId()
    {
        var reviewCalendars = await reviewCalendarRepository.FindAsync(rc => rc.CreatedBy.Equals(currentUser.Email),
            CreateIncludeForReviewCalendarResponse(),
            rc => rc.OrderBy(rc => rc.CreatedDate),
            CreateReviewCalendarSelector());
        return reviewCalendars.Count > 0
            ? reviewCalendars.ToList()
            : OperationResult.Failure<IEnumerable<ReviewCalendarResponse>>(Error.NullValue);
    }

    public async Task<OperationResult> UpdateReviewCalendar(UpdateReviewerSuggestionAndCommentRequest request)
    {
        try
        {
            var reviewer = await reviewerRepository.GetAsync(
                r => r.SupervisorId == currentUser.UserCode && r.ReviewCalenderId == request.ReviewCalenderId,
                true,
                include: r => r.Include(rc => rc.ReviewCalender));
            if (IsReviewerValidToEnterSuggestionAndComment(reviewer))
                return OperationResult.Failure(Error.NullValue);
            reviewer!.Suggestion = request.Suggestion;
            reviewer.Comment = request.Comment;
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


    #region private method

    private static bool IsReviewerValidToEnterSuggestionAndComment(Reviewer? reviewer)
    {
        return reviewer != null && reviewer.ReviewCalender.Status == ReviewCalendarStatus.InProgress &&
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

        var reviewDate = row.Cell(reviewerCol++).GetValue<string>();
        var slot = row.Cell(reviewerCol++).GetValue<string>();
        var room = row.Cell(reviewerCol).GetValue<string>();

        if (string.IsNullOrEmpty(reviewDate) || string.IsNullOrEmpty(slot) || string.IsNullOrEmpty(room))
        {
            logger.LogError("import review failed with message: review date, slot or room is empty!");
            throw new Exception("Invalid review details");
        }

        return new ReviewCalendarDetail()
        {
            ReviewersId = reviewers,
            Date = DateTime.Parse(reviewDate),
            Slot = int.Parse(slot),
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
            Slot = reviewDetail.Slot,
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
            Slot = calendar.Slot,
            GroupId = calendar.GroupId,
            GroupCode = calendar.Group.GroupCode,
            TopicId = calendar.TopicId,
            TopicCode = calendar.Topic.Code!,
            TopicEnglishName = calendar.Topic.EnglishName,
            MainSupervisorCode = calendar.Topic.MainSupervisorId,
            ReviewersCode = calendar.Reviewers.Select(r => r.SupervisorId).ToList(),
            CoSupervisorsCode = calendar.Topic.CoSupervisors.Select(c => c.SupervisorId).ToList(),
            Suggestion = calendar.Reviewers.Select(x => x.Suggestion).ToList(),
            Comment = calendar.Reviewers.Select(x => x.Comment).ToList(),
        };

    private Func<IQueryable<ReviewCalendar>, IIncludableQueryable<ReviewCalendar, object>>
        CreateIncludeForReviewCalendarResponse()
        => rc =>
            rc.Include(rc => rc.Group)
                .Include(rc => rc.Topic).ThenInclude(t => t.CoSupervisors)
                .Include(rc => rc.Reviewers);

    #endregion
}
