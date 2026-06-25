using CaseOSaurus.Domain.Enums;

namespace CaseOSaurus.Domain.Entities;

public class UserCase
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public Priority Priority { get; private set; }
    public CaseType Type { get; private set; }
    public string Requestor { get; private set; }
    public string? AssignedTo { get; private set; }
    public CaseStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    private UserCase() { }

    public UserCase(
        string title,
        string? description,
        Priority priority,
        CaseType type,
        string requestor,
        string createdBy,
        string? assignedTo = null)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Priority = priority;
        Type = type;
        Requestor = requestor;
        AssignedTo = assignedTo;
        Status = CaseStatus.Open;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void UpdateStatus(CaseStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignTo(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        
        AssignedTo = userId;
        UpdatedAt = DateTime.UtcNow;
    }
}
