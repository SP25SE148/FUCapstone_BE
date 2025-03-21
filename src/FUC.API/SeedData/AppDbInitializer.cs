using System.Security.Claims;
using System.Text.Json;
using FUC.Common.Helpers;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Service.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FUC.API.SeedData;

public static class AppDbInitializer
{
    public static async Task SeedData(IApplicationBuilder applicationBuilder)
    {
        // Read and deserialize combined JSON file
        var majorGroupData = await File.ReadAllTextAsync("SeedData/MajorGroup.json");
        var campusData = await File.ReadAllTextAsync("SeedData/Campus.json");
        var semesterData = await File.ReadAllTextAsync("SeedData/Semester.json");
        var businessAreaData = await File.ReadAllTextAsync("SeedData/BusinessArea.json");
        var templateData = await File.ReadAllTextAsync("SeedData/Template.json");
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var majorGroups = JsonSerializer.Deserialize<List<MajorGroup>>(majorGroupData, options);
        var campuses = JsonSerializer.Deserialize<List<Campus>>(campusData, options);
        var semesters = JsonSerializer.Deserialize<List<Semester>>(semesterData, options);
        var businessAreas = JsonSerializer.Deserialize<List<BusinessArea>>(businessAreaData, options); 
        var templates = JsonSerializer.Deserialize<List<TemplateDocument>>(templateData, options); 

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
                    semester.StartDate = semester.StartDate.StartOfDay();
                    semester.EndDate = semester.EndDate.EndOfDay();
                    await context.Set<Semester>().AddAsync(semester);
                }
            }

            if (!context.Set<TemplateDocument>().Any() && templates is not null)
            {
                await context.Set<TemplateDocument>().AddRangeAsync(templates);
            }

            await context.SaveChangesAsync();

        }
    }

    public static async Task SyncTemplateConfigurationKey(IApplicationBuilder applicationBuilder)
    {
        var lifetime = applicationBuilder.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

        lifetime.ApplicationStarted.Register(async () =>
        {
            try
            {
                using var scope = applicationBuilder.ApplicationServices.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<FucDbContext>();
                var bucketConfiguration = scope.ServiceProvider.GetRequiredService<S3BucketConfiguration>();

                var records = await dbContext.Set<TemplateDocument>()
                    .Where(t => t.IsFile && t.IsActive)
                    .ToListAsync();

                // EVALUATION_PROJECT_PROGRESS_KEY
                var activedKey = records.Find(
                    r => r.FileUrl.StartsWith(bucketConfiguration.EvaluationProjectProgressKey));

                ArgumentNullException.ThrowIfNull(activedKey);

                bucketConfiguration.EvaluationProjectProgressKey = activedKey.FileUrl;

                // REVIEWS_CALENDARS_KEY
                activedKey = records.Find(
                    r => r.FileUrl.StartsWith(bucketConfiguration.ReviewsCalendarsKey));

                ArgumentNullException.ThrowIfNull(activedKey);

                bucketConfiguration.ReviewsCalendarsKey = activedKey.FileUrl;

                // DEFENSE_CALENDAR_KEY
                activedKey = records.Find(
                    r => r.FileUrl.StartsWith(bucketConfiguration.DefenseCalendarKey));

                ArgumentNullException.ThrowIfNull(activedKey);

                bucketConfiguration.DefenseCalendarKey = activedKey.FileUrl;

                // EVALUATION_WEEKLY_KEY
                activedKey = records.Find(
                    r => r.FileUrl.StartsWith(bucketConfiguration.EvaluationWeeklyKey));

                ArgumentNullException.ThrowIfNull(activedKey);

                bucketConfiguration.EvaluationWeeklyKey = activedKey.FileUrl;

                // STUDENTS_TEMPLATE_KEY
                activedKey = records.Find(
                    r => r.FileUrl.StartsWith(bucketConfiguration.StudentsTemplateKey));

                ArgumentNullException.ThrowIfNull(activedKey);

                bucketConfiguration.StudentsTemplateKey = activedKey.FileUrl;

                // SUPERVISORS_TEMPLATE_KEY
                activedKey = records.Find(
                    r => r.FileUrl.StartsWith(bucketConfiguration.SupervisorsTemplateKey));

                ArgumentNullException.ThrowIfNull(activedKey);

                bucketConfiguration.SupervisorsTemplateKey = activedKey.FileUrl;

                // THESIS_COUNCIL_MEETING_MINUTES_TEMPLATE_KEY
                activedKey = records.Find(
                    r => r.FileUrl.StartsWith(bucketConfiguration.ThesisCouncilMeetingMinutesTemplateKey));

                ArgumentNullException.ThrowIfNull(activedKey);

                bucketConfiguration.ThesisCouncilMeetingMinutesTemplateKey = activedKey.FileUrl;
            }
            catch (Exception ex)
            {
                Log.Error($"Error during startup task: {ex.Message}");
                throw;
            }
        });
    }
}
