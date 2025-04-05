using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class ArchiveDataStudentsEvent : IntegrationEvent
{
    public IEnumerable<string> StudentsCode { get; set; }
}
