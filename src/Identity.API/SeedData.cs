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
            new IdentityRole { Name = "Student" },
            new IdentityRole { Name = "Supervisor" },
            new IdentityRole { Name = "SuperAdmin" },
            new IdentityRole { Name = "Manager" },
            new IdentityRole { Name = "Admin" },
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
                UserName = "manager1",
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

            result = await userMgr.AddToRoleAsync(manager, "Manager");

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
                UserName = "manager2",
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

            result = await userMgr.AddToRoleAsync(manager2, "Manager");

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
                UserName = "superadmin",
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

            result = await userMgr.AddToRoleAsync(superAdmin, "SuperAdmin");

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
