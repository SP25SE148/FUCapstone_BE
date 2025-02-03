using System.Security.Claims;
using AutoMapper;
using ClosedXML.Excel;
using FUC.Common.Contracts;
using FUC.Common.Shared;
using Identity.API.Models;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Identity.API.Controllers;

public class UsersController(ILogger<UsersController> logger,
    UserManager<ApplicationUser> userManager,
    IPublishEndpoint publishEndpoint,
    IMapper mapper) : ApiController
{
    private const int BatchSize = 2;

    [HttpPost("import/students")]
    [Authorize(Roles = "Manager,Admin,SuperAdmin")]
    public async Task<IActionResult> ImportStudents(IFormFile file)
    {
        var email = User.FindFirst(ClaimTypes.Email)!.Value;
        var result = await ImporProcessingtUsers("Student", file, email);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    [HttpPost("import/supervisors")]
    [Authorize(Roles = "Manager,Admin,SuperAdmin")]
    public async Task<IActionResult> ImportSupervisors(IFormFile file)
    {
        var result = await ImporProcessingtUsers("Supervisor", file, 
            User.FindFirst(ClaimTypes.Email)!.Value);
        return result.IsSuccess ? Ok() : HandleFailure(result);
    }

    private async Task<OperationResult> ImporProcessingtUsers(string userType, IFormFile file, string emailImporter)
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

        foreach (IXLRow row in workSheet.Rows().Skip(4))
        {
            // Check the end of table
            if (!row.Cell(2).TryGetValue<string>(out var usercode)
                || string.IsNullOrEmpty(usercode))
            {
                await SyncUsersToFUCService(users, userType, emailImporter, numberOfUsersInBatchSize, attempTime);
                break;
            }

            var user = new ApplicationUser
            {
                UserCode = usercode,
                UserName = row.Cell(3).GetValue<string>(),
                Email = row.Cell(4).GetValue<string>(),
                MajorId = row.Cell(5).GetValue<string>(),
                CapstoneId = row.Cell(6).GetValue<string>(),
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

            await SyncUsersToFUCService(users, userType, emailImporter, numberOfUsersInBatchSize, attempTime++);
            users.Clear();
            numberOfUsersInBatchSize = 0;
        }

        return OperationResult.Success();
    }

    private static bool IsValidFile(string userType, IFormFile file)
    {
        return file != null && file.Length > 0 && 
            file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) && 
            file.FileName.Contains(userType, StringComparison.OrdinalIgnoreCase);
    }

    private async Task SyncUsersToFUCService(List<UserSync> users, string userType, string emailImporter, int numberOfUsersSync, int attemptTime)
    {
        if (numberOfUsersSync > 0)
        {
            logger.LogInformation("{SyncCount} synced in attemp: {Time}", numberOfUsersSync, attemptTime);

            // Sync user into FUC service
            await publishEndpoint.Publish(new UsersSyncMessage
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

        var result = await userManager.CreateAsync(user, "Pass123@");
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }

        result = await userManager.AddToRoleAsync(user, role);

        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }
    }
}
