﻿namespace FUC.Service.DTOs.TopicAppraisalDTO;

public class AssignSupervisorAppraisalTopicRequest
{
    public Guid TopicId { get; set; }
    public required string SupervisorId { get; set; }
}
