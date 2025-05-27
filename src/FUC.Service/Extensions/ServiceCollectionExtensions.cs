using System.Net.Security;
using System.Reflection;
using System.Security.Authentication;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using FluentValidation;
using FUC.Common.Abstractions;
using FUC.Service.Abstractions;
using FUC.Service.Extensions.Options;
using FUC.Service.Infrastructure;
using FUC.Service.Mapper;
using MassTransit;
using FUC.Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using FUC.Data.Data;
using FUC.Common.IntegrationEventLog;

namespace FUC.Service.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(ServiceProfiles));

        services.AddValidatorsFromAssembly(AssemblyReference.Assembly);

        // TODO: Add Services
        var test = configuration["ConnectionString:DefaultConnection"];

        // DI S3 services
        services.Configure<S3Settings>(configuration.GetSection(nameof(S3Settings)));
        services.AddScoped<IAmazonS3>(sp =>
        {
            var s3Settings = sp.GetRequiredService<IOptions<S3Settings>>().Value;
            BasicAWSCredentials AwsCredentials = new(s3Settings.AWSAccessKeyId, s3Settings.AWSSecretAccessKey);

            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(s3Settings.Region)
            };

            return new AmazonS3Client(AwsCredentials, config);
        });
        services.AddScoped<ITransferUtility, TransferUtility>();
        services.AddScoped<IS3Service, S3Service>();

        services.AddScoped<ICurrentUser, CurrentUser>();

        // DI Service
        services.AddScoped<ICampusService, CampusService>();
        services.AddScoped<ICapstoneService, CapstoneService>();
        services.AddScoped<IMajorService, MajorService>();
        services.AddScoped<IMajorGroupService, MajorGroupService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<ISemesterService, SemesterService>();
        services.AddScoped<IGroupMemberService, GroupMemberService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ISupervisorService, SupervisorService>();
        services.AddScoped<ITopicService, TopicService>();
        services.AddScoped<ISupervisorService, SupervisorService>();
        services.AddScoped<IDocumentsService, DocumentsService>();
        services.AddScoped<IReviewCalendarService, ReviewCalendarService>();
        services.AddScoped<IDefendCapstoneService, DefendCapstoneService>();
        services.AddScoped<ITimeConfigurationService, TimeConfigurationService>();
        services.AddScoped<IArchiveDataApplicationService, ArchiveDataApplicationService>();
        // Add EventLogService
        services.AddEventConsumerConfiguration(configuration);
        services.AddIntegrationEventLogService<FucDbContext>();

        // DI RabbitMQ
        services.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetExecutingAssembly());

            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("fuc", false));
            //x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((context, cfg) =>
            {
                // cfg.Host(configuration["RabbitMq:Host"], 15672, "/", host =>
                // {
                //     host.Username(configuration.GetValue("RabbitMq:Username", "guest"));
                //     host.Password(configuration.GetValue("RabbitMq:Password", "guest"));
                // });


                cfg.Host("kebnekaise.lmq.cloudamqp.com", 5671, "jxkfadoo", h =>
                {
                    h.Username("jxkfadoo");
                    h.Password("fnmvqw95gss64f0tFUVWYdGovpobPJ96"); // <-- thay đúng vào đây
                    h.UseSsl(s =>
                    {
                        s.Protocol = SslProtocols.Tls12;
                        s.ServerName = "kebnekaise.lmq.cloudamqp.com";
                    });
                });

                cfg.ConfigureEndpoints(context);
            });
        });


        return services;
    }
}
