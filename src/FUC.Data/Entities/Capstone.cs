using FUC.Data.Abstractions;
using FUC.Data.Abstractions.Entities;

namespace FUC.Data.Entities;


public sealed class Capstone : SoftDeleteEntity 
{
    public Guid Id { get; set; }
    public Guid MajorId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int MinMember { get; set; }
    public int MaxMember { get; set; }
    public int ReviewCount { get; set; }
   
    
    public Major Major { get; set; } = null!;
    public ICollection<Group> Groups { get; set; } = new List<Group>();
    public ICollection<Student> Students { get; set; } = new List<Student>();
    
    
}
