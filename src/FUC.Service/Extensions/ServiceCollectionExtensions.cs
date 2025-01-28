using System.Reflection;
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

namespace FUC.Service.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services, IConfiguration configuration)
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
        services.AddScoped<ICapstoneService,CapstoneService>();
        services.AddScoped<IMajorService,MajorService>();
        services.AddScoped<IMajorGroupService,MajorGroupService>();
        
        // DI RabbitMQ
        services.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetExecutingAssembly());

            x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
