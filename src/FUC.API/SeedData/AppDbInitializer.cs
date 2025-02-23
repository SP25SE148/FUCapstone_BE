using System.Security.Claims;
using System.Text.Json;
using FUC.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FUC.API.SeedData;

public class AppDbInitializer
{
    public static async Task SeedData(IApplicationBuilder applicationBuilder)
    {
        // Read and deserialize combined JSON file
        var majorGroupData = await File.ReadAllTextAsync("SeedData/MajorGroup.json");
        var campusData = await File.ReadAllTextAsync("SeedData/Campus.json");
        var semesterData = await File.ReadAllTextAsync("SeedData/Semester.json");
        var businessAreaData = await File.ReadAllTextAsync("SeedData/BusinessArea.json");
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var majorGroups = JsonSerializer.Deserialize<List<MajorGroup>>(majorGroupData, options);
        var campuses = JsonSerializer.Deserialize<List<Campus>>(campusData, options);
        var semesters = JsonSerializer.Deserialize<List<Semester>>(semesterData, options);
        var businessAreas = JsonSerializer.Deserialize<List<BusinessArea>>(businessAreaData, options); 
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
            if (!context.Set<BusinessArea>().Any() && businessAreas is not null)
            {
                await context.Set<BusinessArea>().AddRangeAsync(businessAreas);
            }
     
            if (!context.Set<MajorGroup>().Any() && majorGroups is not null)
            {
                foreach (var majorGroup in majorGroups)
                {
                    // Add each Major and its nested Capstones
                    foreach (var major in majorGroup.Majors)
                    {
                        foreach (var capstone in major.Capstones)
                        {
                            await context.Set<Capstone>().AddAsync(capstone);
                        }
                        await context.Set<Major>().AddAsync(major);
                    }
                    await context.Set<MajorGroup>().AddAsync(majorGroup);
                }
            }

            if (!context.Set<Campus>().Any() && campuses is not null)
            {
                await context.Set<Campus>().AddRangeAsync(campuses);
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
                    await context.Set<Semester>().AddAsync(semester);
                }
            }

            await context.SaveChangesAsync();

        }
    }
}
