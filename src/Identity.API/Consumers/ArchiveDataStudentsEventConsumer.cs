using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.Options;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Identity.API.Consumers;

public class ArchiveDataStudentsEventConsumer : BaseEventConsumer<ArchiveDataStudentsEvent>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ArchiveDataStudentsEventConsumer> _logger;

    public ArchiveDataStudentsEventConsumer(ILogger<ArchiveDataStudentsEventConsumer> logger,
        UserManager<ApplicationUser> userManager,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _userManager = userManager; 
    }

    protected override async Task ProcessMessage(ArchiveDataStudentsEvent message)
    {
        _logger.LogInformation("Starting to consume the event {EventType}.", nameof(ArchiveDataStudentsEvent));

        using var semaphore = new SemaphoreSlim(10); // Max parallelism

        var tasks = message.StudentsCode.Select(async id =>
        {
            await semaphore.WaitAsync();
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }

                await Task.CompletedTask;
               
            }
            finally
            {
                semaphore.Release();
            }
        }).ToList();

        await Task.WhenAll(tasks);

        _logger.LogInformation("Done to delete students data.");
    }
}
