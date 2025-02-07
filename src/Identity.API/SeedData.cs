using FUC.Common.Constants;
using Identity.API.Data;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Identity.API;

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

        if (roleMgr.Roles.Any() && userMgr.Users.Any())
            return;

        foreach (var role in roles)
        {
            await roleMgr.CreateAsync(role);
        }

        // Add manager
        var manager = await userMgr.FindByEmailAsync("manager1@fpt.edu.vn");
        if (manager == null)
        {
            manager = new ApplicationUser
            {
                UserCode = "manager1",
                FullName = "manager1",
                UserName = "manager1@fpt.edu.vn",
                Email = "manager1@fpt.edu.vn",
                CampusId = "HCM",
                CapstoneId = "SEP490",
                MajorId = "SE",
                EmailConfirmed = true
            };
            var result = await userMgr.CreateAsync(manager, "Pass123$");

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = await userMgr.AddToRoleAsync(manager, UserRoles.Manager);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            Log.Debug("manager created");
        }
        else
        {
            Log.Debug("manager already exists");
        }

        // Add manager
        var manager2 = await userMgr.FindByEmailAsync("manager2@fpt.edu.vn");
        if (manager2 == null)
        {
            manager2 = new ApplicationUser
            {
                UserCode = "manager2",
                FullName = "manager2",
                UserName = "manager2@fpt.edu.vn",
                Email = "manager2@fpt.edu.vn",
                CampusId = "HCM",
                CapstoneId = "SEP490",
                MajorId = "SE",
                EmailConfirmed = true
            };
            var result = await userMgr.CreateAsync(manager2, "Pass123$");

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = await userMgr.AddToRoleAsync(manager2, UserRoles.Manager);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            Log.Debug("manager2 created");
        }
        else
        {
            Log.Debug("manager2 already exists");
        }

        // Add superadmin
        var superAdmin = await userMgr.FindByEmailAsync("superadmin@fpt.edu.vn");
        if (superAdmin == null)
        {
            superAdmin = new ApplicationUser
            {
                UserCode = "superadmin",
                FullName = "superadmin",
                UserName = "superadmin@fpt.edu.vn",
                Email = "superadmin@fpt.edu.vn",
                CampusId = "All",
                CapstoneId = "All",
                MajorId = "All",
                EmailConfirmed = true
            };
            var result = await userMgr.CreateAsync(superAdmin, "Pass123$");

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = await userMgr.AddToRoleAsync(superAdmin, UserRoles.SuperAdmin);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            Log.Debug("superAdmin created");
        }
        else
        {
            Log.Debug("superAdmin already exists");
        }
    }
}
