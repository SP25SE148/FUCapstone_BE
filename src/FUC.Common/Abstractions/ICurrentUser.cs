﻿namespace FUC.Common.Abstractions;
public interface ICurrentUser
{
    public string Id { get; }
    public string Name { get; }
    public string Email { get; }
    public string UserCode { get; }
    public string MajorId { get; }
    public string CapstoneId { get; }
    public string CampusId { get; }
    public string Role { get;}
}

