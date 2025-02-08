using System.Security.Claims;
using System.Text.Json;
using FUC.Data.Data;
using FUC.Data.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FUC.API.SeedData;

public class AppDbInitializer
{
    public static async Task SeedData(IApplicationBuilder applicationBuilder)
    {
        // Read and deserialize combined JSON file
        var majorGroupData = await File.ReadAllTextAsync("SeedData/MajorGroup.json");
        var campusData = await File.ReadAllTextAsync("SeedData/Campus.json");
        var semesterData = await File.ReadAllTextAsync("SeedData/Semester.json");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var majorGroups = JsonSerializer.Deserialize<List<MajorGroup>>(majorGroupData, options);
        var campuses = JsonSerializer.Deserialize<List<Campus>>(campusData, options);
        var semesters = JsonSerializer.Deserialize<List<Semester>>(semesterData, options);
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var services = serviceScope.ServiceProvider;
            var context = services.GetService<DbContext>();

            var httpContextAccessor = applicationBuilder.ApplicationServices.GetRequiredService<IHttpContextAccessor>();

            // Manually create a fake HttpContext with a test user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "super-admin-id"),
                new Claim("name", "superadmin"),
                new Claim(ClaimTypes.Email, "superadmin@fpt.edu.vn"),
                new Claim(ClaimTypes.GivenName, "superadmin")
            };

            var identity = new ClaimsIdentity(claims, "AdminAuth");
            var user = new ClaimsPrincipal(identity);

            // Set the fake user in IHttpContextAccessor
            httpContextAccessor.HttpContext = new DefaultHttpContext { User = user };

            // Ensure database is created
            context.Database.EnsureCreated();

            // Seed data
            if (!context.Set<MajorGroup>().Any() && majorGroups is not null)
            {
                foreach (var majorGroup in majorGroups)
                {
                    // Add each Major and its nested Capstones
                    foreach (var major in majorGroup.Majors)
                    {
                        foreach (var capstone in major.Capstones)
                        {
                            context.Set<Capstone>().Add(capstone);
                        }
                        context.Set<Major>().Add(major);
                    }
                    context.Set<MajorGroup>().Add(majorGroup);
                }
            }

            if (!context.Set<Campus>().Any() && campuses is not null)
            {
                context.Set<Campus>().AddRange(campuses);
            }
            
            if (!context.Set<Semester>().Any() && semesters is not null)
            {
                foreach (var semester in semesters)
                {
                    if (semester.StartDate.Kind == DateTimeKind.Unspecified)
                    {
                        semester.StartDate = DateTime.SpecifyKind(semester.StartDate, DateTimeKind.Utc);
                    }
                    
                    if (semester.EndDate.Kind == DateTimeKind.Unspecified)
                    {
                        semester.EndDate = DateTime.SpecifyKind(semester.EndDate, DateTimeKind.Utc);
                    }
                    context.Set<Semester>().Add(semester);
                }
            }

            await context.SaveChangesAsync();

        }
    }
}
