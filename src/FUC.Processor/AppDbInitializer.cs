using FUC.Processor.Data;
using FUC.Processor.Models;

namespace FUC.Processor;

public class AppDbInitializer
{
    public static async Task SeedData(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var services = serviceScope.ServiceProvider;
            var context = services.GetRequiredService<ProcessorDbContext>();

            context.Database.EnsureCreated();

            if (!context.Set<Reminder>().Any())
            {
                context.Add(new Reminder
                {
                    Content = "Test",
                    ReminderType = "Test",
                    RemindDate = DateTime.UtcNow.AddDays(1),
                    RemindFor = "superadmin@fpt.edu.vn"
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
