using System.Security.Claims;
using AutoMapper;
using ClosedXML.Excel;
using FUC.Common.Abstractions;
using FUC.Common.Constants;
using FUC.Common.Contracts;
using FUC.Common.IntegrationEventLog.Services;
using FUC.Common.Shared;
using Identity.API.Data;
using Identity.API.Models;
using Identity.API.Payloads.Requests;
using Identity.API.Payloads.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Identity.API.Controllers;

public class UsersController(ILogger<UsersController> logger,
    ICurrentUser currentUser,
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext dbContext,
    IIntegrationEventLogService eventService,
    IMapper mapper) : ApiController
{
    private const int BatchSize = 200;

    [HttpPost("admins")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin}")]
    public async Task<IActionResult> CreateAdmin([FromBody] AdminDto user)
    {
        await CreateApplicationUser(new ApplicationUser
        {
            UserCode = user.Email.Split("@")[0],
            FullName = user.FullName,
            Email = user.Email,
            UserName = user.Email,
            CampusId = user.CampusId,
            CapstoneId = "all",
            MajorId = "all",
            EmailConfirmed = true
        }, UserRoles.Admin);

        return Ok(OperationResult.Success());
    }

    [HttpPost("managers")]
    [Authorize(Roles = $"{UserRoles.Admin}")]
    public async Task<IActionResult> CreateManager([FromBody] ManagerDto user)
    {
        await CreateApplicationUser(new ApplicationUser
        {
            UserCode = user.Email.Split("@")[0],
            FullName = user.FullName,
            Email = user.Email,
            UserName = user.Email,
            CampusId = currentUser.CampusId,
            CapstoneId = user.CapstoneId,
            MajorId = "all",
            EmailConfirmed = true,
        }, UserRoles.Manager);

        return Ok(OperationResult.Success());
    }

    [HttpPost("supervisors")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager}")]
    public async Task<IActionResult> CreateSupervisors([FromBody] SupervisorDto user)
    {
        await dbContext.BeginTransactionAsync();

        var supervisor = new ApplicationUser
        {
            UserCode = user.Email.Split("@")[0],
            FullName = user.FullName,
            Email = user.Email,
            UserName = user.Email,
            CampusId = currentUser.CampusId,
            CapstoneId = "all",
            MajorId = user.MajorId,
            EmailConfirmed = true
        };

        await CreateApplicationUser(supervisor, UserRoles.Supervisor);

        SyncUsersToFUCService(new List<UserSync>{ mapper.Map<UserSync>(supervisor) }, 
            UserRoles.Supervisor, 
            User.FindFirst(ClaimTypes.Email)!.Value, 1, 1);

        await dbContext.CommitTransactionAsync();

        return Ok(OperationResult.Success());
    }

    [HttpPost("students")]
    [Authorize(Roles = $"{UserRoles.Admin},{UserRoles.Manager}")]
    public async Task<IActionResult> CreateStudent([FromBody] StudentDto user)
    {
        await dbContext.BeginTransactionAsync();

        var student = new ApplicationUser
        {
            UserCode = user.StudentCode,
            FullName = user.FullName,
            Email = user.Email,
            UserName = user.Email,
            CampusId = currentUser.CampusId,
            CapstoneId = user.CapstoneId,
            MajorId = user.MajorId,
            EmailConfirmed = true
        };

        await CreateApplicationUser(student, UserRoles.Student);

        SyncUsersToFUCService(new List<UserSync> { mapper.Map<UserSync>(student) }, 
            UserRoles.Student, 
            User.FindFirst(ClaimTypes.Email)!.Value, 1, 1);

        await dbContext.CommitTransactionAsync();

        return Ok(OperationResult.Success());
    }

    [HttpPost("test/bus/{i}")]
    public async Task<IActionResult> TestBus(int i)
    {
        logger.LogInformation("Test publish message into queue with EventService");

        await dbContext.BeginTransactionAsync();

        var user = new ApplicationUser
        {
            UserCode = $"Test{i}",
            FullName = $"Test{i}",
            UserName = $"test{i}@fpt.edu.vn",
            Email = $"test{i}@fpt.edu.vn",
            MajorId = "SE",
            CapstoneId = "SEP490",
            CampusId = "HCM",
            EmailConfirmed = true,
        };

        await CreateApplicationUser(user, UserRoles.Student);

        eventService.SendEvent(new UsersSyncMessage
        {
            AttempTime = 1,
            UserType = "Test",
            UsersSync = new List<UserSync> { mapper.Map<UserSync>(user) },
            CreatedBy = "test"
        });

        await dbContext.CommitTransactionAsync();

        return Ok();
    }

    [HttpPost("import/students")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.Admin},{UserRoles.Manager}")]
    public async Task<IActionResult> ImportStudents(IFormFile file)
    {
        var email = User.FindFirst(ClaimTypes.Email)!.Value;
        var result = await ImportProcessingtUsers(UserRoles.Student, file, email);
        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpPost("import/supervisors")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.Admin},{UserRoles.Manager}")]
    public async Task<IActionResult> ImportSupervisors(IFormFile file)
    {
        var result = await ImportProcessingtUsers(UserRoles.Supervisor, file,
            User.FindFirst(ClaimTypes.Email)!.Value);
        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("get-all-admin")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin}")]
    public async Task<IActionResult> GetAllAdminAsync()
    {
        
        var result = await GetUserInRoleAsync(UserRoles.Admin, currentUser.CampusId);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("get-all-manager")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.Admin}")]
    public async Task<IActionResult> GetAllManagerAsync()
    {
        var result = await GetUserInRoleAsync(UserRoles.Manager, currentUser.CampusId);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }
    private async Task<OperationResult> ImportProcessingtUsers(string userType, IFormFile file, string emailImporter)
    {
        logger.LogInformation("Start processing Users file");
        if (!IsValidFile(userType, file))
        {
            return OperationResult.Failure(new Error("File.Format", "File is wrong format"));
        }

        using var stream = new MemoryStream();
        {
            await file.CopyToAsync(stream);

            using XLWorkbook wb = new XLWorkbook(stream);
            {
                IXLWorksheet workSheet = wb.Worksheet(1);

                return await ProcessRows(workSheet, userType, emailImporter);
            }
        }
    }

    private async Task<OperationResult> ProcessRows(IXLWorksheet workSheet, string userType, string emailImporter)
    {
        var attempTime = 1;
        var numberOfUsersInBatchSize = 0;
        List<UserSync> users = new List<UserSync>();

        // Get campusCode in cell(B,2)
        var campusCode = workSheet.Cell(2, 2).GetValue<string>();
        if (string.IsNullOrEmpty(campusCode))
        {
            logger.LogError("Can not parse campusCode");
            return OperationResult.Failure(new Error("File.Format", "File is wrong format"));
        }

        await dbContext.BeginTransactionAsync();

        foreach (IXLRow row in workSheet.Rows().Skip(5))
        {
            // Check the end of table
            if (!row.Cell(2).TryGetValue<string>(out var usercode)
                || string.IsNullOrEmpty(usercode))
            {
                SyncUsersToFUCService(users, userType, emailImporter, numberOfUsersInBatchSize, attempTime);
                break;
            }

            var user = new ApplicationUser
            {
                UserCode = usercode,
                FullName = row.Cell(3).GetValue<string>(),
                UserName = row.Cell(4).GetValue<string>(),
                Email = row.Cell(4).GetValue<string>(),
                MajorId = row.Cell(5).GetValue<string>(),
                CapstoneId = row.Cell(6).GetValue<string>() is string value && !string.IsNullOrEmpty(value) ? value : "All",
                CampusId = campusCode,
                EmailConfirmed = true,
            };

            users.Add(mapper.Map<UserSync>(user));

            await CreateApplicationUser(user, userType);

            numberOfUsersInBatchSize++;

            if (numberOfUsersInBatchSize < BatchSize)
            {
                continue;
            }

            SyncUsersToFUCService(users, userType, emailImporter, numberOfUsersInBatchSize, attempTime++);
            users.Clear();
            numberOfUsersInBatchSize = 0;
        }

        await dbContext.CommitTransactionAsync();

        return OperationResult.Success();
    }

    private static bool IsValidFile(string userType, IFormFile file)
    {
        return file != null && file.Length > 0 &&
            file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) &&
            file.FileName.Contains(userType, StringComparison.OrdinalIgnoreCase);
    }

    private void SyncUsersToFUCService(List<UserSync> users, string userType, string emailImporter, int numberOfUsersSync, int attemptTime)
    {
        if (numberOfUsersSync > 0)
        {
            logger.LogInformation("{SyncCount} synced in attemp: {Time}", numberOfUsersSync, attemptTime);

            // Sync user into FUC service
            eventService.SendEvent(new UsersSyncMessage
            {
                AttempTime = attemptTime,
                UserType = userType,
                UsersSync = users,
                CreatedBy = emailImporter
            });
        }
    }

    private async Task CreateApplicationUser(ApplicationUser user, string role)
    {
        Log.Information("{User} - {role} created", user.UserCode, role);

        var result = await userManager.CreateAsync(user, role == "Student" ? "Pass123@" : "Pass123$");
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(result.Errors.First().Description);
        }

        result = await userManager.AddToRoleAsync(user, role);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(result.Errors.First().Description);
        }
    }

    private async Task<OperationResult<IEnumerable<UserResponseDTO>>> GetUserInRoleAsync(string role, string predicate)
    {
        IList<ApplicationUser> result = await userManager.GetUsersInRoleAsync(role);
        if (!predicate.ToLower().Equals("all"))
        {
            result = result.Where(a => a.CampusId.ToLower().Equals(predicate.ToLower())).ToList();
        }
        return result.ToList().Count > 0
            ? OperationResult.Success(mapper.Map<IEnumerable<UserResponseDTO>>(result.ToList()))
            : OperationResult.Failure<IEnumerable<UserResponseDTO>>(Error.NullValue);
    }
}
