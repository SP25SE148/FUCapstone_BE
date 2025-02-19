using System.Text.Json;
using FUC.Common.Constants;
using Identity.API.Data;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Identity.API.SeedData;

public class SeedData
{
    public static async Task EnsureSeedDataAsync(WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        context.Database.Migrate();

        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var roles = new List<IdentityRole>
        {
            new IdentityRole { Name = UserRoles.Student },
            new IdentityRole { Name = UserRoles.Supervisor },
            new IdentityRole { Name = UserRoles.SuperAdmin },
            new IdentityRole { Name = UserRoles.Manager },
            new IdentityRole { Name = UserRoles.Admin },
        };

        if (roleMgr.Roles.Any() || userMgr.Users.Any())
            return;

        foreach (var role in roles)
        {
            await roleMgr.CreateAsync(role);
        }

        // Add Manager-FUC
        var superAdminsData = await File.ReadAllTextAsync("SeedData/SuperAdmin.json");
        var adminsData = await File.ReadAllTextAsync("SeedData/Admin.json");
        var managersData = await File.ReadAllTextAsync("SeedData/Manager.json");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var superAdmins = JsonSerializer.Deserialize<List<SeedUserModel>>(superAdminsData, options);
        var admins = JsonSerializer.Deserialize<List<SeedUserModel>>(adminsData, options);
        var managers = JsonSerializer.Deserialize<List<SeedUserModel>>(managersData, options);

        var taskAddSuperAdmins = CreateFUCManager(app, superAdmins!, UserRoles.SuperAdmin);
        var taskAddAdmins = CreateFUCManager(app, admins!, UserRoles.Admin);
        var taskAddManagers = CreateFUCManager(app, managers!, UserRoles.Manager);

        await Task.WhenAll(taskAddSuperAdmins, taskAddAdmins, taskAddManagers);
    }

    private static async Task CreateFUCManager(WebApplication app, List<SeedUserModel> usersSeed, string role)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var data in usersSeed)
        {
            var user = new ApplicationUser
            {
                UserCode = data.UserCode,
                FullName = data.FullName,
                UserName = data.Email,
                Email = data.Email,
                CampusId = data.CampusId,
                CapstoneId = data.CapstoneId,
                MajorId = data.MajorId,
                EmailConfirmed = true
            };

            var result = await userMgr.CreateAsync(user, "Pass123$");

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = await userMgr.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            Log.Debug("{Role} created", role);
        }
    }

    private class SeedUserModel
    {
        public string UserCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string CampusId { get; set; }
        public string CapstoneId { get; set; }
        public string MajorId { get; set; }
    }
}
