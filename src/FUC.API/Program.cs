using FUC.API.Extensions;
using FUC.API.Middlewares;
using FUC.API.SeedData;
using FUC.Common.Cache;
using FUC.Data.Extensions;
using FUC.Service.Abstractions;
using FUC.Service.Extensions;
using FUC.Service.Extensions.Options;
using FUC.Service.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    var bucketConfiguration = new S3BucketConfiguration
    {
        FUCTopicBucket = Environment.GetEnvironmentVariable("TOPIC_BUCKET_NAME"),
        FUCTemplateBucket = Environment.GetEnvironmentVariable("TEMPLATE_BUCKET_NAME"),
        FUCGroupDocumentBucket = Environment.GetEnvironmentVariable("GROUP_DOCUMENT_BUCKET_NAME"),
        FUCThesisBucket = Environment.GetEnvironmentVariable("THESIS_BUCKET_NAME"),
        EvaluationProjectProgressKey = Environment.GetEnvironmentVariable("EVALUATION_PROJECT_PROGRESS_KEY"),
        EvaluationWeeklyKey = Environment.GetEnvironmentVariable("EVALUATION_WEEKLY_KEY"),
        ReviewsCalendarsKey = Environment.GetEnvironmentVariable("REVIEWS_CALENDARS_KEY"),
        DefenseCalendarKey = Environment.GetEnvironmentVariable("DEFENSE_CALENDAR_KEY"),
        StudentsTemplateKey = Environment.GetEnvironmentVariable("STUDENTS_TEMPLATE_KEY"),
        SupervisorsTemplateKey = Environment.GetEnvironmentVariable("SUPERVISORS_TEMPLATE_KEY"),
        ThesisCouncilMeetingMinutesTemplateKey = Environment.GetEnvironmentVariable("THESIS_COUNCIL_MEETING_MINUTES_TEMPLATE_KEY")
    };

    builder.Services.AddSingleton(bucketConfiguration);

    builder.Services.Configure<SystemConfiguration>(builder.Configuration.GetSection("SystemConfiguration"));
    builder.Services.AddSingleton<ISystemConfigurationService, SystemConfigurationService>();

    // Add services to the container.
    builder.Services.AddServices();
    builder.Services.AddJwtAuthentication(builder.Configuration);

    // Add DI for FUC.Data
    builder.Services.AddDataAccessServices(builder.Configuration);
    builder.Services.AddCacheConfiguration(builder.Configuration);

    // Add DI for FUC.Service 
    builder.Services.AddBusinessLogicServices(builder.Configuration);

    var app = builder.Build();

    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseCors(builder => builder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials() // to support a SignalR
        .WithOrigins("https://localhost:3000"));

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.UseSerilogRequestLogging();
    await AppDbInitializer.SeedData(app);
    await AppDbInitializer.SyncTemplateConfigurationKey(app);
    
    await app.RunAsync();

}
catch (Exception ex) when (
                            // https://github.com/dotnet/runtime/issues/60600
                            ex.GetType().Name is not "StopTheHostException"
                            // HostAbortedException was added in .NET 7, but since we target .NET 6 we
                            // need to do it this way until we target .NET 8
                            && ex.GetType().Name is not "HostAbortedException"
                        )
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    await Log.CloseAndFlushAsync();
}
