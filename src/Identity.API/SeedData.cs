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

        // Add student1
        var student1 = await userMgr.FindByEmailAsync("thangttse172374@fpt.edu.vn");
        if (student1 == null)
        {
            student1 = new ApplicationUser
            {
                UserName = "thangttse172374",
                Email = "thangttse172374@fpt.edu.vn",
                EmailConfirmed = true,
            };
            var result = await userMgr.CreateAsync(student1, "Pass123@");
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = await userMgr.AddToRoleAsync(student1, "Student");

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
            Log.Debug("student1 created");
        }
        else
        {
            Log.Debug("student1 already exists");
        }

        // Add student2
        var student2 = await userMgr.FindByEmailAsync("huyttse172375@fpt.edu.vn");
        if (student2 == null)
        {
            student2 = new ApplicationUser
            {
                UserName = "huyttse172375",
                Email = "huyttse172375@fpt.edu.vn",
                EmailConfirmed = true,
            };
            var result = await userMgr.CreateAsync(student2, "Pass123@");
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = await userMgr.AddToRoleAsync(student2, "Student");

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
            Log.Debug("student2 created");
        }
        else
        {
            Log.Debug("student2 already exists");
        }

        // Add manager
        var manager = await userMgr.FindByEmailAsync("manager1@fpt.edu.vn");
        if (manager == null)
        {
            manager = new ApplicationUser
            {
                UserName = "manager1",
                Email = "manager1@fpt.edu.vn",
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
    }
}
