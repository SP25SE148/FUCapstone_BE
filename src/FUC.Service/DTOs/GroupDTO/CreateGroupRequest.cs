using System.Collections.ObjectModel;

namespace FUC.Service.DTOs.GroupDTO;

public record CreateGroupRequest(
    string SemesterId/*,
    string MajorId,
    string CampusId,
    string CapstoneId,
    IReadOnlyList<string> MembersId*/
    );
