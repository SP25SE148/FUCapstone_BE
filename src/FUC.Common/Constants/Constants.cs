namespace FUC.Common.Constants;

public static class ProblemDetailTypes
{
    public const string BadRequestType = "https://tools.ietf.org/html/rfc7231#section-6.5.1";

    public const string InternalExceptionType = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
}

public static class UserRoles
{
    public const string Student = nameof(Student);
    public const string Supervisor = nameof(Supervisor);
    public const string SuperAdmin = nameof(SuperAdmin);
    public const string Manager = nameof(Manager);
    public const string Admin = nameof(Admin);
}

public static class TopicAppraisalRequirement
{
    public const int SupervisorAppraisalMinimum = 2;
}
