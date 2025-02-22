namespace FUC.Service.Extensions.Options;

public class S3Settings
{
    public string AWSAccessKeyId { get; set; } = string.Empty;

    public string AWSSecretAccessKey { get; set; } = string.Empty;

    public string Region { get; init; } = string.Empty;
}
