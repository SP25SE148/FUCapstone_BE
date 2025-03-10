﻿using FUC.Data.Abstractions;
using FUC.Data.Enums;

namespace FUC.Data.Entities;

public sealed class Group : AuditableSoftDeleteEntity
{
    public Guid Id { get; set; }
    public string SemesterId { get; set; }
    public string MajorId { get; set; }
    public string CampusId { get; set; }
    public string? SupervisorId { get; set; }
    public string CapstoneId { get; set; }

    public string? TopicCode { get; set; }
    public string GroupCode { get; set; } = string.Empty;
    public GroupStatus Status { get; set; }

    public Major Major { get; set; } = null!;
    public Semester Semester { get; set; } = null!;

    public Campus Campus { get; set; } = null!;
    public Capstone Capstone { get; set; } = null!;
    public Supervisor Supervisor { get; set; } = null!;
    public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
    public ICollection<TopicRequest> TopicRequests { get; set; } = new List<TopicRequest>();
    public ProjectProgress ProjectProgress { get; set; } = null!;
}
