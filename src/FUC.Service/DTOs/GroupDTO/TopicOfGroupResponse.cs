using FUC.Service.DTOs.TopicDTO;

namespace FUC.Service.DTOs.GroupDTO;

public sealed record TopicOfGroupResponse(TopicResponse? TopicResponse, GroupResponse GroupResponse);
