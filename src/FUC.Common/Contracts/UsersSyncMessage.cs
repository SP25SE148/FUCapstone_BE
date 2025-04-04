﻿using FUC.Common.Events;

namespace FUC.Common.Contracts;
public sealed class UsersSyncMessage : IntegrationEvent
{
    public int AttempTime { get; set; }

    public string CreatedBy { get; set; }

    public string UserType { get; set; }

    public IEnumerable<UserSync> UsersSync { get; set; }
}
