using CaseOSaurus.Domain.Enums;

namespace CaseOSaurus.Application.Queries.GetCases;

public class CaseSummary
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; }

    public CaseSummary() { }

    public CaseSummary(Guid id, string title, string status, string? assignedTo, DateTime createdAt)
    {
        Id = id;
        Title = title;
        Status = status;
        AssignedTo = assignedTo;
        CreatedAt = createdAt;
    }
}
