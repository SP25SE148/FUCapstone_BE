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
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        
        var majorGroups = JsonSerializer.Deserialize<List<MajorGroup>>(majorGroupData, options);
        var campuses = JsonSerializer.Deserialize<List<Campus>>(campusData, options);
        
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var services = serviceScope.ServiceProvider;
            var context = services.GetService<DbContext>();

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
            
            await context.SaveChangesAsync();
            
        }
    }
}
