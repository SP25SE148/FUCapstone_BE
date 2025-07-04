﻿using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public sealed class Semester : AuditableSoftDeleteEntity
{
    public string Id { get; set; } // semester code
    public string Name { get; set; } = string.Empty;
    public int MaxGroupsPerSupervisor { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public ICollection<Group> Groups { get; set; } = new List<Group>();
    public ICollection<TimeConfiguration>? TimeConfigurations { get; set; } = new List<TimeConfiguration>();
    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    public ICollection<ReviewCalendar> ReviewCalendars { get; set; } = new List<ReviewCalendar>();

    public ICollection<DefendCapstoneProjectInformationCalendar>
        DefendCapstoneProjectInformationCalendars { get; set; } = new List<DefendCapstoneProjectInformationCalendar>();
}
