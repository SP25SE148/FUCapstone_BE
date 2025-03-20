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
                    Content = "Test1",
                    ReminderType = "Test",
                    RemindDate = DateTime.Now.AddMinutes(3),
                    RemindFor = "superadmin@fpt.edu.vn"
                });

                context.Add(new Reminder
                {
                    Content = "Test2",
                    ReminderType = "Test",
                    RemindDate = DateTime.SpecifyKind(DateTime.Now.AddMinutes(2), DateTimeKind.Unspecified),
                    RemindFor = "superadmin@fpt.edu.vn"
                });
            }

            if (!context.Set<Notification>().Any())
            {
                context.Add(new Notification
                {
                    IsRead = false,
                    Content = "Test",
                    UserCode = "SE173512",
                    ReferenceTarget = "Test",
                    Type = "Test",
                    CreatedDate = DateTime.Now,
                });

                context.Add(new Notification
                {
                    IsRead = false,
                    Content = "Test1",
                    UserCode = "SE173512",
                    ReferenceTarget = "Test1",
                    Type = "Test",
                    CreatedDate = DateTime.Now, 
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
